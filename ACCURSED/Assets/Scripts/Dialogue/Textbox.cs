using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class Textbox : MonoBehaviour
{
    [Header("References")]
    [Header("This stores the main text")]
    public List<NPCDialogue> npcText;
    [Header("This stores branching dialogue")]
    public List<NPCDialogue> npcTextBranches;

    private int npcTextBranchNum;

    [Header("This stores the text object to display unto")]
    [SerializeField]
    private TextMeshProUGUI textDisplay;
    private AudioSource source;

    public int index = 0;

    public int baseIndexRef;

    //private NPCDialogue tempRefRef;

    private bool typing = true;

    [SerializeField]
    private KeyCode inputKey;

    [SerializeField]
    private bool stopAudio;

    [Header("This holds the parent object for the buttons")]
    [SerializeField]
    private GameObject options;

    [Header("Usually this hides the text and background of the textbox \n but you can change it to just hide \n one or the other")]
    [SerializeField]
    private GameObject textBoxAndText;

    [Header("These are the button objects for multiple choices")]
    [SerializeField]
    private GameObject[] buttons;

    [HideInInspector]
    public List<NPCDialogue> secretHolder;
    [HideInInspector]
    public bool dodo;

    //[SerializeField]
    //private GameObject refQuest;

    //private NPCDialogue tempHolder;

    [HideInInspector]
    public int indexRefTemp;

    [HideInInspector]
    public bool special;

    public GameObject refSpawner;


    //the coroutine that is currently running
    private Coroutine runningCo;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        /*
        baseIndexRef = refSpawner.GetComponent<DialogueSpawner>().baseIndex;

        //tempRefRef = npcText[0];

        
        if(baseIndexRef >= npcText.Count)
        {
            baseIndexRef = npcText.Count - 1;
        }

        if (npcText[baseIndexRef].branches != null)
        {
            npcTextBranches = npcText[baseIndexRef].branches;
            npcTextBranchNum = npcTextBranches.Count;
            for (var i = 0; i < npcTextBranchNum; i++)
            {
                buttons[i].gameObject.GetComponent<DialogueOption>().branchedText = npcTextBranches[i];
            }
        }

        index = 0;
        typing = false;

        nextSentence();
        */

        stuff();

    }

    public void stuff()
    {
        baseIndexRef = refSpawner.GetComponent<DialogueSpawner>().baseIndex;

        //tempRefRef = npcText[0];


        if (baseIndexRef >= npcText.Count)
        {
            baseIndexRef = npcText.Count - 1;
        }

        if (npcText[baseIndexRef].branches != null)
        {
            npcTextBranches = npcText[baseIndexRef].branches;
            npcTextBranchNum = npcTextBranches.Count;
            for (var i = 0; i < npcTextBranchNum; i++)
            {
                buttons[i].gameObject.GetComponent<DialogueOption>().branchedText = npcTextBranches[i];
            }
        }

        index = 0;
        typing = false;

        nextSentence();
    }

    private void OnDisable()
    {
        index = 0;
        setSpecial();
    }

    public void nextSentence()
    {
        if (index < npcText[baseIndexRef].dialogueList.Length)
        {
            runningCo = StartCoroutine(WriteSentence());
        }
        else
        {
            index = 0;
            //npcText = null;
            gameObject.SetActive(false);
        }
    }

    IEnumerator WriteSentence()
    {
        textDisplay.text = npcText[baseIndexRef].dialogueList[index];
        index++;

        yield return null;
    }

    void nextSentenceSkip()
    {
        if (index < npcText[baseIndexRef].dialogueList.Length && index != npcText[baseIndexRef].branchNum)
        {
            StartCoroutine(SkipSentence());
        }

        else if(npcText[baseIndexRef].branching && index == npcText[baseIndexRef].branchNum)
        {
            index = 0;
            textBoxAndText.SetActive(false);

            for (var i = 0; i < npcTextBranchNum; i++)
            {
                buttons[i].gameObject.SetActive(true);
                buttons[i].gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = buttons[i].gameObject.GetComponent<DialogueOption>().branchedText.ButtonText;

                if (buttons.Length == 3)
                {
                    if (i == 2)
                    {
                        buttons[i].gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, buttons[i].gameObject.GetComponent<RectTransform>().anchoredPosition.y);
                    }
                }

            }
        }
        else if (npcText[baseIndexRef].sideBarOrNot && index == npcText[baseIndexRef].summonSidebar)
        {
            refSpawner.GetComponent<DialogueSpawner>().spawnSideBar();
        }

        else if (!npcText[baseIndexRef].branching)
        {
            index = 0;

            npcTextBranches = null;

            if (baseIndexRef < npcText.Count - 1 && !special)
            {
                baseIndexRef += 1;
            }

            refSpawner.GetComponent<DialogueSpawner>().baseIndex++;

            gameObject.SetActive(false);
        }

        if (npcText[baseIndexRef].varName != "")
        {
            var temp = npcText[baseIndexRef].varName;
            if (!QuestVarHolder.instance.questBools[temp])
            {
                QuestVarHolder.instance.questBools[temp] = true;
            }
        }

    }

    public void setSpecial()
    {
        //Debug.Log("setting");
        if (special)
        {
            //Debug.Log("inner setting");
            refSpawner.GetComponent<DialogueSpawner>().baseIndex--;
            baseIndexRef = indexRefTemp;
            indexRefTemp = 0;
            special = false;
        }
    }

    IEnumerator SkipSentence()
    {
        textDisplay.text = npcText[baseIndexRef].dialogueList[index];
        index++;
        yield return null;
    }


    void Update()
    {

        if (Input.GetKeyDown(inputKey))
        {
            if (textBoxAndText.activeInHierarchy)
            {
                if (typing)
                {
                    //skips to the end of the sentence
                    typing = false;
                    nextSentence();
                }
                else if (!typing)
                {
                    //goes to the next sentence
                    nextSentenceSkip();
                }
            }
            }

        }
}
