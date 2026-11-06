using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

// This class manages the overall Item Pickup UI System 
public class FlavorTextPopupManager : MonoBehaviour
{
    public static FlavorTextPopupManager Instance { get; private set; }

    [SerializeField] Transform parentCanvas;
    [SerializeField] GameObject FlavorPopupPrefab;

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

    public void PromptAppear()
    {
        
    }

    public void PromptDisappear()
    {
        //StopCurrentTransition();

    }

    //private IEnumerator MenuOpenRoutine(CanvasGroup canvas)
    //{
    //    canvas.alpha = 0f;

    //    yield return UITransitions.Instance.FadeTransition(canvas, 0f, 1f, 0.1f);

    //    menuAppear = null;
    //}

    //private IEnumerator MenuCloseRoutine(CanvasGroup canvas)
    //{
    //    canvas.alpha = 1f;

    //    yield return UITransitions.Instance.FadeTransition(canvas, 1f, 0f, 0.1f);


    //    if (currentPrompt != null)
    //    {
    //        GameObject promptToDestroy = currentPrompt;

    //        currentPrompt = null;

    //        Destroy(promptToDestroy);
    //    }
        
    //    menuAppear = null;
    //}

    private void CheckPromptTrigger()
    {
        
    }

    // prompt popup, no action
    public void Prompt(string promptText)
    {
        
    }


}
