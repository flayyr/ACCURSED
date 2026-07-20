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

        [Header("Interactive Keybind")]
        public SettingsKeybindAction keybindAction;

        [Header("Static Display")]
        public string actionName;
        public string bindingText;

        [Header("Optional")]
        public string statusText;

        public BindingEntry()
        {
        }

        /// <summary>
        /// Creates an interactive keyboard keybind entry.
        /// The displayed name and key will come from
        /// SettingsKeybindManager.
        /// </summary>
        public BindingEntry(
            SettingsKeybindAction keybindAction,
            string statusText = "")
        {
            isInteractiveKeybind = true;

            this.keybindAction = keybindAction;
            this.statusText = statusText;

            actionName = "";
            bindingText = "";
        }

        /// <summary>
        /// Creates a non-interactive display-only entry.
        /// This can still be used for placeholder pages.
        /// </summary>
        public BindingEntry(
            string actionName,
            string bindingText,
            string statusText = "")
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

        public BindingSection(
            string sectionTitle,
            List<BindingEntry> entries)
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
    private List<BindingSection> sections =
        new List<BindingSection>();

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
        if (contentRoot == null)
        {
            Debug.LogWarning(
                name + ": Content Root has not been assigned.");
            return;
        }

        if (sectionHeaderPrefab == null)
        {
            Debug.LogWarning(
                name + ": Section Header Prefab has not been assigned.");
            return;
        }

        if (rowPrefab == null)
        {
            Debug.LogWarning(
                name + ": Row Prefab has not been assigned.");
            return;
        }

        ClearDisplay();

        foreach (BindingSection section in sections)
        {
            if (section == null)
                continue;

            GameObject headerObject = Instantiate(sectionHeaderPrefab, contentRoot);

            headerObject.name = "Section - " + section.sectionTitle;

            TMP_Text headerText = headerObject.GetComponentInChildren<TMP_Text>(true);

            if (headerText != null)
                headerText.text = section.sectionTitle;

            if (section.entries == null)
                continue;

            foreach (BindingEntry entry in section.entries)
            {
                if (entry == null)
                    continue;

                SettingsBindingDisplayRow newRow = Instantiate(rowPrefab, contentRoot);

                if (entry.isInteractiveKeybind)
                {
                    SettingsKeybindRow keybindRow =
                        newRow.GetComponent<SettingsKeybindRow>();

                    if (keybindRow != null)
                    {
                        keybindRow.Initialize(entry.keybindAction);
                    }
                    else
                    {
                        Debug.LogWarning(
                            newRow.name +
                            " is an interactive keybind row, but its prefab " +
                            "does not contain SettingsKeybindRow.");
                    }
                }
                else
                {
                    // Display-only row, such as a placeholder setting.
                    newRow.SetDisplay(
                        entry.actionName,
                        entry.bindingText,
                        entry.statusText);
                }
            }
        }

        hasBuilt = true;

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentRoot);

        if (scrollRect != null)
            scrollRect.verticalNormalizedPosition = 1f;

        SettingsMenuNavigator navigator = GetComponentInParent<SettingsMenuNavigator>();

        if (navigator != null)
        {
            navigator.RefreshGeneratedOptions();
        }
        else
        {
            Debug.LogWarning(name + ": No SettingsMenuNavigator was found in the parent hierarchy.");
        }
    }

    [ContextMenu("Clear Display")]
    public void ClearDisplay()
    {
        if (contentRoot == null)
            return;

        for (int i = contentRoot.childCount - 1; i >= 0; i--)
        {
            GameObject child =
                contentRoot.GetChild(i).gameObject;

            // Immediately remove it from the layout.
            child.SetActive(false);

            if (Application.isPlaying)
                Destroy(child);
            else
                DestroyImmediate(child);
        }

        hasBuilt = false;
    }

    [ContextMenu("Load Game Defaults")]
    public void LoadGameDefaults()
    {
        sections = new List<BindingSection>
        {
            new BindingSection("Game", new List<BindingEntry>
                {
                    new BindingEntry("Move Forward", "W"),
                    new BindingEntry("Move Backward", "S"),
                    new BindingEntry("Move Left", "A"),
                    new BindingEntry("Move Right", "D"),
                    new BindingEntry("Dodge", "Space"),
                    new BindingEntry("Sprint", "Shift"),
                    new BindingEntry("Walk", "Alt")
                }),

            new BindingSection("Actions",new List<BindingEntry>
                {
                    new BindingEntry("Heal", "Q"),
                    new BindingEntry("Use Item", "G"),
                    new BindingEntry("Remembrance", "R"),
                    new BindingEntry("Vestige", "E"),
                    new BindingEntry("Interact", "F")
                }),

            new BindingSection("Interface",new List<BindingEntry>
                {
                    new BindingEntry("Menu", "Esc"),
                    new BindingEntry("HUD", "Tab")
                })
        };

        hasBuilt = false;
    }

    [ContextMenu("Load Keyboard Defaults")]
    public void LoadKeyboardDefaults()
    {
        sections = new List<BindingSection>
    {
        new BindingSection(
            "Movement",
            new List<BindingEntry>
            {
                new BindingEntry(
                    SettingsKeybindAction.MoveForwards),

                new BindingEntry(
                    SettingsKeybindAction.MoveBackwards),

                new BindingEntry(
                    SettingsKeybindAction.MoveLeft),

                new BindingEntry(
                    SettingsKeybindAction.MoveRight),

                new BindingEntry(
                    SettingsKeybindAction.Dodge),

                new BindingEntry(
                    SettingsKeybindAction.Sprint),

                new BindingEntry(
                    SettingsKeybindAction.Walk)
            }),

        new BindingSection(
            "Actions",
            new List<BindingEntry>
            {
                new BindingEntry(
                    SettingsKeybindAction.Heal),

                new BindingEntry(
                    SettingsKeybindAction.UseItem),

                new BindingEntry(
                    SettingsKeybindAction.Remembrance),

                new BindingEntry(
                    SettingsKeybindAction.Vestige),

                new BindingEntry(
                    SettingsKeybindAction.Interact)
            }),

        new BindingSection("Interface", new List<BindingEntry>
            {
                new BindingEntry(SettingsKeybindAction.Menu), 
                new BindingEntry(SettingsKeybindAction.HUD)
            })
        };

        hasBuilt = false;
    }

    [ContextMenu("Load Controller Defaults")]
    public void LoadControllerDefaults()
    {
        sections = new List<BindingSection>
        {
            new BindingSection("Controller", new List<BindingEntry>
                {
                    new BindingEntry("Move Forward", "W"),
                    new BindingEntry("Move Backward", "S"),
                    new BindingEntry("Move Left", "A"),
                    new BindingEntry("Move Right", "D"),
                    new BindingEntry("Dodge", "Space"),
                    new BindingEntry("Sprint", "Shift"),
                    new BindingEntry("Walk", "Alt")
                }),

            new BindingSection("Actions", new List<BindingEntry>
                {
                    new BindingEntry("Heal", "Q"),
                    new BindingEntry("Use Item", "G"),
                    new BindingEntry("Remembrance", "R"),
                    new BindingEntry("Vestige", "E"),
                    new BindingEntry("Interact", "F")
                }),

            new BindingSection("Interface",new List<BindingEntry>
                {
                    new BindingEntry("Menu", "Esc"),
                    new BindingEntry("HUD", "Tab")
                })
        };

        hasBuilt = false;
    }

    [ContextMenu("Load Display Defaults")]
    public void LoadDisplayDefaults()
    {
        sections = new List<BindingSection>
        {
            new BindingSection("Display", new List<BindingEntry>
                {
                    new BindingEntry("Move Forward", "W"),
                    new BindingEntry("Move Backward", "S"),
                    new BindingEntry("Move Left", "A"),
                    new BindingEntry("Move Right", "D"),
                    new BindingEntry("Dodge", "Space"),
                    new BindingEntry("Sprint", "Shift"),
                    new BindingEntry("Walk", "Alt")
                }),

            new BindingSection("Actions", new List<BindingEntry>
                {
                    new BindingEntry("Heal", "Q"),
                    new BindingEntry("Use Item", "G"),
                    new BindingEntry("Remembrance", "R"),
                    new BindingEntry("Vestige", "E"),
                    new BindingEntry("Interact", "F")
                }),

            new BindingSection("Interface", new List<BindingEntry>
                {
                    new BindingEntry("Menu", "Esc"),
                    new BindingEntry("HUD", "Tab")
                })
        };

        hasBuilt = false;
    }

    [ContextMenu("Load Sound Defaults")]
    public void LoadSoundDefaults()
    {
        sections = new List<BindingSection>
        {
            new BindingSection("Sound", new List<BindingEntry>
                {
                    new BindingEntry("Move Forward", "W"),
                    new BindingEntry("Move Backward", "S"),
                    new BindingEntry("Move Left", "A"),
                    new BindingEntry("Move Right", "D"),
                    new BindingEntry("Dodge", "Space"),
                    new BindingEntry("Sprint", "Shift"),
                    new BindingEntry("Walk", "Alt")
                }),

            new BindingSection("Actions", new List<BindingEntry>
                {
                    new BindingEntry("Heal", "Q"),
                    new BindingEntry("Use Item", "G"),
                    new BindingEntry("Remembrance", "R"),
                    new BindingEntry("Vestige", "E"),
                    new BindingEntry("Interact", "F")
                }),

            new BindingSection("Interface", new List<BindingEntry>
                {
                    new BindingEntry("Menu", "Esc"),
                    new BindingEntry("HUD", "Tab")
                })
        };

        hasBuilt = false;
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
}