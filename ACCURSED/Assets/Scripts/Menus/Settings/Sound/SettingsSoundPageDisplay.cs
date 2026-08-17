using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsSoundPageDisplay : MonoBehaviour
{
    [System.Serializable]
    public class SoundEntry
    {
        public SettingsSoundSetting setting;

        [Tooltip("Optional. Leave empty to use " + "SettingsSoundManager's display name.")]
        public string displayName;

        public SoundEntry()
        {

        }

        public SoundEntry(SettingsSoundSetting setting, string displayName = "")
        {
            this.setting = setting;
            this.displayName = displayName;
        }
    }

    [System.Serializable]
    public class SoundSection
    {
        public string sectionTitle;

        public List<SoundEntry> entries = new List<SoundEntry>();

        public SoundSection()
        {
            
        }

        public SoundSection(string sectionTitle, List<SoundEntry> entries)
        {
            this.sectionTitle = sectionTitle;
            this.entries = entries;
        }
    }

    [Header("Page References")]
    [SerializeField] private RectTransform contentRoot;
    [SerializeField] private ScrollRect scrollRect;

    [Header("Prefabs")]
    [SerializeField] private GameObject sectionHeaderPrefab;

    [SerializeField]
    private SettingsSoundRow sliderRowPrefab;

    [Header("Displayed Sections")]
    [SerializeField]
    private List<SoundSection> sections = new List<SoundSection>();

    private bool hasBuilt;

    private void OnEnable()
    {
        if (!Application.isPlaying)
            return;

        if (!hasBuilt)
            BuildDisplay();

        StartCoroutine(ResetScrollNextFrame());
    }

    [ContextMenu("Build Display")]
    public void BuildDisplay()
    {
        if (!ValidateReferences())
            return;

        ClearDisplay();

        foreach (SoundSection section in sections)
        {
            if (section == null)
                continue;

            CreateSectionHeader(section.sectionTitle);

            if (section.entries == null)
                continue;

            foreach (SoundEntry entry in section.entries)
            {
                if (entry != null)
                    CreateSliderRow(entry);
            }
        }

        hasBuilt = true;

        Canvas.ForceUpdateCanvases();

        LayoutRebuilder.ForceRebuildLayoutImmediate(contentRoot);

        if (scrollRect != null)
            scrollRect.verticalNormalizedPosition = 1f;
        

        StartCoroutine(RefreshNavigatorNextFrame());
    }

    private bool ValidateReferences()
    {
        if (contentRoot == null)
        {
            Debug.LogWarning(name + ": Content Root has not been assigned.");

            return false;
        }

        if (sectionHeaderPrefab == null)
        {
            Debug.LogWarning(name + ": Section Header Prefab has not been assigned.");

            return false;
        }

        if (sliderRowPrefab == null)
        {
            Debug.LogWarning(
                name +
                ": Slider Row Prefab has not been assigned."
           );

            return false;
        }

        return true;
    }

    private void CreateSectionHeader(string sectionTitle)
    {
        GameObject headerObject = Instantiate(sectionHeaderPrefab, contentRoot);

        headerObject.name = "Section - " + sectionTitle;

        TMP_Text headerText = headerObject.GetComponentInChildren<TMP_Text>(true);

        if (headerText != null)
            headerText.text = sectionTitle;
    }

    private void CreateSliderRow(SoundEntry entry)
    {
        SettingsSoundRow newRow = Instantiate(sliderRowPrefab, contentRoot);

        string rowName = string.IsNullOrWhiteSpace(entry.displayName)
            ? entry.setting.ToString()
            : entry.displayName;

        newRow.gameObject.name = "Slider - " + rowName;

        newRow.Initialize(entry.setting, entry.displayName
        );
    }

    [ContextMenu("Clear Display")]
    public void ClearDisplay()
    {
        if (contentRoot == null)
            return;

        for (int i = contentRoot.childCount - 1; i >= 0; i--)
        {
            GameObject child = contentRoot.GetChild(i).gameObject;

            child.SetActive(false);

            if (Application.isPlaying)
            {
                Destroy(child);
            }
            else
            {
                DestroyImmediate(child);
            }
        }

        hasBuilt = false;
    }

    [ContextMenu("Load Sound Defaults")]
    public void LoadSoundDefaults()
    {
        sections = new List<SoundSection>
        {
            new SoundSection("Volume",
                new List<SoundEntry>
                {
                    new SoundEntry(SettingsSoundSetting.MasterVolume, "Master Volume"),

                    new SoundEntry(SettingsSoundSetting.MusicVolume, "Music Volume"),

                    new SoundEntry(SettingsSoundSetting.SoundEffectsVolume, "Sound Effects"),

                    new SoundEntry(SettingsSoundSetting.AmbienceVolume, "Ambience"),

                    new SoundEntry(SettingsSoundSetting.UIVolume, "UI Volume")
                })
        };

        hasBuilt = false;

        if (Application.isPlaying && isActiveAndEnabled)
            BuildDisplay();
    }

    private IEnumerator ResetScrollNextFrame()
    {
        yield return null;

        Canvas.ForceUpdateCanvases();

        if (contentRoot != null)
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentRoot);

        if (scrollRect != null)
            scrollRect.verticalNormalizedPosition = 1f;
    }

    private IEnumerator RefreshNavigatorNextFrame()
    {
        yield return null;

        Canvas.ForceUpdateCanvases();

        if (contentRoot != null)
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentRoot);

        SettingsMenuNavigator navigator = GetComponentInParent<SettingsMenuNavigator>();

        if (navigator != null)
        {
            navigator.RefreshGeneratedOptions();
        }
        else
        {
            Debug.LogWarning(name + ": No SettingsMenuNavigator " + "was found in the parent hierarchy.");
        }
    }
}