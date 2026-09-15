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

    }

}
