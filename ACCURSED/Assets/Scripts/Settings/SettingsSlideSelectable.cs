using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SettingsSliderSelectable : SettingsMenuSelectable
{
    [Header("Slider")]
    public Slider slider;

    [Header("Adjust Mode Visual")]
    public GameObject adjustingPrefab;

    private GameObject adjustingInstance;

    [Header("Keyboard Adjust Instruction")]
    public GameObject instructionPrefab;
    public Vector2 instructionOffset = new Vector2(0f, -35f);

    private GameObject instructionInstance;

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
        Debug.Log("Slider Activate on: " + gameObject.name);

        if (navigator != null)
        {
            Debug.Log("Slider adjust mode");
            navigator.StartSliderAdjustMode(this);
        }
        else
        {
            Debug.LogWarning("Slider has no navigator: " + gameObject.name);
        }
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
        {
            SpawnAdjustingVisual();
            SpawnInstructionText();
        }
        else
        {
            RemoveAdjustingVisual();
            RemoveInstructionText();
        }
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

    private void SpawnInstructionText()
    {
        RemoveInstructionText();

        Debug.Log("Selected");

        if (instructionPrefab == null)
            return;

        RectTransform sliderRect = GetComponent<RectTransform>();

        if (sliderRect == null)
            return;

        instructionInstance = Instantiate(instructionPrefab, sliderRect);

        RectTransform rect = instructionInstance.GetComponent<RectTransform>();

        if (rect == null)
        {
            Destroy(instructionInstance);
            instructionInstance = null;
            return;
        }

        rect.anchorMin = new Vector2(0.5f, 0f);
        rect.anchorMax = new Vector2(0.5f, 0f);
        rect.pivot = new Vector2(0.5f, 1f);

        rect.anchoredPosition = instructionOffset;
        rect.localScale = Vector3.one;
        rect.localRotation = Quaternion.identity;

        rect.SetAsLastSibling();
    }

    private void RemoveInstructionText()
    {
        if (instructionInstance != null)
        {
            Destroy(instructionInstance);
            instructionInstance = null;
        }
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        RemoveAdjustingVisual();
        RemoveInstructionText();
    }
}