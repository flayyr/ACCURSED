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

    [Header("This section is for checking quest progress \n fill in with name of variable \n example: QuestVarHolder.instance.exampleQuestStarted")]
    [SerializeField]
    private string questNotStarted;
    [SerializeField]
    private string questStarted;
    [SerializeField]
    private string questFinished;

    [SerializeField]
    private int questNum;

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

    [SerializeField]
    private NPCDialogue[] branchingDialogue;

    void SpawnText()
    {

        
        if (questOrNot)
        {
            //FieldInfo questNStart = typeof(QuestVarHolder).GetField(questNotStarted, BindingFlags.Public | BindingFlags.Instance);
            //FieldInfo questNComplete = typeof(QuestVarHolder).GetField(questStarted, BindingFlags.Public | BindingFlags.Instance);
            //FieldInfo questNFinish = typeof(QuestVarHolder).GetField(questFinished, BindingFlags.Public | BindingFlags.Instance);

            string questNStart = questNotStarted.ToString();

            //foreach(TestList<string, bool> pair in TestDictionary)
            //{
            //Debug.Log(refVars.GetComponent<TestList>().questNames[1]);
            //}

            //if (QuestVarHolder.instance.(bool)questNStart.GetValue(this) == false)

            //checks if quest isnt started
            if (QuestVarHolder.instance.trueOrFalseBools[questNum] == false)
            {
                //object value = questNStart.GetValue(QuestVarHolder.instance);
                textBoxPrefab.GetComponent<Textbox>().index = 0;
                textBoxPrefab.GetComponent<Textbox>().npcText = questNotStartedText;
                Debug.Log("quest hasnt been started yet");
            }
            //checks if quest is started
            else if (QuestVarHolder.instance.trueOrFalseBools[questNum + 1] == false)
            {
                //object value = questNStart.GetValue(QuestVarHolder.instance);
                textBoxPrefab.GetComponent<Textbox>().index = 0;
                textBoxPrefab.GetComponent<Textbox>().npcText = questStartedText;
                Debug.Log("quest has been started");
            }
            //checks if quest is finished
            else if (QuestVarHolder.instance.trueOrFalseBools[questNum + 2] == false)
            {
                //object value = questNStart.GetValue(QuestVarHolder.instance);
                textBoxPrefab.GetComponent<Textbox>().index = 0;
                textBoxPrefab.GetComponent<Textbox>().npcText = questFinishedText;
                Debug.Log("quest has been finished");
            }
        }
        else
        {
            textBoxPrefab.GetComponent<Textbox>().index = 0;
            textBoxPrefab.GetComponent<Textbox>().npcText = baseText;
            if (branchingDialogue != null)
            {
                textBoxPrefab.GetComponent<Textbox>().npcTextBranches = branchingDialogue;
                Debug.Log("no quest to be had");
            }
        }

        if(textBoxPrefab.activeInHierarchy == false)
        {
            textBoxPrefab.SetActive(true);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnText();
        }
    }
}
