using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingsKeybindRow : SettingsMenuSelectable
{
    [Header("Text")]
    [SerializeField] private TMP_Text actionNameText;

    [SerializeField] private TMP_Text keyText;

    [SerializeField] private TMP_Text statusText;

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

    [Header("Clickable Key Field")]
    [SerializeField] private RectTransform valueArea;

    private SettingsKeybindAction action;
    private bool hasBeenInitialized;

    private SettingsKeybindManager keybindManager;
    private SettingsMenuNavigator menuNavigator;

    private GameObject highlightInstance;
    private CanvasGroup highlightCanvasGroup;

    private bool rowSelected;
    private bool isListening;
    private bool interactionLocked;

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

        // Generated prefab instances reach OnEnable before
        // SettingsBindingPageDisplay calls Initialize().

        // Do not register or refresh until this particular
        // instance has received its action.
        if (!hasBeenInitialized)
            return;

        if (keybindManager != null)
        {
            keybindManager.RegisterRow(this);
        }

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
        if (!isListening ||
            highlightInstance == null ||
            highlightCanvasGroup == null)
        {
            return;
        }

        float sine =
            Mathf.Sin(
                Time.unscaledTime *
                blinkCyclesPerSecond *
                Mathf.PI *
                2f
            );

        float normalizedSine = (sine + 1f) * 0.5f;

        highlightCanvasGroup.alpha =
            Mathf.Lerp(
                minimumBlinkAlpha,
                maximumBlinkAlpha,
                normalizedSine
            );
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        FindReferences();

        if (menuNavigator == null)
        {
            Debug.LogWarning(name + ": SettingsMenuNavigator was not found.");

            return;
        }

        if (!hasBeenInitialized)
        {
            Debug.LogWarning(name + ": This keybind row was clicked before Initialize() " + "assigned its action.");

            return;
        }

        if (menuNavigator.IsListeningForBinding)
            return;
        
        menuNavigator.SelectOptionByMouse(this);

        if (valueArea == null)
        {
            Debug.LogWarning(name + ": Value Area has not been assigned.");

            return;
        }

        bool clickedValueArea = RectTransformUtility.RectangleContainsScreenPoint
            (
                valueArea,
                eventData.position,
                eventData.pressEventCamera
            );

        if (!clickedValueArea)
            return;

        Debug.Log("Beginning listening for: " + action);

        menuNavigator.StartKeybindListen(this);
    }

    public void Initialize(SettingsKeybindAction assignedAction)
    {
        FindReferences();

        // Remove any previous registration if this row
        // is being reinitialized.
        if (hasBeenInitialized && keybindManager != null)
        {
            keybindManager.UnregisterRow(this);
        }

        action = assignedAction;
        hasBeenInitialized = true;

        if (isActiveAndEnabled && keybindManager != null)
        {
            keybindManager.RegisterRow(this);
        }

        RefreshDisplay();
    }

    private void FindReferences()
    {
        if (keybindManager == null)
        {
            keybindManager = GetComponentInParent<SettingsKeybindManager>();
        }

        if (menuNavigator == null)
        {
            menuNavigator = GetComponentInParent<SettingsMenuNavigator>();
        }
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

        // Prevent the highlight from blocking clicks.
        Graphic[] graphics = highlightInstance.GetComponentsInChildren<Graphic>(true);

        foreach (Graphic graphic in graphics)
        {
            graphic.raycastTarget = false;
        }

        highlightCanvasGroup = highlightInstance.GetComponent<CanvasGroup>();

        if (highlightCanvasGroup == null)
        {
            highlightCanvasGroup = highlightInstance.AddComponent<CanvasGroup>();
        }

        highlightInstance.SetActive(false);
    }

    public override void OnPointerEnter(
    PointerEventData eventData)
    {
        FindReferences();

        if (interactionLocked)
            return;

        if (menuNavigator != null && menuNavigator.IsListeningForBinding)
            return;

        // Mouse hover becomes the menu's one official selection.
        if (menuNavigator != null)
            menuNavigator.SelectOptionByMouse(this);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {

        // The navigator owns which row is selected. Entering another row will automatically deselect this one.
    }

    /// <summary>
    /// Connect this method only to the ValueArea Button.
    /// </summary>
    public void BeginListeningFromValueField()
    {
        if (!hasBeenInitialized)
            return;

        FindReferences();

        if (menuNavigator == null)
        {
            Debug.LogWarning(name + ": SettingsMenuNavigator was not found.");
            return;
        }

        if (menuNavigator.IsListeningForBinding)
            return;

        Debug.Log("Clicked key field for: " + action);

        menuNavigator.SelectOptionByMouse(this);
        menuNavigator.StartKeybindListen(this);
    }

    public override void SetSelected(bool selected)
    {
        rowSelected = selected;
        RefreshHighlight();
    }

    public override void Activate()
    {
        // Intentionally empty.

        // Selecting the row or pressing Enter/Space on it. Does not begin rebinding.

        // Only the ValueArea Button calls BeginListeningFromValueField().
    }

    public void SetListening(bool listening)
    {
        isListening = listening;

        if (isListening)
        {
            rowSelected = true;

            if (statusText != null)
                statusText.text = "Press a key or mouse button";
        }
        else
        {
            if (statusText != null)
                statusText.text = "";

            if (highlightCanvasGroup != null)
                highlightCanvasGroup.alpha = 1f;
        }

        RefreshHighlight();
    }

    public void SetInteractionLocked(bool locked)
    {
        interactionLocked = locked;

        if (interactionLocked && !isListening)
            rowSelected = false;

        RefreshHighlight();
    }

    public void ApplyBinding(KeyCode newKey)
    {
        FindReferences();

        if (!hasBeenInitialized)
        {
            Debug.LogWarning(name + ": Cannot apply a binding before initialization.");

            return;
        }

        if (keybindManager == null)
        {
            Debug.LogWarning(name + ": SettingsKeybindManager was not found.");

            return;
        }

        keybindManager.SetBinding(action, newKey);

        RefreshDisplay();
    }

    public void RefreshDisplay()
    {
        if (!hasBeenInitialized)
            return;

        if (keybindManager == null)
            FindReferences();

        if (keybindManager == null)
            return;

        if (actionNameText != null)
        {
            actionNameText.text =
                keybindManager.GetActionDisplayName(action);
        }

        if (keyText != null)
        {
            KeyCode currentKey =
                keybindManager.GetBinding(action);

            keyText.text =
                keybindManager.GetKeyDisplayName(currentKey);
        }
    }

    private void RefreshHighlight()
    {
        if (highlightInstance == null)
            return;

        bool shouldShow = isListening || (!interactionLocked && rowSelected);

        highlightInstance.SetActive(shouldShow);

        // A selected row is steady. Only a listening row blinks.
        if (shouldShow && !isListening && highlightCanvasGroup != null)
            highlightCanvasGroup.alpha = 1f;
    }
}