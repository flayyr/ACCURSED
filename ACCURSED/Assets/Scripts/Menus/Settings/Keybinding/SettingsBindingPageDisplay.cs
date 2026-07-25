using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsBindingPageDisplay : MonoBehaviour
{
    [System.Serializable]
    public class BindingEntry
    {
        [Header("Entry Type")]
        public bool isInteractiveKeybind;

        [Header("Keybind")]
        public SettingsKeybindAction keybindAction;

        [Header("Displayed Text")]
        public string actionName;
        public string bindingText;
        public string statusText;

        public BindingEntry()
        {
        }

        /// <summary>
        /// Creates an interactive entry using the manager's display name.
        /// </summary>
        public BindingEntry(SettingsKeybindAction keybindAction, string statusText = "")
        {
            isInteractiveKeybind = true;
            this.keybindAction = keybindAction;
            this.statusText = statusText;
            actionName = "";
            bindingText = "";
        }

        /// <summary>
        /// Creates an interactive entry with a custom displayed name.
        /// </summary>
        public BindingEntry(SettingsKeybindAction keybindAction, string actionName, string statusText = "")
        {
            isInteractiveKeybind = true;
            this.keybindAction = keybindAction;
            this.actionName = actionName;
            bindingText = "";
            this.statusText = statusText;
        }

        /// <summary>
        /// Creates a non-interactive display-only entry.
        /// </summary>
        public BindingEntry(string actionName, string bindingText, string statusText = "")
        {
            isInteractiveKeybind = false;
            this.actionName = actionName;
            this.bindingText = bindingText;
            this.statusText = statusText;
        }
    }

    [System.Serializable]
    public class BindingSection
    {
        public string sectionTitle;
        public List<BindingEntry> entries = new List<BindingEntry>();

        public BindingSection()
        {
        }

        public BindingSection(string sectionTitle, List<BindingEntry> entries)
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
    [SerializeField] private SettingsBindingDisplayRow rowPrefab;

    [Header("Displayed Sections")]
    [SerializeField]
    private List<BindingSection> sections = new List<BindingSection>();

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

        foreach (BindingSection section in sections)
        {
            if (section == null)
                continue;

            CreateSectionHeader(section.sectionTitle);

            if (section.entries == null)
                continue;

            foreach (BindingEntry entry in section.entries)
            {
                if (entry != null)
                    CreateEntryRow(entry);
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

        if (rowPrefab == null)
        {
            Debug.LogWarning(
                name +
                ": Row Prefab has not been assigned.");
            return false;
        }

        return true;
    }

    private void CreateSectionHeader(
        string sectionTitle)
    {
        GameObject headerObject =
            Instantiate(
                sectionHeaderPrefab,
                contentRoot);

        headerObject.name =
            "Section - " + sectionTitle;

        TMP_Text headerText =
            headerObject.GetComponentInChildren
                <TMP_Text>(true);

        if (headerText != null)
            headerText.text = sectionTitle;
    }

    private void CreateEntryRow(BindingEntry entry)
    {
        SettingsBindingDisplayRow newRow = Instantiate(rowPrefab, contentRoot);

        string rowName = string.IsNullOrWhiteSpace(entry.actionName)
            ? entry.keybindAction.ToString()
            : entry.actionName;

        newRow.SetDisplay(entry.actionName, entry.bindingText, entry.statusText);

        newRow.gameObject.name = "Row - " + rowName;

        if (!entry.isInteractiveKeybind)
            return;

        SettingsKeybindRow keybindRow = newRow.GetComponent<SettingsKeybindRow>();

        if (keybindRow == null)
        {
            keybindRow = newRow.GetComponentInChildren<SettingsKeybindRow>(true);
        }

        if (keybindRow == null)
        {
            Debug.LogWarning(newRow.gameObject.name + ": Interactive entry has no SettingsKeybindRow.", newRow.gameObject);
            return;
        }

        keybindRow.Initialize(entry.keybindAction);
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

    [ContextMenu("Load Game Defaults")]
    public void LoadGameDefaults()
    {
        sections = CreatePlaceholderSections();
        hasBuilt = false;

        if (Application.isPlaying && isActiveAndEnabled)
            BuildDisplay();
    }

    [ContextMenu("Load Keyboard Defaults")]
    public void LoadKeyboardDefaults()
    {
        sections = new List<BindingSection>
        {
            new BindingSection("Game",
                new List<BindingEntry>
                {
                    new BindingEntry(SettingsKeybindAction.MoveForwards, "Move Forward"),
                    new BindingEntry(SettingsKeybindAction.MoveBackwards, "Move Backward"),
                    new BindingEntry(SettingsKeybindAction.MoveLeft, "Move Left"),
                    new BindingEntry(SettingsKeybindAction.MoveRight, "Move Right"),
                    new BindingEntry(SettingsKeybindAction.Dodge, "Dodge"),
                    new BindingEntry(SettingsKeybindAction.Sprint, "Sprint"),
                    new BindingEntry(SettingsKeybindAction.Walk, "Walk")
                }),

            new BindingSection("Actions",
                new List<BindingEntry>
                {
                    new BindingEntry(SettingsKeybindAction.Attack, "Attack"),
                    new BindingEntry(SettingsKeybindAction.HeavyAttack, "Heavy Attack"),
                    new BindingEntry(SettingsKeybindAction.Parry, "Parry"),
                    new BindingEntry(SettingsKeybindAction.Heal, "Heal"),
                    new BindingEntry(SettingsKeybindAction.UseItem, "Use Item"),
                    new BindingEntry(SettingsKeybindAction.Remembrance, "Remembrance"),
                    new BindingEntry(SettingsKeybindAction.Vestige, "Vestige"),
                    new BindingEntry(SettingsKeybindAction.Interact, "Interact")
                }),

            new BindingSection("Interface",
                new List<BindingEntry>
                {
                    new BindingEntry(SettingsKeybindAction.Menu, "Menu"),
                    new BindingEntry(SettingsKeybindAction.HUD, "HUD")
                }),

            new BindingSection("Test",
                new List<BindingEntry>
                {
                    new BindingEntry(SettingsKeybindAction.Test, "Test")
                })
        };

        hasBuilt = false;

        if (Application.isPlaying && isActiveAndEnabled)
            BuildDisplay();
    }

    [ContextMenu("Load Controller Defaults")]
    public void LoadControllerDefaults()
    {
        sections = CreatePlaceholderSections();
        hasBuilt = false;

        if (Application.isPlaying && isActiveAndEnabled)
            BuildDisplay();
    }

    [ContextMenu("Load Display Defaults")]
    public void LoadDisplayDefaults()
    {
        sections = CreatePlaceholderSections();
        hasBuilt = false;

        if (Application.isPlaying && isActiveAndEnabled)
            BuildDisplay();
    }

    [ContextMenu("Load Sound Defaults")]
    public void LoadSoundDefaults()
    {
        sections = CreatePlaceholderSections();
        hasBuilt = false;

        if (Application.isPlaying && isActiveAndEnabled)
            BuildDisplay();
    }

    private static List<BindingSection> CreatePlaceholderSections()
    {
        return new List<BindingSection>
        {
            new BindingSection("Test",
                new List<BindingEntry>
                {
                    new BindingEntry("Test", "T"),
                })
        };
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
            Debug.LogWarning(name + ": No SettingsMenuNavigator was found " + "in the parent hierarchy.");
        }
    }
}
