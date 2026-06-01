using UnityEngine;
using UnityEngine.UI;

public class SettingsSliderSelectable : SettingsMenuSelectable
{
    [Header("Slider")]
    public Slider slider;

    [Header("Adjust Mode Visual")]
    public GameObject adjustingPrefab;

    private GameObject adjustingInstance;

    private void Awake()
    {
        if (slider == null)
            slider = GetComponent<Slider>();
    }

    protected override RectTransform GetVisualTarget()
    {
        if (slider != null && slider.handleRect != null)
            return slider.handleRect;

        return base.GetVisualTarget();
    }

    public override void Activate()
    {
        if (navigator != null)
            navigator.StartSliderAdjustMode(this);
    }

    public void AdjustSlider(float amount)
    {
        if (slider == null)
            return;

        slider.value += amount;
    }

    public void SetAdjusting(bool adjusting)
    {
        if (adjusting)
            SpawnAdjustingVisual();
        else
            RemoveAdjustingVisual();
    }

    private void SpawnAdjustingVisual()
    {
        RemoveAdjustingVisual();

        if (adjustingPrefab == null)
            return;

        RectTransform target = GetVisualTarget();

        if (target == null)
            return;

        adjustingInstance = Instantiate(adjustingPrefab, target);

        RectTransform rect = adjustingInstance.GetComponent<RectTransform>();

        if (rect == null)
        {
            Destroy(adjustingInstance);
            adjustingInstance = null;
            return;
        }

        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);

        rect.anchoredPosition = Vector2.zero;
        rect.localScale = Vector3.one;
        rect.localRotation = Quaternion.identity;

        rect.SetAsLastSibling();
    }

    private void RemoveAdjustingVisual()
    {
        if (adjustingInstance != null)
        {
            Destroy(adjustingInstance);
            adjustingInstance = null;
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        RemoveAdjustingVisual();
    }
}