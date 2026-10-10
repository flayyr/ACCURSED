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

    [Header("insert DEFAULT/FIRST sidebar here \n only fill out with initial side bar information")]
    [SerializeField] protected List<sideBarCustomization> sideBarFirst = new List<sideBarCustomization>();


    [Serializable]
   public class triggeredDialogue
    {
        public NPCDialogue[] questText;
    }

    [Header("This section is for checking quest progress FOR TEXTBOX \n fill in with JUST name of variable \n example: exampleQuestStarted \n in order please")]
    [SerializeField]
    private string[] varNames;

    [Header("This section is for checking variables FOR SIDEBAR \n fill in with JUST name of variable \n example: exampleQuestStarted \n in order please")]
    [SerializeField]
    private string[] sideBarVarNames;

    [Header("insert quest dialogues here \n MUST MATCH ORDER OF QUEST VARIABLES")]
    [SerializeField] protected List<triggeredDialogue> triggeredDialogues = new List<triggeredDialogue>();

    [Serializable]
    public class sideBarCustomization
    {

        [Header("For side bar NPC name")]
        [SerializeField]
        public string sideBarNPCName;

        [Header("one box per option, tick if that option should lead \n to shop, MAX 4")]
        [SerializeField]
        public bool[] sidebarOptions;

        [Header("what should each of the sidebar buttons say?")]
        [SerializeField]
        public string[] sideBarButtonNames;

        [Header("IF THERE ARE MULTIPLE CONVERSATIONS \n should be same amount of options \n give shop a slot but leave it blank \n max of four options")]
        [SerializeField]
        public NPCDialogue[] sidebarDialogues;
    }

    [Header("insert customized sidebar stuff here")]
    [SerializeField] protected List<sideBarCustomization> sideBarCustomizations = new List<sideBarCustomization>();

    [Header("DONT TOUCH THESE")]
    public int baseIndex;
    public int baseSideDex;

    //[SerializeField]
    public bool[] tempRefVars;
    public bool[] tempRefSideVars;

    [Header("ignore this")]
    [SerializeField]
    public NPCDialogue[] tempDialogueHolder;


    private void Start()
    {
        var i = 0;
        var j = 0;

        tempRefVars = new bool[varNames.Length];

        foreach (string var in varNames)
        {
            //Debug.Log("hi");
            tempRefVars[i] = QuestVarHolder.instance.questBools[var];
            i++;
        }

        tempRefSideVars = new bool[sideBarVarNames.Length];

        foreach (string var in sideBarVarNames)
        {
            //Debug.Log("yo");
            tempRefSideVars[j] = QuestVarHolder.instance.questBools[var];
            j++;
        }
    }

    public void spawnSideBar()
    {
        var tIndex = 0;

        //Debug.Log("yo");

        sideBarPrefab.GetComponent<SideBar>().NPCName = sideBarFirst[tIndex].sideBarNPCName;
        sideBarPrefab.GetComponent<SideBar>().buttonClicked = -3;
        sideBarPrefab.GetComponent<SideBar>().dialogueOrShop = sideBarFirst[tIndex].sidebarOptions;
        sideBarPrefab.GetComponent<SideBar>().bunchaDialogues = sideBarFirst[tIndex].sidebarDialogues;
        sideBarPrefab.GetComponent<SideBar>().buttonName = sideBarFirst[tIndex].sideBarButtonNames;
        sideBarPrefab.GetComponent<SideBar>().refTextbox = textBoxPrefab;
        sideBarPrefab.GetComponent<SideBar>().refParent = gameObject;

        foreach (string varName in sideBarVarNames)
        {
            if (QuestVarHolder.instance.questBools[varName] != tempRefSideVars[tIndex])
            {
                //Debug.Log("yo");
                //baseIndex = 0;
                tempRefSideVars[tIndex] = QuestVarHolder.instance.questBools[varName];
            }


            if (QuestVarHolder.instance.questBools[varName])
            {
                //Debug.Log("or this");
                //Debug.Log(tIndex);
                sideBarPrefab.GetComponent<SideBar>().NPCName = sideBarCustomizations[tIndex].sideBarNPCName;
                sideBarPrefab.GetComponent<SideBar>().buttonClicked = -3;
                sideBarPrefab.GetComponent<SideBar>().dialogueOrShop = sideBarCustomizations[tIndex].sidebarOptions;
                sideBarPrefab.GetComponent<SideBar>().bunchaDialogues = sideBarCustomizations[tIndex].sidebarDialogues;
                sideBarPrefab.GetComponent<SideBar>().buttonName = sideBarCustomizations[tIndex].sideBarButtonNames;
                sideBarPrefab.GetComponent<SideBar>().refTextbox = textBoxPrefab;
                sideBarPrefab.GetComponent<SideBar>().refParent = gameObject;

            }

            tIndex++;
        }

        if (textBoxPrefab.activeInHierarchy)
        {
            textBoxPrefab.SetActive(false);
        }

        if (!sideBarPrefab.activeInHierarchy)
        {
            sideBarPrefab.SetActive(true); 
        }
    }

    public void spawnText()
    {
        var tIndex = 0;
        //var a = 0;

        textBoxPrefab.GetComponent<Textbox>().refSpawner = this.gameObject;

        textBoxPrefab.GetComponent<Textbox>().npcText = baseText;

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
                    //Debug.Log("one above");
                    //if(textBoxPrefab.GetComponent<Textbox>().npcText != triggeredDialogues[tIndex].questText)
                    {
                        //Debug.Log("inside here");
                        textBoxPrefab.GetComponent<Textbox>().npcText = triggeredDialogues[tIndex].questText;
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

        if (!textBoxPrefab.activeInHierarchy && !sideBarPrefab.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                spawnText();
            }
        }
    }

    //COMMENT OUT LATER THIS IS FOR TESTING PURPOSES
    public void Update()
    {
        checkInput();
    }

}
