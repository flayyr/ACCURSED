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
    [Header("This stores the text object to display unto")]
    [SerializeField]
    private TextMeshProUGUI textDisplay;
    private AudioSource source;

    public int index = 0;
    private bool typing = true;
    //0 is fast, 1 is really really slow
    //[SerializeField]
    //private float typeSpeed = 0.05f;

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
            int i = 0;
            foreach (GameObject button in buttons)
            {
                //Debug.Log(i);
                button.gameObject.GetComponent<DialogueOption>().branchedText = npcTextBranches[i];
                i++;
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
        if (index < npcText.dialogueList.Length)
        {
            StartCoroutine(SkipSentence());
        }

        else if(npcText.branching && index == npcText.branchNum)
        {
            index = 0;
            npcText = null;
            textBoxAndText.SetActive(false);
            options.SetActive(true);
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

        //Debug.Log(textDisplay.maxVisibleCharacters);
        //Debug.Log(index);   

        if (Input.GetKeyDown(inputKey))
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
