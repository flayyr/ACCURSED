using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatueSeeingField : MonoBehaviour
{
    public const int MaxShaderFields = 16;

    [Header("Field")]
    [Min(0.1f)]
    [SerializeField] private float maxRadius = 6f;

    [Min(0.01f)]
    [SerializeField] private float spreadDuration = 0.35f;

    [Min(0.01f)]
    [SerializeField] private float collapseDuration = 0.2f;

    [SerializeField] private AnimationCurve spreadCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Optional Visible Mask / Wave")]
    [Tooltip("Assign a child transform with a circular sprite/material. It scales with the field radius.")]
    [SerializeField] private Transform visualMask;

    [Tooltip("World-space diameter represented when Visual Mask has local scale (1,1,1). Usually 1 for a 1-unit circle sprite.")]
    [Min(0.001f)]
    [SerializeField] private float visualDiameterAtScaleOne = 1f;

    [SerializeField] private bool hideVisualAtZeroRadius = true;

    private float currentRadius;
    private bool targetSeeing;
    private Coroutine radiusRoutine;

    private static readonly List<StatueSeeingField> activeFields = new List<StatueSeeingField>();
    private static readonly Vector4[] shaderFieldData = new Vector4[MaxShaderFields];

    private static readonly int FieldCountID = Shader.PropertyToID("_StatueVisionFieldCount");
    private static readonly int FieldDataID = Shader.PropertyToID("_StatueVisionFields");

    public float CurrentRadius => currentRadius;
    public float MaxRadius => maxRadius;
    public bool TargetSeeing => targetSeeing;

    private void Awake()
    {
        ApplyVisualScale();
    }

    public void SetSeeing(bool seeing, bool instant = false)
    {
        targetSeeing = seeing;

        if (radiusRoutine != null)
        {
            StopCoroutine(radiusRoutine);
            radiusRoutine = null;
        }

        if (seeing)
            RegisterField();

        float targetRadius = seeing ? maxRadius : 0f;

        if (instant)
        {
            currentRadius = targetRadius;
            ApplyVisualScale();

            if (!seeing)
                UnregisterField();

            PushShaderGlobals();
            return;
        }

        radiusRoutine = StartCoroutine(AnimateRadius(targetRadius, seeing ? spreadDuration : collapseDuration));
    }

    private IEnumerator AnimateRadius(float targetRadius, float duration)
    {
        float startRadius = currentRadius;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / duration);
            float curvedT = spreadCurve != null ? spreadCurve.Evaluate(t) : t;

            currentRadius = Mathf.Lerp(startRadius, targetRadius, curvedT);
            ApplyVisualScale();
            PushShaderGlobals();

            yield return null;
        }

        currentRadius = targetRadius;
        ApplyVisualScale();

        if (!targetSeeing && currentRadius <= 0.001f)
            UnregisterField();

        PushShaderGlobals();
        radiusRoutine = null;
    }

    private void ApplyVisualScale()
    {
        if (visualMask == null)
            return;

        if (hideVisualAtZeroRadius)
            visualMask.gameObject.SetActive(currentRadius > 0.001f);

        float diameter = currentRadius * 2f;
        float scale = diameter / visualDiameterAtScaleOne;

        visualMask.localScale = new Vector3(scale, scale, 1f);
    }

    private void RegisterField()
    {
        if (!activeFields.Contains(this))
            activeFields.Add(this);
    }

    private void UnregisterField()
    {
        activeFields.Remove(this);
    }

    private static void PushShaderGlobals()
    {
        int count = 0;

        for (int i = activeFields.Count - 1; i >= 0; i--)
        {
            if (activeFields[i] == null)
                activeFields.RemoveAt(i);
        }

        for (int i = 0; i < activeFields.Count && count < MaxShaderFields; i++)
        {
            StatueSeeingField field = activeFields[i];

            if (field == null || field.currentRadius <= 0.001f)
                continue;

            Vector3 p = field.transform.position;
            shaderFieldData[count] = new Vector4(p.x, p.y, field.currentRadius, 0f);
            count++;
        }

        for (int i = count; i < MaxShaderFields; i++)
            shaderFieldData[i] = Vector4.zero;

        Shader.SetGlobalInt(FieldCountID, count);
        Shader.SetGlobalVectorArray(FieldDataID, shaderFieldData);
    }

    /// <summary>
    /// Future enemy/environment code can call this without knowing which statue owns the field.
    /// </summary>
    public static bool IsPointInsideAnyVisionField(Vector3 worldPosition)
    {
        Vector2 point = worldPosition;

        for (int i = 0; i < activeFields.Count; i++)
        {
            StatueSeeingField field = activeFields[i];

            if (field == null || field.currentRadius <= 0.001f)
                continue;

            Vector2 center = field.transform.position;
            float radius = field.currentRadius;

            if ((point - center).sqrMagnitude <= radius * radius)
                return true;
        }

        return false;
    }

    public bool ContainsPoint(Vector3 worldPosition)
    {
        Vector2 point = worldPosition;
        Vector2 center = transform.position;
        return (point - center).sqrMagnitude <= currentRadius * currentRadius;
    }

    private void OnDisable()
    {
        if (radiusRoutine != null)
        {
            StopCoroutine(radiusRoutine);
            radiusRoutine = null;
        }

        currentRadius = 0f;
        targetSeeing = false;
        UnregisterField();
        ApplyVisualScale();
        PushShaderGlobals();
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, maxRadius);
    }
#endif
}
