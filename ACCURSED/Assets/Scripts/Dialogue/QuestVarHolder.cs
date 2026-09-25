using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;


public class QuestVarHolder : MonoBehaviour
{

    public static QuestVarHolder instance;

    //fill this in with whatever variables you needd for quest
    // a variable for before the quest has started, a variable for when the quest is in progress, and a variable for after the quest is done

    //public string[] varNames;

    public bool[] trueOrFalseBools;

    public Dictionary<string, bool> questBools = new Dictionary<string, bool>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        //Dictionary<bool, QuestVarHolder> questbools = new Dictionary<bool, QuestVarHolder>();
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        //questBools.Add("tempQuest", false);
        questBools.Add("tempQuestStarted", false);
        questBools.Add("tempQuestFinished", false);
    }

    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            questBools["tempQuest"] = true;
            Debug.Log(questBools["tempQuest"]);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            questBools["tempQuestStarted"] = true;
            Debug.Log(questBools["tempQuestStarted"]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            questBools["tempQuestFinished"] = true;
            Debug.Log(questBools["tempQuestFinished"]);
        }
        
    }

}
