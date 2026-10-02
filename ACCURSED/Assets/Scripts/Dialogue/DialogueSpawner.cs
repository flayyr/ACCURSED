using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using static NPCManager;

public class DialogueSpawner : MonoBehaviour
{
    //[Header("This is just a reference to make the dialogue spawner \n not spawn if there is a side bar active")]
    //[SerializeField]
    //private GameObject sidebar;

    //[Header("Tick this box if there is quest-related dialogue \n like multiple dialogues for asking about quests")]
    //[SerializeField]
    //public bool questOrNot;

    /*
    [Header("This section is for checking quest progress \n fill in with JUST name of variable \n example: exampleQuestStarted")]
    //[SerializeField]
    //private string questNotStarted;
    [SerializeField]
    private string questStarted;
    [SerializeField]
    private string questInProgress;
    [SerializeField]
    private string questFinished;
    */

    //[SerializeField]
    //private int questNum;

    /*
    [Header("This is the object that holds quest variables \n only need to fill it out if this NPC has quests to give \n or info to reference")]
    [SerializeField]
    private GameObject questRef;
    */

    [Header("Reference to the textbox prefab that should spawn")]
    [SerializeField]
    private GameObject textBoxPrefab;
    [Header("Refernece to the sidebar prefab that should spawn")]
    [SerializeField]
    private GameObject sideBarPrefab;

    [Header("main dialogue, not quest")]
    [SerializeField]
    public NPCDialogue[] baseText;

    [Serializable]
   public class triggeredDialogue
    {
        public NPCDialogue[] questText;
    }

    [Header("This section is for checking quest progress \n fill in with JUST name of vairable \n example: exampleQuestStarted \n in order please")]
    [SerializeField]
    private string[] varNames;

    [Header("insert quest dialogues here \n MUST MATCH ORDER OF QUEST VARIABLES")]
    [SerializeField] protected List<triggeredDialogue> triggeredDialogues = new List<triggeredDialogue>();

    /*
    [Header("Use this section if this is quest related dialogue")]
    [SerializeField]
    private NPCDialogue[] questNotStartedText;
    [SerializeField]
    private NPCDialogue[] questStartedText;
    [SerializeField]
    private NPCDialogue[] questFinishedText;
    */


    [Header("Tick this box if there should be a side bar pop-up")]
    [SerializeField]
    private bool sideBarOrNot;

    [Header("For side bar NPC name")]
    [SerializeField]
    private string sideBarNPCName;

    [Header("one box per option, tick if that option should lead \n to shop, MAX 4")]
    [SerializeField]
    private bool[] sidebarOptions;

    /*
    [Header("Mark which one should give a quest if it gives one")]
    [SerializeField]
    private bool[] giveQuest;
    */

    [Header("what should each of the sidebar buttons say?")]
    [SerializeField]
    private string[] sideBarButtonNames;

    [Header("IF THERE ARE MULTIPLE CONVERSATIONS \n should be same amount of options \n give shop a slot but leave it blank \n \n if it's a quest, also leave it blank, \nfill it out in the quest section \n and leave slot blank")]
    [SerializeField]
    private NPCDialogue[] sidebarDialogues;

    public int baseIndex;

    [SerializeField]
    public bool[] tempRefVars;


    private void Start()
    {
        var i = 0;

        tempRefVars = new bool[varNames.Length];

        foreach (string var in varNames)
        {
            tempRefVars[i] = QuestVarHolder.instance.questBools[var];
            i++;
        }
    }

    public void Awake()
    {

    }

    public void spawnSideBar()
    {

        sideBarPrefab.GetComponent<SideBar>().NPCName = sideBarNPCName;
        sideBarPrefab.GetComponent<SideBar>().buttonClicked = -3;
        sideBarPrefab.GetComponent<SideBar>().dialogueOrShop = sidebarOptions;
        sideBarPrefab.GetComponent<SideBar>().bunchaDialogues = sidebarDialogues;
        sideBarPrefab.GetComponent<SideBar>().buttonName = sideBarButtonNames;
        sideBarPrefab.GetComponent<SideBar>().refTextbox = textBoxPrefab;
        sideBarPrefab.GetComponent<SideBar>().refParent = gameObject;

        if (!sideBarPrefab.activeInHierarchy)
        {
            sideBarPrefab.SetActive(true); 
        }
    }

    public void WORK()
    {
        //textBoxPrefab.GetComponent<Textbox>().baseIndex = 0;
    }
    public void spawnText()
    {

        var tIndex = 0;
        //var a = 0;

        textBoxPrefab.GetComponent<Textbox>().refSpawner = this.gameObject;

        if(textBoxPrefab.GetComponent<Textbox>().npcText.Length == 0)
        {
            textBoxPrefab.GetComponent<Textbox>().npcText = baseText;
        }

        foreach (string var in varNames)
        {
            if(var != "")
            {

                if (QuestVarHolder.instance.questBools[var] != tempRefVars[tIndex])
                {
                    baseIndex = 0;
                    tempRefVars[tIndex] = QuestVarHolder.instance.questBools[var];
                }


                if (QuestVarHolder.instance.questBools[var])
                {
                    if(textBoxPrefab.GetComponent<Textbox>().npcText != triggeredDialogues[tIndex].questText)
                    {
                        textBoxPrefab.GetComponent<Textbox>().npcText = triggeredDialogues[tIndex].questText;
                        //Debug.Log("happening");
                    }
                }
            }

            tIndex++;
        }

        if (!textBoxPrefab.activeInHierarchy)
        {
            textBoxPrefab.SetActive(true);
        }

    }

    public void checkInput()
    {

        //baseIndexRef = textBoxPrefab.GetComponent<Textbox>().baseIndex;

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
        //baseIndexRef = textBoxPrefab.GetComponent<Textbox>().baseIndex;

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

}
