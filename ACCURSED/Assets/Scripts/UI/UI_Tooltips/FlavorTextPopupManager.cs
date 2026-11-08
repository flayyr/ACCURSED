using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

// This class manages the overall Item Pickup UI System 
public class FlavorTextPopupManager : MonoBehaviour
{
    public static FlavorTextPopupManager Instance { get; private set; }

    [SerializeField] Transform parentCanvas;
    [SerializeField] FlavorPopUpUI FlavorPopupPrefab;

    Queue<FlavorPopUpUI> popupQueue = new Queue<FlavorPopUpUI>();

    void Awake()
    {
        // Singleton check
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        //for some reason onEnable gets called before persistent player instance is set sometimes, assigning again in start to avoid that.
        if (PersistentPlayer.Instance != null)
            PersistentPlayer.Instance.GetComponent<PlayerController>().InteractKeyPressed += CheckPromptTrigger;
    }
    private void OnDisable()
    {
        if(PersistentPlayer.Instance!=null)
            PersistentPlayer.Instance.GetComponent<PlayerController>().InteractKeyPressed -= CheckPromptTrigger;
    }

    private void PromptAppear(string text)
    {
        FlavorPopUpUI currPopup = Instantiate(FlavorPopupPrefab, parentCanvas);
        currPopup.SetText(text);
        popupQueue.Enqueue(currPopup);
    }

    private void PromptDisappear()
    {
        while(popupQueue.Count>0 && popupQueue.Peek() != null)
        {
            FlavorPopUpUI currPopup = popupQueue.Dequeue();
            currPopup.CloseMenu();
        }
    }

    private void CheckPromptTrigger()
    {
        PromptDisappear();
    }

    // prompt popup, no action
    public void Prompt(string promptText)
    {
        PromptAppear(promptText);
    }


}
