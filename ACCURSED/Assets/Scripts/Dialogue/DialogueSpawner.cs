using System;
using System.Reflection;
using Unity.VisualScripting;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class DialogueSpawner : MonoBehaviour
{
    //[Header("This is just a reference to make the dialogue spawner \n not spawn if there is a side bar active")]
    //[SerializeField]
    //private GameObject sidebar;

    [Header("Tick this box if there is quest-related dialogue \n like multiple dialogues for asking about quests")]
    [SerializeField]
    public bool questOrNot;

    [Header("This section is for checking quest progress \n fill in with JUST name of variable \n example: exampleQuestStarted")]
    //[SerializeField]
    //private string questNotStarted;
    [SerializeField]
    private string questStarted;
    [SerializeField]
    private string questFinished;

    //[SerializeField]
    //private int questNum;

    [Header("This is the object that holds quest variables \n only need to fill it out if this NPC has quests to give \n or info to reference")]
    [SerializeField]
    private GameObject questRef;

    [Header("Reference to the textbox prefab that should spawn")]
    [SerializeField]
    private GameObject textBoxPrefab;
    [Header("Refernece to the sidebar prefab that should spawn")]
    [SerializeField]
    private GameObject sideBarPrefab;

    [Header("main dialogue, not quest")]
    [SerializeField]
    public NPCDialogue[] baseText;

    [Header("Use this section if this is quest related dialogue")]
    [SerializeField]
    private NPCDialogue[] questNotStartedText;
    [SerializeField]
    private NPCDialogue[] questStartedText;
    [SerializeField]
    private NPCDialogue[] questFinishedText;


    [Header("Tick this box if there should be a side bar pop-up")]
    [SerializeField]
    private bool sideBarOrNot;

    [Header("For side bar NPC name")]
    [SerializeField]
    private string sideBarNPCName;

    [Header("one box per option, tick if that option should lead \n to shop, MAX 4")]
    [SerializeField]
    private bool[] sidebarOptions;

    [Header("Mark which one should give a quest if it gives one")]
    [SerializeField]
    private bool[] giveQuest;

    [Header("what should each of the sidebar buttons say?")]
    [SerializeField]
    private string[] sideBarButtonNames;

    [Header("IF THERE ARE MULTIPLE CONVERSATIONS \n should be same amount of options \n give shop a slot but leave it blank \n \n if it's a quest, also leave it blank, \nfill it out in the quest section \n and leave slot blank")]
    [SerializeField]
    private NPCDialogue[] sidebarDialogues;

    public int baseIndexRef;
    public int questIndexRef1;
    public int questIndexRef2;

    //[SerializeField]
    //private NPCDialogue[] branchingDialogue;

    private void Start()
    {

    }

    public void spawnSideBar()
    {
        /*int i = 0;
        foreach(bool quest in giveQuest)
        {
            if (quest)
            {
                sideBarPrefab.GetComponent<SideBar>().questButton = i;
            }
            i++;
        }
        */
        sideBarPrefab.GetComponent<SideBar>().questBoolsSideBar = giveQuest;
        sideBarPrefab.GetComponent<SideBar>().NPCName = sideBarNPCName;
        sideBarPrefab.GetComponent<SideBar>().buttonClicked = -3;
        sideBarPrefab.GetComponent<SideBar>().dialogueOrShop = sidebarOptions;
        sideBarPrefab.GetComponent<SideBar>().bunchaDialogues = sidebarDialogues;
        sideBarPrefab.GetComponent<SideBar>().buttonName = sideBarButtonNames;
        sideBarPrefab.GetComponent<SideBar>().refTextbox = textBoxPrefab;
        sideBarPrefab.GetComponent<SideBar>().refParent = gameObject;
        //baseIndexRef = textBoxPrefab.GetComponent<Textbox>().baseIndex;

        /*
        if (sidebarDialogues != null)
        {

        }
        */

        if (!sideBarPrefab.activeInHierarchy)
        {
            sideBarPrefab.SetActive(true); 
        }
    }

    public void spawnText()
    {

        if (questOrNot)
        {
            if (questRef != null)
            {
                //checks if quest has been completed
                if (questRef.GetComponent<QuestVarHolder>().questBools[questFinished] == true)
                {
                    textBoxPrefab.GetComponent<Textbox>().index = 0;
                    textBoxPrefab.GetComponent<Textbox>().npcText[textBoxPrefab.GetComponent<Textbox>().baseIndex] = questFinishedText[textBoxPrefab.GetComponent<Textbox>().baseQuest1Index];
                    //Debug.Log("quest hasnt been started yet");
                }
                //checks if quest is in progress
                else if (questRef.GetComponent<QuestVarHolder>().questBools[questStarted] == true)
                {
                    textBoxPrefab.GetComponent<Textbox>().index = 0;
                    textBoxPrefab.GetComponent<Textbox>().npcText[textBoxPrefab.GetComponent<Textbox>().baseIndex] = questFinishedText[textBoxPrefab.GetComponent<Textbox>().baseQuest2Index];
                    //Debug.Log("quest has been started");
                }
                //checks if quest has been started          
                else if (questRef.GetComponent<QuestVarHolder>().questBools[questStarted] == false)
                {
                    textBoxPrefab.GetComponent<Textbox>().index = 0;
                    textBoxPrefab.GetComponent<Textbox>().npcText[textBoxPrefab.GetComponent<Textbox>().baseIndex] = questFinishedText[textBoxPrefab.GetComponent<Textbox>().baseQuest3Index];
                    //Debug.Log("quest has been finished");
                }
            }
        } 
        else
        {
            //if there is no quest, it will just do a default textbox
            textBoxPrefab.GetComponent<Textbox>().index = 0;
            textBoxPrefab.GetComponent<Textbox>().npcText = baseText;
            //textBoxPrefab.GetComponent<Textbox>().npcText[textBoxPrefab.GetComponent<Textbox>().baseIndex] = baseText[baseIndexRef];
            //Debug.Log("No quest to be had");
        }

        if(textBoxPrefab.activeInHierarchy == false)
        {
            textBoxPrefab.SetActive(true);
        }
    }

    public void checkInput()
    {

        baseIndexRef = textBoxPrefab.GetComponent<Textbox>().baseIndex;

        if (!sideBarOrNot && !textBoxPrefab.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                spawnText();
            }
        }
        else if (sideBarOrNot && !textBoxPrefab.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                spawnSideBar();
            }

        }
    }

    //COMMENT OUT LATER THIS IS FOR TESTING PURPOSES
    public void Update()
    {
        baseIndexRef = textBoxPrefab.GetComponent<Textbox>().baseIndex;

        if (!sideBarOrNot && !textBoxPrefab.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                /*
                if (baseText[baseIndexRef].GetComponent<NPCDialogue>().varName != null)
                {
                    questOrNot = true;
                }
                */
                spawnText();
            }
        }
        else if (sideBarOrNot && !textBoxPrefab.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                spawnSideBar();
            }

        }
    }

}
