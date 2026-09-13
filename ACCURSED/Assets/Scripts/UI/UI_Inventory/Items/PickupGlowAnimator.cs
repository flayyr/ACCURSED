using UnityEngine;

public class PickupGlowAnimator : MonoBehaviour
{
    [Header("Beam")]
    [SerializeField] private Transform beam;

    [Header("Scale Pulse")]
    [SerializeField] private float scaleAmount = 0.04f;
    [SerializeField] private float scaleSpeed = 2f;

    [Header("Sway")]
    [SerializeField] private float swayAmount = 1.5f;
    [SerializeField] private float swaySpeed = 1.3f;

    private Vector3 startingScale;
    private Quaternion startingRotation;

    private void Awake()
    {
        if (beam == null)
            return;

        startingScale = beam.localScale;
        startingRotation = beam.localRotation;
    }

    private void Update()
    {
        if (beam == null)
            return;

        float scalePulse = Mathf.Sin(Time.time * scaleSpeed) * scaleAmount;

        beam.localScale = new Vector3(
            startingScale.x + scalePulse,
            startingScale.y - scalePulse * 0.5f,
            startingScale.z);

        float sway = Mathf.Sin(Time.time * swaySpeed) * swayAmount;

        beam.localRotation = startingRotation * Quaternion.Euler(0f, 0f, sway);
    }
}