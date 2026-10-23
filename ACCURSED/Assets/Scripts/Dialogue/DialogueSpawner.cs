using System;
using System.Collections.Generic;
//using System.Reflection;
//using Unity.VisualScripting;
//using UnityEditor.Rendering;
using UnityEngine;
//using UnityEngine.Networking;
using static NPCManager;

public class DialogueSpawner : MonoBehaviour
{

    [Header("Reference to the textbox prefab that should spawn")]
    [SerializeField]
    private GameObject textBoxPrefab;
    [Header("Refernece to the sidebar prefab that should spawn")]
    [SerializeField]
    private GameObject sideBarPrefab;

    [Header("main dialogue, not quest")]
    [SerializeField]
    public List<NPCDialogue> baseText;

    [Header("insert DEFAULT/FIRST sidebar here \n only fill out with initial side bar information")]
    [SerializeField] protected List<sideBarCustomization> sideBarFirst = new List<sideBarCustomization>();


    [Serializable]
   public class triggeredDialogue
    {
        public List<NPCDialogue> questText;
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
        public List<NPCDialogue> sidebarDialogues;
    }

    [Header("insert customized sidebar stuff here")]
    [SerializeField] protected List<sideBarCustomization> sideBarCustomizations = new List<sideBarCustomization>();

    //[Header("DONT TOUCH THESE")]
    [HideInInspector] public int baseIndex;
    [HideInInspector] public int baseSideDex;
    [HideInInspector] 
    public List<NPCDialogue> placeholder;
    [HideInInspector] public NPCDialogue placeholderOptions;
    [HideInInspector] public List<NPCDialogue> highway;

    [HideInInspector] public List<NPCDialogue> inUse;

    //[SerializeField]
    [HideInInspector] public bool[] tempRefVars;
    [HideInInspector] public bool[] tempRefSideVars;

    //[Header("ignore this")]
    //[SerializeField]
    [HideInInspector] public List<NPCDialogue> tempDialogueHolder;

    //[HideInInspector]
    //public bool dodo;
    //[HideInInspector]
    //public List<NPCDialogue> secretHolder;

    private void Start()
    {

        placeholder = baseText;

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
        sideBarPrefab.GetComponent<SideBar>().refParent = this.gameObject;

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
                sideBarPrefab.GetComponent<SideBar>().refParent = this.gameObject;

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
        //doIt();

        var tIndex = 0;
        //var a = 0;

        textBoxPrefab.GetComponent<Textbox>().refSpawner = this.gameObject;

        //Debug.Log("spawning");

        foreach (string var in varNames)
        {

            //inUse = triggeredDialogues[tIndex].questText;

            if (var != "")
            {

                if (QuestVarHolder.instance.questBools[var] != tempRefVars[tIndex])
                {
                    baseIndex = 0;
                    tempRefVars[tIndex] = QuestVarHolder.instance.questBools[var];
                }

                if (QuestVarHolder.instance.questBools[var])
                {
                    //inUse = triggeredDialogues[tIndex].questText;
                    // Ensure we have a source array
                    var source = triggeredDialogues[tIndex].questText;
                    if (source != null && !ReferenceEquals(placeholder, source))
                    {
                        inUse = new List<NPCDialogue>(source);
                        placeholder = new List<NPCDialogue>(source);
                    }
                }
            }
            tIndex++;
        }

        activeAndClear();

    }

    public void activeAndClear()
    {
        //textBoxPrefab.GetComponent<Textbox>().npcText.Clear();
        textBoxPrefab.GetComponent<Textbox>().npcText = new List<NPCDialogue>(placeholder);

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
    /*
    public void Update()
    {
        checkInput();
    }
    */

}
