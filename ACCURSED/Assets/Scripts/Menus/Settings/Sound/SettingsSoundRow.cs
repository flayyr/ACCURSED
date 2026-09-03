using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsSoundRow : SettingsSliderSelectable
{
    [Header("Text")]
    [SerializeField] private TMP_Text bindText;

    [Header("Slider")]
    [SerializeField] private Slider soundSlider;

    [Header("Highlight")]
    [SerializeField] private RectTransform rowBackground;

    [SerializeField] private GameObject selectionHighlight;

    private SettingsSoundSetting setting;
    private SettingsSoundManager soundManager;

    private bool hasBeenInitialized;
    private bool rowSelected;

    public SettingsSoundSetting Setting
    {
        get { return setting; }
    }

    public void Initialize(SettingsSoundSetting assignedSetting, string customDisplayName = "")
    {
        FindReferences();
        PrepareHighlight();

        setting = assignedSetting;
        hasBeenInitialized = true;

        if (soundManager == null)
        {
            Debug.LogError(name + ": SettingsSoundManager was not found. " + "Put SettingsSoundManager on an ancestor of the Sound page, " +
                "preferably the SettingsPrefab root.", gameObject);

            return;
        }

        if (soundSlider == null)
        {
            Debug.LogError(name + ": No Unity Slider was found inside this row.", gameObject);

            return;
        }

        slider = soundSlider;

        soundSlider.minValue = 0f;
        soundSlider.maxValue = 1f;
        soundSlider.wholeNumbers = false;

        soundSlider.onValueChanged.RemoveListener(HandleSliderChanged);

        soundSlider.onValueChanged.AddListener(HandleSliderChanged);

        if (bindText != null)
        {
            bindText.text = string.IsNullOrWhiteSpace(customDisplayName)
                ? soundManager.GetDisplayName(setting)
                : customDisplayName;
        }

        soundManager.RegisterRow(this);

        RefreshDisplay();
        RefreshHighlight();
    }

    private void FindReferences()
    {
        if (soundManager == null)
            soundManager = GetComponentInParent<SettingsSoundManager>(true);

        if (soundSlider == null)
            soundSlider = GetComponentInChildren<Slider>(true);
    }

    private void PrepareHighlight()
    {
        if (selectionHighlight == null)
            return;

        if (rowBackground != null)
        {
            RectTransform highlightRect = selectionHighlight.GetComponent<RectTransform>();

            if (highlightRect != null)
            {
                if (highlightRect.parent != rowBackground)
                    highlightRect.SetParent(rowBackground, false );

                highlightRect.anchorMin = Vector2.zero;

                highlightRect.anchorMax = Vector2.one;

                highlightRect.offsetMin = Vector2.zero;

                highlightRect.offsetMax = Vector2.zero;

                highlightRect.localScale = Vector3.one;

                highlightRect.localRotation = Quaternion.identity;
            }
        }

        Graphic[] graphics = selectionHighlight.GetComponentsInChildren<Graphic>(true);

        foreach (Graphic graphic in graphics)
        {
            graphic.raycastTarget = false;
        }

        selectionHighlight.SetActive(false);
    }

    private void HandleSliderChanged(float value)
    {
        if (!hasBeenInitialized)
            return;

        if (soundManager == null)
            FindReferences();

        if (soundManager == null)
        {
            Debug.LogWarning(name + ": SettingsSoundManager was not found.");

            return;
        }

        soundManager.SetValue(setting, value);
    }

    public void RefreshDisplay()
    {
        if (!hasBeenInitialized)
            return;

        if (soundManager == null)
            FindReferences();

        if (soundManager == null || soundSlider == null)
            return;

        soundSlider.SetValueWithoutNotify(soundManager.GetValue(setting));
    }

    public override void SetSelected(bool selected)
    {
        rowSelected = selected;

        RefreshHighlight();
    }

    private void RefreshHighlight()
    {
        if (selectionHighlight == null)
            return;

        selectionHighlight.SetActive(rowSelected);
    }

    private void OnDestroy()
    {
        if (soundSlider != null)
            soundSlider.onValueChanged.RemoveListener(HandleSliderChanged);

        if (soundManager != null)
            soundManager.UnregisterRow(this);
    }
}