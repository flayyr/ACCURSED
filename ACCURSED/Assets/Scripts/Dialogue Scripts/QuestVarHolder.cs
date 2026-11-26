using System;
using System.Collections.Generic;
using UnityEngine;


public class QuestVarHolder : MonoBehaviour
{

    public static QuestVarHolder instance;

    //fill this in with whatever variables you needd for quest
    // a variable for before the quest has started, a variable for when the quest is in progress, and a variable for after the quest is done

    //public string[] varNames;

    //public bool[] trueOrFalseBools;

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

        questBools.Add("reckonerDefeat", false);
        questBools.Add("ReckonerEcho", false);
        questBools.Add("gotEye", false);

        questBools.Add("Graveyard", false);
        questBools.Add("Rest1", false);
        questBools.Add("Rest2", false);

        questBools.Add("PLACEHOLDER1", false);
        questBools.Add("PLACEHOLDER2", false);

        questBools.Add("ContentWoman1", false);
        questBools.Add("ContentWoman2", false);
        questBools.Add("ContentWomanAltar", false);

        questBools.Add("FractureKnight1", false);
        questBools.Add("FractureKnight2", false);

        questBools.Add("RimelightMerchant1", false);
        questBools.Add("CotsDead", false);


    }

    private void Start()
    {
        //questBools.Add("tempQuest", false);
    }

    private void Update()
    {

        //to be commented out later, currently for testing purposes.
        if (Input.GetKeyDown(KeyCode.U))
        {
            questBools["reckonerDefeat"] = true;
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            questBools["Graveyard"] = true;
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            questBools["PLACEHOLDER1"] = true;
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            questBools["PLACEHOLDER2"] = true;
            questBools["FractureKnight2"] = true;
        }

    }

}
