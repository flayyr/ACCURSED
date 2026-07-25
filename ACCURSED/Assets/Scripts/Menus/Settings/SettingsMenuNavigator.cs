using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuNavigator : MonoBehaviour
{
    private enum NavigationArea
    {
        Tabs,
        Options
    }

    [Header("Tabs")]
    public List<SettingsTabButton> tabs = new List<SettingsTabButton>();

    [Header("Scroll View")]
    public ScrollRect scrollRect;
    public RectTransform scrollContent;
    public RectTransform scrollViewport;
    public float scrollPadding = 30f;

    [Header("Input")]
    public float sliderKeyboardSpeed = 0.5f;

    private static readonly KeyCode[] BindableKeys = CreateBindableKeyCache();

    private readonly List<SettingsMenuSelectable> currentOptions = new List<SettingsMenuSelectable>();

    private int selectedTabIndex;
    private int selectedOptionIndex;

    private NavigationArea currentArea = NavigationArea.Tabs;

    private SettingsSliderSelectable activeSlider;
    private bool isAdjustingSlider;

    private SettingsKeybindRow activeKeybindRow;
    private SettingsKeybindSlot activeKeybindSlot = SettingsKeybindSlot.Trigger;

    private bool isListeningForBinding;
    private int bindingStartFrame;

    public bool IsListeningForBinding
    {
        get { return isListeningForBinding; }
    }

    public bool IsAdjustingSlider
    {
        get { return isAdjustingSlider; }
    }

    private void Start()
    {
        RegisterAllSelectables();

        if (tabs.Count == 0)
            return;

        SelectTab(0, true);
        UseScrollViewFromTab(tabs[selectedTabIndex]);
        currentArea = NavigationArea.Tabs;
    }

    private void Update()
    {
        if (isListeningForBinding)
        {
            HandleKeybindListeningInput();
            return;
        }

        if (isAdjustingSlider)
        {
            HandleSliderAdjustingInput();
            return;
        }

        HandleNormalNavigationInput();
        HandleConfirmInput();
    }

    private void RegisterAllSelectables()
    {
        foreach (SettingsTabButton tab in tabs)
        {
            if (tab != null)
                tab.SetNavigator(this);
        }

        SettingsMenuSelectable[] allSelectables = FindObjectsByType<SettingsMenuSelectable>
            (FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (SettingsMenuSelectable selectable in allSelectables) selectable.SetNavigator(this);
    }

    private void HandleNormalNavigationInput()
    {
        if (currentArea == NavigationArea.Tabs)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                SelectTab(selectedTabIndex - 1, true);

            if (Input.GetKeyDown(KeyCode.RightArrow))
                SelectTab(selectedTabIndex + 1, true);

            if (Input.GetKeyDown(KeyCode.DownArrow) )
                MoveFromTabsToOptions();

            return;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
            SelectOption(selectedOptionIndex + 1);

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (selectedOptionIndex <= 0)
            {
                MoveFromOptionsToTabs();
            }
            else
            {
                SelectOption(selectedOptionIndex - 1);
            }
        }
    }

    private void HandleConfirmInput()
    {
        if (!Input.GetKeyDown(KeyCode.Return) && !Input.GetKeyDown(KeyCode.Space))
            return;

        if (currentArea == NavigationArea.Tabs)
        {
            SelectTab(selectedTabIndex, true);
            return;
        }

        if (currentArea == NavigationArea.Options && currentOptions.Count > 0)
            currentOptions[selectedOptionIndex].Activate();
    }

    private void HandleSliderAdjustingInput()
    {
        if (activeSlider == null)
        {
            isAdjustingSlider = false;
            return;
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            StopSliderAdjustMode();
            return;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
            activeSlider.AdjustSlider(-sliderKeyboardSpeed * Time.unscaledDeltaTime);

        if (Input.GetKey(KeyCode.RightArrow))
            activeSlider.AdjustSlider(sliderKeyboardSpeed * Time.unscaledDeltaTime);
    }

    /// <summary>
    /// Compatibility overload for existing single-key rows.
    /// </summary>
    public void StartKeybindListen(SettingsKeybindRow row)
    {
        StartKeybindListen(row, SettingsKeybindSlot.Trigger);
    }

    public void StartKeybindListen(SettingsKeybindRow row, SettingsKeybindSlot slot)
    {
        if (row == null)
        {
            Debug.LogWarning("StartKeybindListen received a null row.");
            return;
        }

        if (isListeningForBinding || isAdjustingSlider)
            return;

        // Select before enabling the listening lock.
        SelectOptionByMouse(row);

        activeKeybindRow = row;
        activeKeybindSlot = slot;

        isListeningForBinding = true;
        bindingStartFrame = Time.frameCount;

        for (int i = 0; i < currentOptions.Count; i++)
        {
            SettingsMenuSelectable option = currentOptions[i];

            if (option == null)
                continue;

            bool isActiveRow = option == row;
            option.SetSelected(isActiveRow);

            SettingsKeybindRow keybindRow = option as SettingsKeybindRow;

            if (keybindRow == null)
                continue;

            keybindRow.SetInteractionLocked(!isActiveRow);

            if (isActiveRow)
            {
                keybindRow.SetListening(true, slot);
            }
            else
            {
                keybindRow.SetListening(false);
            }
        }
    }

    private void HandleKeybindListeningInput()
    {
        if (activeKeybindRow == null)
        {
            CancelKeybindListen();
            return;
        }

        // Prevent the click used to open a field from
        // immediately becoming that field's new binding.
        if (Time.frameCount <= bindingStartFrame + 1)
            return;

        for (int i = 0; i < BindableKeys.Length; i++)
        {
            KeyCode key = BindableKeys[i];

            if (!Input.GetKeyDown(key))
                continue;

            FinishKeybindListen(key);
            return;
        }
    }

    private void FinishKeybindListen(KeyCode newKey)
    {
        SettingsKeybindRow finishedRow = activeKeybindRow;

        SettingsKeybindSlot finishedSlot = activeKeybindSlot;

        EndListeningState(finishedRow);

        if (finishedRow != null)
        {
            finishedRow.ApplyBinding(finishedSlot, newKey);

            finishedRow.RefreshDisplay();
        }
    }

    public void CancelKeybindListen(SettingsKeybindRow requestingRow = null)
    {
        if (requestingRow != null && requestingRow != activeKeybindRow)
            return;

        SettingsKeybindRow cancelledRow = activeKeybindRow;

        EndListeningState(cancelledRow);
    }

    private void EndListeningState(SettingsKeybindRow previouslyActiveRow)
    {
        activeKeybindRow = null;
        activeKeybindSlot = SettingsKeybindSlot.Trigger;
        isListeningForBinding = false;

        for (int i = 0; i < currentOptions.Count; i++)
        {
            SettingsMenuSelectable option = currentOptions[i];

            if (option == null)
                continue;

            option.SetSelected(option == previouslyActiveRow);

            SettingsKeybindRow keybindRow = option as SettingsKeybindRow;

            if (keybindRow == null)
                continue;

            keybindRow.SetInteractionLocked(false);
            keybindRow.SetListening(false);
        }
    }

    private static KeyCode[] CreateBindableKeyCache()
    {
        List<KeyCode> keys = new List<KeyCode>();

        foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
        {
            if (key == KeyCode.None)
                continue;

            if (key.ToString().StartsWith("Joystick"))
                continue;

            keys.Add(key);
        }

        return keys.ToArray();
    }

    public void SelectTab(int index, bool showTab)
    {
        if (tabs.Count == 0)
            return;

        if (index < 0)
            index = tabs.Count - 1;

        if (index >= tabs.Count)
            index = 0;

        DeselectCurrentOption();

        selectedTabIndex = index;
        currentArea = NavigationArea.Tabs;

        for (int i = 0; i < tabs.Count; i++)
            tabs[i].SetSelected(i == selectedTabIndex);

        if (!showTab)
            return;

        tabs[selectedTabIndex].ShowThisTab();
        UseScrollViewFromTab(tabs[selectedTabIndex]);
        RefreshCurrentOptions();

        if (scrollRect != null)
        {
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 1f;
        }
    }

    public void SelectTab(SettingsTabButton tab, bool showTab)
    {
        int index = tabs.IndexOf(tab);

        if (index >= 0)
            SelectTab(index, showTab);
    }

    private void MoveFromTabsToOptions()
    {
        StartCoroutine(MoveFromTabsToOptionsNextFrame());
    }

    private IEnumerator MoveFromTabsToOptionsNextFrame()
    {
        SelectTab(selectedTabIndex, true);
        yield return null;

        RefreshCurrentOptions();

        if (currentOptions.Count == 0)
        {
            Debug.LogWarning(name + ": No active settings rows were found " + "inside the selected tab panel.");
            yield break;
        }

        if (tabs[selectedTabIndex] != null)
            tabs[selectedTabIndex].SetSelected(false);

        currentArea = NavigationArea.Options;
        SelectOption(0);
    }

    private void MoveFromOptionsToTabs()
    {
        DeselectCurrentOption();
        currentArea = NavigationArea.Tabs;

        if (tabs.Count > 0)
            tabs[selectedTabIndex].SetSelected(true);
    }

    private void SelectOption(int index)
    {
        if (currentOptions.Count == 0)
            return;

        if (index < 0)
            index = currentOptions.Count - 1;

        if (index >= currentOptions.Count)
            index = 0;

        selectedOptionIndex = index;

        for (int i = 0; i < currentOptions.Count; i++)
        {
            currentOptions[i].SetSelected(i == selectedOptionIndex);
        }

        StartCoroutine(ScrollToSelectedOptionNextFrame());
    }

    public void SelectOptionByMouse(SettingsMenuSelectable option)
    {
        if (isListeningForBinding)
            return;

        RefreshCurrentOptions();

        int index = currentOptions.IndexOf(option);

        if (index < 0)
            return;

        tabs[selectedTabIndex].SetSelected(false);
        currentArea = NavigationArea.Options;
        SelectOption(index);
    }

    public void SelectTabByMouse(SettingsTabButton tab)
    {
        if (!isListeningForBinding)
            SelectTab(tab, true);
    }

    private void DeselectCurrentOption()
    {
        foreach (SettingsMenuSelectable option in currentOptions)
        {
            if (option != null)
                option.SetSelected(false);
        }
    }

    private void RefreshCurrentOptions()
    {
        currentOptions.Clear();

        if (tabs.Count == 0)
            return;

        GameObject activeTabPanel = tabs[selectedTabIndex].tabPanel;

        if (activeTabPanel == null)
            return;

        SettingsMenuSelectable[] selectables = activeTabPanel.GetComponentsInChildren <SettingsMenuSelectable>(true);

        foreach (SettingsMenuSelectable selectable in selectables)
        {
            if (selectable == null || selectable is SettingsTabButton || !selectable.gameObject.activeInHierarchy)
                continue;

            currentOptions.Add(selectable);
        }

        currentOptions.Sort(delegate ( SettingsMenuSelectable a, SettingsMenuSelectable b)
        {
            return a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex());
        });

        selectedOptionIndex = 0;
    }

    public void StartSliderAdjustMode(SettingsSliderSelectable slider)
    {
        activeSlider = slider;
        isAdjustingSlider = true;

        if (activeSlider != null)
            activeSlider.SetAdjusting(true);
    }

    public void StopSliderAdjustMode()
    {
        if (activeSlider != null)
            activeSlider.SetAdjusting(false);

        activeSlider = null;
        isAdjustingSlider = false;
    }

    private void ScrollToSelectedOption()
    {
        if (scrollRect == null || scrollContent == null || scrollViewport == null)
            return;

        if (currentArea != NavigationArea.Options || currentOptions.Count == 0)
            return;

        RectTransform selectedRect = currentOptions[selectedOptionIndex].GetComponent<RectTransform>();

        if (selectedRect == null)
            return;

        Canvas.ForceUpdateCanvases();

        float contentHeight = scrollContent.rect.height;

        float viewportHeight = scrollViewport.rect.height;

        if (contentHeight <= viewportHeight)
            return;

        Vector3 selectedWorldCenter = selectedRect.TransformPoint(selectedRect.rect.center);

        Vector3 viewportLocalCenter = scrollViewport.InverseTransformPoint(selectedWorldCenter);

        float selectedTop = viewportLocalCenter.y + selectedRect.rect.height * 0.5f;

        float selectedBottom = viewportLocalCenter.y - selectedRect.rect.height * 0.5f;

        float viewportTop = scrollViewport.rect.height * 0.5f;

        float viewportBottom = -scrollViewport.rect.height * 0.5f;

        float scrollAmount = 0f;

        if (selectedTop > viewportTop - scrollPadding)
        {
            scrollAmount = selectedTop - viewportTop + scrollPadding;
        }
        else if (selectedBottom < viewportBottom + scrollPadding)
        {
            scrollAmount = selectedBottom - viewportBottom - scrollPadding;
        }

        if (Mathf.Approximately(scrollAmount, 0f))
            return;

        float normalizedDelta = scrollAmount / (contentHeight - viewportHeight);

        scrollRect.verticalNormalizedPosition += normalizedDelta;

        scrollRect.verticalNormalizedPosition = Mathf.Clamp01(scrollRect.verticalNormalizedPosition);

        Canvas.ForceUpdateCanvases();
    }

    public void ResetNavigation()
    {
        CancelKeybindListen();
        StopSliderAdjustMode();
        RegisterAllSelectables();

        selectedTabIndex = 0;
        selectedOptionIndex = 0;
        currentArea = NavigationArea.Tabs;

        if (tabs.Count == 0)
            return;

        SelectTab(0, true);
        UseScrollViewFromTab(tabs[selectedTabIndex]);

        if (scrollRect != null)
        {
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 1f;
        }
    }

    private void UseScrollViewFromTab(SettingsTabButton tab)
    {
        if (tab == null)
            return;

        scrollRect = tab.tabScrollRect;
        scrollContent = tab.tabScrollContent;
        scrollViewport = tab.tabScrollViewport;
    }

    public void RefreshGeneratedOptions()
    {
        RefreshCurrentOptions();

        foreach (SettingsMenuSelectable option in currentOptions)
        {
            if (option != null)
                option.SetNavigator(this);
        }

        if (currentArea == NavigationArea.Options && currentOptions.Count > 0)
        {
            selectedOptionIndex = Mathf.Clamp(selectedOptionIndex, 0, currentOptions.Count - 1);

            SelectOption(selectedOptionIndex);
        }
    }

    private IEnumerator ScrollToSelectedOptionNextFrame()
    {
        yield return null;
        ScrollToSelectedOption();
    }
}
