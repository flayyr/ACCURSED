using System.Collections.Generic;
using UnityEngine;

public class SettingsMenuNavigator : MonoBehaviour
{
    private enum NavigationArea
    {
        Tabs,
        Options
    }

    [Header("Tabs")]
    public List<SettingsTabButton> tabs = new List<SettingsTabButton>();

    [Header("Input")]
    public float sliderKeyboardStep = 0.05f;

    private List<SettingsMenuSelectable> currentOptions = new List<SettingsMenuSelectable>();

    private int selectedTabIndex = 0;
    private int selectedOptionIndex = 0;

    private NavigationArea currentArea = NavigationArea.Tabs;

    private SettingsSliderSelectable activeSlider;
    private bool isAdjustingSlider;

    private void Start()
    {
        RegisterAllSelectables();

        if (tabs.Count > 0)
        {
            SelectTab(0, true);
            currentArea = NavigationArea.Tabs;
        }
    }

    private void Update()
    {
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

        SettingsMenuSelectable[] allSelectables = GetComponentsInChildren<SettingsMenuSelectable>(true);

        foreach (SettingsMenuSelectable selectable in allSelectables)
        {
            selectable.SetNavigator(this);
        }
    }

    private void HandleNormalNavigationInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            if (currentArea == NavigationArea.Tabs)
            {
                SelectTab(selectedTabIndex - 1, true);
            }
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            if (currentArea == NavigationArea.Tabs)
            {
                SelectTab(selectedTabIndex + 1, true);
            }
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            if (currentArea == NavigationArea.Tabs)
            {
                MoveFromTabsToOptions();
            }
            else
            {
                SelectOption(selectedOptionIndex + 1);
            }
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            if (currentArea == NavigationArea.Options)
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
    }

    private void HandleConfirmInput()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            if (currentArea == NavigationArea.Tabs)
            {
                tabs[selectedTabIndex].Activate();
            }
            else if (currentArea == NavigationArea.Options && currentOptions.Count > 0)
            {
                currentOptions[selectedOptionIndex].Activate();
            }
        }
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

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            activeSlider.AdjustSlider(-sliderKeyboardStep);
        }

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            activeSlider.AdjustSlider(sliderKeyboardStep);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StopSliderAdjustMode();
        }
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
        {
            tabs[i].SetSelected(i == selectedTabIndex);
        }

        if (showTab)
        {
            tabs[selectedTabIndex].ShowThisTab();
            RefreshCurrentOptions();
        }
    }

    public void SelectTab(SettingsTabButton tab, bool showTab)
    {
        int index = tabs.IndexOf(tab);

        if (index >= 0)
        {
            SelectTab(index, showTab);
        }
    }

    private void MoveFromTabsToOptions()
    {
        RefreshCurrentOptions();

        if (currentOptions.Count == 0)
            return;

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
    }

    public void SelectOptionByMouse(SettingsMenuSelectable option)
    {
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

        SettingsMenuSelectable[] selectables =
            activeTabPanel.GetComponentsInChildren<SettingsMenuSelectable>(true);

        foreach (SettingsMenuSelectable selectable in selectables)
        {
            if (selectable == null)
                continue;

            if (selectable is SettingsTabButton)
                continue;

            if (!selectable.gameObject.activeInHierarchy)
                continue;

            currentOptions.Add(selectable);
        }

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
}