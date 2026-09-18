using System;
using System.Reflection;
using Unity.VisualScripting;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class DialogueSpawner : MonoBehaviour
{
    [Header("Tick this box if there is quest-related dialogue \n like multiple dialogues for whether or not you have an item")]
    [SerializeField]
    private bool questOrNot;

    [Header("This section is for checking quest progress \n fill in with JUST name of variable \n example: exampleQuestStarted")]
    //[SerializeField]
    //private string questNotStarted;
    [SerializeField]
    private string questStarted;
    [SerializeField]
    private string questFinished;

    [SerializeField]
    private int questNum;

    [SerializeField]
    private GameObject questRef;

    [SerializeField]
    private GameObject textBoxPrefab;
    [SerializeField]
    private NPCDialogue baseText;

    [Header("Use this section if this is quest related dialogue")]
    [SerializeField]
    private NPCDialogue questNotStartedText;
    [SerializeField]
    private NPCDialogue questStartedText;
    [SerializeField]
    private NPCDialogue questFinishedText;

    //[SerializeField]
    //private NPCDialogue[] branchingDialogue;

    void SpawnText()
    {
        if (questOrNot)
        {

            //checks if quest has been completed
            if (questRef.GetComponent<QuestVarHolder>().questBools[questFinished] == true)
            {
                textBoxPrefab.GetComponent<Textbox>().index = 0;
                textBoxPrefab.GetComponent<Textbox>().npcText = questFinishedText;
                //Debug.Log("quest hasnt been started yet");
            }
            //checks if quest is in progress
            else if (questRef.GetComponent<QuestVarHolder>().questBools[questStarted] == true)
            {
                textBoxPrefab.GetComponent<Textbox>().index = 0;
                textBoxPrefab.GetComponent<Textbox>().npcText = questStartedText;
                //Debug.Log("quest has been started");
            }
            //checks if quest has been started          
            else if (questRef.GetComponent<QuestVarHolder>().questBools[questStarted] == false)
            {
                textBoxPrefab.GetComponent<Textbox>().index = 0;
                textBoxPrefab.GetComponent<Textbox>().npcText = questNotStartedText;
                //Debug.Log("quest has been finished");
            }
            
        } 
        else
        {
            //if there is no quest, it will just do a default textbox
            textBoxPrefab.GetComponent<Textbox>().index = 0;
            textBoxPrefab.GetComponent<Textbox>().npcText = baseText;
            //Debug.Log("No quest to be had");
        }

        if(textBoxPrefab.activeInHierarchy == false)
        {
            textBoxPrefab.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            SpawnText();
        }
    }
}
