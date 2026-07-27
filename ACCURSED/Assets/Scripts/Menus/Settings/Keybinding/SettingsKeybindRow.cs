using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingsKeybindRow : SettingsMenuSelectable
{
    [Header("Text")]
    [SerializeField] private TMP_Text actionNameText;
    [SerializeField] private TMP_Text statusText;

    [Header("Single-Key Display")]
    [Tooltip("Usually the existing full-width ValueArea object.")]
    [SerializeField] private GameObject singleValueRoot;

    [Tooltip("The existing full-width clickable value area.")]
    [SerializeField] private RectTransform valueArea;

    [Tooltip("The existing key text inside ValueArea.")]
    [SerializeField] private TMP_Text keyText;

    [Header("Chord Display")]
    [Tooltip("Parent containing the two smaller value areas and the + sign.")]
    [SerializeField] private GameObject chordValueRoot;

    [SerializeField] private RectTransform modifierValueArea;
    [SerializeField] private TMP_Text modifierKeyText;

    [SerializeField] private RectTransform triggerValueArea;
    [SerializeField] private TMP_Text triggerKeyText;

    [Header("Highlight")]
    [Tooltip("The Background RectTransform of this row.")]
    [SerializeField] private RectTransform rowBackground;

    [SerializeField] private GameObject highlightPrefab;

    [Header("Blinking")]
    [Tooltip("Approximately 0.5 to 1 creates a slow blink.")]
    [SerializeField] private float blinkCyclesPerSecond = 0.7f;

    [Range(0f, 1f)]
    [SerializeField] private float minimumBlinkAlpha = 0.2f;

    [Range(0f, 1f)]
    [SerializeField] private float maximumBlinkAlpha = 0.85f;

    private SettingsKeybindAction action;
    private bool hasBeenInitialized;

    private SettingsKeybindManager keybindManager;
    private SettingsMenuNavigator menuNavigator;

    private GameObject highlightInstance;
    private CanvasGroup highlightCanvasGroup;

    private bool rowSelected;
    private bool isListening;
    private bool interactionLocked;

    private SettingsKeybindSlot listeningSlot = SettingsKeybindSlot.Trigger;

    public SettingsKeybindAction Action
    {
        get { return action; }
    }

    private void Awake()
    {
        FindReferences();
        CreateHighlight();
    }

    private void OnEnable()
    {
        FindReferences();

        // Generated instances receive OnEnable before Initialize().
        if (!hasBeenInitialized)
            return;

        if (keybindManager != null)
            keybindManager.RegisterRow(this);

        RefreshDisplay();
        RefreshHighlight();
    }

    protected override void OnDisable()
    {
        if (keybindManager != null)
            keybindManager.UnregisterRow(this);

        if (isListening && menuNavigator != null)
            menuNavigator.CancelKeybindListen(this);

        base.OnDisable();
    }

    private void Update()
    {
        if (!isListening || highlightInstance == null || highlightCanvasGroup == null)
            return;

        float sine = Mathf.Sin(Time.unscaledTime * blinkCyclesPerSecond * Mathf.PI * 2f);

        float normalizedSine = (sine + 1f) * 0.5f;

        highlightCanvasGroup.alpha = Mathf.Lerp(minimumBlinkAlpha, maximumBlinkAlpha, normalizedSine);
    }

    public void Initialize(SettingsKeybindAction assignedAction)
    {
        FindReferences();

        if (hasBeenInitialized && keybindManager != null)
            keybindManager.UnregisterRow(this);

        action = assignedAction;
        hasBeenInitialized = true;

        if (isActiveAndEnabled && keybindManager != null)
            keybindManager.RegisterRow(this);

        RefreshDisplay();
    }

    private void FindReferences()
    {
        if (keybindManager == null)
            keybindManager = GetComponentInParent<SettingsKeybindManager>();

        if (menuNavigator == null)
            menuNavigator = GetComponentInParent<SettingsMenuNavigator>();
    }

    private void CreateHighlight()
    {
        if (highlightPrefab == null || rowBackground == null || highlightInstance != null)
            return;

        highlightInstance = Instantiate(highlightPrefab, rowBackground);

        RectTransform highlightRect = highlightInstance.GetComponent<RectTransform>();

        if (highlightRect != null)
        {
            highlightRect.anchorMin = Vector2.zero;
            highlightRect.anchorMax = Vector2.one;

            highlightRect.offsetMin = Vector2.zero;
            highlightRect.offsetMax = Vector2.zero;

            highlightRect.pivot = new Vector2(0.5f, 0.5f);

            highlightRect.localScale = Vector3.one;
            highlightRect.localRotation = Quaternion.identity;
        }

        Graphic[] graphics = highlightInstance.GetComponentsInChildren<Graphic>(true);

        foreach (Graphic graphic in graphics)
            graphic.raycastTarget = false;

        highlightCanvasGroup = highlightInstance.GetComponent<CanvasGroup>();

        if (highlightCanvasGroup == null)
            highlightCanvasGroup = highlightInstance.AddComponent<CanvasGroup>();

        highlightInstance.SetActive(false);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        FindReferences();

        if (interactionLocked)
            return;

        if (menuNavigator != null && menuNavigator.IsListeningForBinding)
            return;

        if (menuNavigator != null)
            menuNavigator.SelectOptionByMouse(this);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        FindReferences();

        if (!CanBeginListening())
            return;

        menuNavigator.SelectOptionByMouse(this);

        SettingsKeybind binding = keybindManager.GetBinding(action);

        if (binding.IsChord)
        {
            if (ContainsPoint(modifierValueArea, eventData))
            {
                BeginListening(SettingsKeybindSlot.Modifier);
                return;
            }

            if (ContainsPoint(triggerValueArea, eventData))
                BeginListening(SettingsKeybindSlot.Trigger);

            return;
        }

        if (ContainsPoint(valueArea, eventData))
            BeginListening(SettingsKeybindSlot.Trigger);
    }

    public override void OnPointerExit(
        PointerEventData eventData)
    {
        // Selection is owned by SettingsMenuNavigator.
    }

    /// <summary>
    /// Connect this to the existing single ValueArea Button.
    /// </summary>
    public void BeginListeningFromValueField()
    {
        BeginListening(SettingsKeybindSlot.Trigger);
    }

    /// <summary>
    /// Connect this to the left/smaller chord Button.
    /// </summary>
    public void BeginListeningFromModifierField()
    {
        BeginListening(SettingsKeybindSlot.Modifier);
    }

    /// <summary>
    /// Connect this to the right/smaller chord Button.
    /// </summary>
    public void BeginListeningFromTriggerField()
    {
        BeginListening(SettingsKeybindSlot.Trigger);
    }

    private void BeginListening( SettingsKeybindSlot slot)
    {
        FindReferences();

        if (!CanBeginListening())
            return;

        menuNavigator.SelectOptionByMouse(this);
        menuNavigator.StartKeybindListen(this, slot);
    }

    private bool CanBeginListening()
    {
        if (!hasBeenInitialized || interactionLocked)
            return false;

        if (keybindManager == null || menuNavigator == null)
        {
            Debug.LogWarning(name + ": Keybind manager or navigator was not found.");

            return false;
        }

        return !menuNavigator.IsListeningForBinding;
    }

    private static bool ContainsPoint(RectTransform area, PointerEventData eventData)
    {
        return area != null && RectTransformUtility.RectangleContainsScreenPoint(area, eventData.position, eventData.pressEventCamera);
    }

    public override void SetSelected(bool selected)
    {
        rowSelected = selected;
        RefreshHighlight();
    }

    public override void Activate()
    {
        // Intentionally empty.
        // Selecting the row or pressing Enter/Space does not start rebinding.
        // Only clicking a specific value field starts listening.
    }

    public void SetListening(bool listening)
    {
        SetListening(listening, SettingsKeybindSlot.Trigger);
    }

    public void SetListening(bool listening, SettingsKeybindSlot slot)
    {
        isListening = listening;
        listeningSlot = slot;

        if (isListening)
        {
            rowSelected = true;

            if (statusText != null)
            {
                statusText.text = slot == SettingsKeybindSlot.Modifier
                    ? "Press the first key"
                    : "Press a key or mouse button";
            }
        }
        else
        {
            if (statusText != null)
                statusText.text = "";

            if (highlightCanvasGroup != null)
                highlightCanvasGroup.alpha = 1f;
        }

        RefreshDisplay();
        RefreshHighlight();
    }

    public void SetInteractionLocked(bool locked)
    {
        interactionLocked = locked;

        if (interactionLocked && !isListening)
            rowSelected = false;

        RefreshHighlight();
    }

    public void ApplyBinding(SettingsKeybindSlot slot, KeyCode newKey)
    {
        FindReferences();

        if (!hasBeenInitialized || keybindManager == null)
        {
            Debug.LogWarning(name + ": Cannot apply a binding before initialization.");

            return;
        }

        keybindManager.SetBinding(action, slot, newKey);
    }

    /// <summary>
    /// Compatibility overload for the original single-key navigator.
    /// </summary>
    public void ApplyBinding(KeyCode newKey)
    {
        ApplyBinding(SettingsKeybindSlot.Trigger, newKey);
    }

    public void RefreshDisplay()
    {
        if (!hasBeenInitialized)
            return;

        if (keybindManager == null)
            FindReferences();

        if (keybindManager == null)
            return;

        SettingsKeybind binding = keybindManager.GetBinding(action);

        if (actionNameText != null)
            actionNameText.text = keybindManager.GetActionDisplayName(action);

        SetRootActive(singleValueRoot, valueArea, !binding.IsChord);

        if (chordValueRoot != null)
            chordValueRoot.SetActive(binding.IsChord);

        if (keyText != null)
        {
            keyText.text = isListening && listeningSlot == SettingsKeybindSlot.Trigger
                ? "..."
                : keybindManager.GetKeyDisplayName(binding.Trigger);
        }

        if (modifierKeyText != null)
        {
            modifierKeyText.text = isListening && listeningSlot == SettingsKeybindSlot.Modifier
                ? "..."
                : keybindManager.GetKeyDisplayName(binding.Modifier);
        }

        if (triggerKeyText != null)
        {
            triggerKeyText.text = isListening && listeningSlot == SettingsKeybindSlot.Trigger
                ? "..."
                : keybindManager.GetKeyDisplayName(binding.Trigger);
        }
    }

    private static void SetRootActive(GameObject explicitRoot, RectTransform fallbackArea, bool active)
    {
        if (explicitRoot != null)
        {
            explicitRoot.SetActive(active);
            return;
        }

        if (fallbackArea != null)
            fallbackArea.gameObject.SetActive(active);
    }

    private void RefreshHighlight()
    {
        if (highlightInstance == null)
            return;

        bool shouldShow = isListening || (!interactionLocked && rowSelected);

        highlightInstance.SetActive(shouldShow);

        if (shouldShow && !isListening && highlightCanvasGroup != null)
            highlightCanvasGroup.alpha = 1f;
    }
}
