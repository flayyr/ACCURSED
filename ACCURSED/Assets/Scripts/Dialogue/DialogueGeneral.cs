using TMPro;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class Textbox : MonoBehaviour
{
    [Header("References")]
    [Header("This stores the main text")]
    public NPCDialogue npcText;
    [Header("This stores branching dialogue")]
    public NPCDialogue[] npcTextBranches;

    private int npcTextBranchNum;

    [Header("This stores the text object to display unto")]
    [SerializeField]
    private TextMeshProUGUI textDisplay;
    private AudioSource source;

    public int index = 0;
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

    /*
    private Color off = Color.black;
    private Color on = Color.black;

    //HAS NOT BEEN IMPLEMENTED AND PROBABLY WONT BE
    [Header("section for if NPC has multiple sets of dialogue")]
    [SerializeField]
    private bool multipleDialogues;
    [SerializeField]
    private int NumofDialogues;
    */

    //the coroutine that is currently running
    private Coroutine runningCo;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {

        if (npcText.branching)
        {

            npcTextBranches = npcText.branches;
            npcTextBranchNum = npcTextBranches.Length;

            //int i = 0;
            //foreach (GameObject button in buttons)
            //foreach(NPCDialogue branches in npcTextBranches)
            for(var i = 0; i < npcTextBranchNum; i++)
            {
                buttons[i].gameObject.GetComponent<DialogueOption>().branchedText = npcTextBranches[i];
            }
        }

        index = 0;
        typing = false;

        nextSentence();
    }


    public void nextSentence()
    {
        if (index < npcText.dialogueList.Length)
        {
            runningCo = StartCoroutine(WriteSentence());
        }
        else
        {
            index = 0;
            npcText = null;
            gameObject.SetActive(false);
        }
    }

    IEnumerator WriteSentence()
    {
        textDisplay.text = npcText.dialogueList[index];
        index++;

        yield return null;
    }

    void nextSentenceSkip()
    {
        if (index < npcText.dialogueList.Length && index != npcText.branchNum)
        {
            StartCoroutine(SkipSentence());
        }

        else if(npcText.branching && index == npcText.branchNum)
        {
            index = 0;
            npcText = null;
            textBoxAndText.SetActive(false);

            //int i = 0;
            //foreach (GameObject button in buttons)
            //foreach(NPCDialogue branches in npcTextBranches)
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

            //options.SetActive(true);
        }

        else if (!npcText.branching) 
        {
            index = 0;
            gameObject.SetActive(false);
        }
    }

    IEnumerator SkipSentence()
    {
        textDisplay.text = npcText.dialogueList[index];
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
