using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingsTabButton : SettingsMenuSelectable
{
    [Header("Tab Panel")]
    public GameObject tabPanel;

    [Header("All Tab Panels")]
    public List<GameObject> allTabPanels = new List<GameObject>();

    [Header("This Tab's Scroll View")]
    public ScrollRect tabScrollRect;
    public RectTransform tabScrollContent;
    public RectTransform tabScrollViewport;

    public override void Activate()
    {
        if (navigator != null)
            navigator.SelectTab(this, true);
        else
            ShowThisTab();
    }

    public void ShowThisTab()
    {
        foreach (GameObject panel in allTabPanels)
        {
            if (panel != null)
                panel.SetActive(false);
        }

        if (tabPanel != null)
            tabPanel.SetActive(true);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (navigator != null && navigator.IsAdjustingSlider)
            return;

        if (navigator != null)
        {
            navigator.SelectTab(this, false);
        }
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (navigator != null && navigator.IsAdjustingSlider)
            return;

        if (navigator != null)
        {
            navigator.SelectTab(this, true);
        }
        else
        {
            ShowThisTab();
        }
    }
}