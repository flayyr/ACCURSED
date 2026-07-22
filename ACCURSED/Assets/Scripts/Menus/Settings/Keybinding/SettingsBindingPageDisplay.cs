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
        /// Creates an interactive keyboard keybind entry.
        /// The displayed name and key will come from
        /// SettingsKeybindManager.
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
        /// Creates a non-interactive display-only entry.
        /// This can still be used for placeholder pages.
        /// </summary>
        public BindingEntry(SettingsKeybindAction keybindAction, string actionName, string statusText = "")
        {
            isInteractiveKeybind = true;

            this.keybindAction = keybindAction;
            this.actionName = actionName;
            this.bindingText = "";
            this.statusText = statusText;
        }

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
            Debug.LogWarning(name + ": Content Root has not been assigned.");
            return;
        }

        if (sectionHeaderPrefab == null)
        {
            Debug.LogWarning( name + ": Section Header Prefab has not been assigned.");
            return;
        }

        if (rowPrefab == null)
        {
            Debug.LogWarning(name + ": Row Prefab has not been assigned.");
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

                Debug.Log("Building entry: " + entry.actionName + " | Interactive: " 
                    + entry.isInteractiveKeybind + " | Action: " + entry.keybindAction
                );

                SettingsBindingDisplayRow newRow =
                    Instantiate(rowPrefab, contentRoot);

                newRow.gameObject.name =
                    "Row - " + entry.actionName;

                // Set the initial text for every kind of row.
                newRow.SetDisplay(entry.actionName, entry.bindingText, entry.statusText
                );

                if (!entry.isInteractiveKeybind)
                {
                    Debug.Log( newRow.gameObject.name + " was created as a static display row.", newRow.gameObject
                    );

                    continue;
                }

                // First check the same GameObject.
                SettingsKeybindRow keybindRow = newRow.GetComponent<SettingsKeybindRow>();

                // Also support SettingsKeybindRow being on a child.
                if (keybindRow == null)
                {
                    keybindRow = newRow.GetComponentInChildren <SettingsKeybindRow>(true);
                }

                if (keybindRow == null)
                {
                    Debug.LogWarning(newRow.gameObject.name + ": This entry is marked as an interactive keybind, " 
                        + "but SettingsKeybindRow was not found on the row prefab.", newRow.gameObject
                    );

                    continue;
                }

                keybindRow.Initialize(entry.keybindAction);

                Debug.Log("Initialized " + newRow.gameObject.name + " as " + entry.keybindAction 
                    + " | Instance ID: " + keybindRow.GetInstanceID(), keybindRow
                );
            }
        }

        hasBuilt = true;

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentRoot);

        if (scrollRect != null)
            scrollRect.verticalNormalizedPosition = 1f;

        StartCoroutine(RefreshNavigatorNextFrame());
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
            new BindingSection("Game", new List<BindingEntry>
                {
                    new BindingEntry(SettingsKeybindAction.MoveForwards,"Move Forward", "W"),
                    new BindingEntry(SettingsKeybindAction.MoveBackwards,"Move Backward", "S"),
                    new BindingEntry(SettingsKeybindAction.MoveLeft,"Move Left", "A"),
                    new BindingEntry(SettingsKeybindAction.MoveRight,"Move Right", "D"),
                    new BindingEntry(SettingsKeybindAction.Dodge,"Dodge", "Space"),
                    new BindingEntry(SettingsKeybindAction.Sprint,"Sprint", "Shift"),
                    new BindingEntry(SettingsKeybindAction.Walk,"Walk", "Alt")
                }),

            new BindingSection("Actions",new List<BindingEntry>
                {
                    new BindingEntry(SettingsKeybindAction.Heal,"Heal", "Q"),
                    new BindingEntry(SettingsKeybindAction.UseItem,"Use Item", "G"),
                    new BindingEntry(SettingsKeybindAction.Remembrance,"Remembrance", "R"),
                    new BindingEntry(SettingsKeybindAction.Vestige,"Vestige", "E"),
                    new BindingEntry(SettingsKeybindAction.Interact,"Interact", "F")
                }),

            new BindingSection("Interface",new List<BindingEntry>
                {
                    new BindingEntry(SettingsKeybindAction.Menu,"Menu", "Esc"),
                    new BindingEntry(SettingsKeybindAction.HUD,"HUD", "Tab")
                })
        };

        hasBuilt = false;

        if (Application.isPlaying && isActiveAndEnabled)
            BuildDisplay();
    }

    [ContextMenu("Load Controller Defaults")]
    public void LoadControllerDefaults()
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

    [ContextMenu("Load Display Defaults")]
    public void LoadDisplayDefaults()
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

    [ContextMenu("Load Sound Defaults")]
    public void LoadSoundDefaults()
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