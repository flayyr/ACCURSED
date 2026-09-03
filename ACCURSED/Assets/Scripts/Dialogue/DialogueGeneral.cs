using TMPro;
using UnityEngine;
using System.Collections;

public class Textbox : MonoBehaviour
{
    [Header("References")]
    public NPCDialogue npcText;
    [SerializeField]
    private TextMeshProUGUI textDisplay;
    private AudioSource source;

    public int index = 0;
    private bool typing = true;
    //0 is fast, 1 is really really slow
    [SerializeField]
    private float typeSpeed = 0.05f;

    [SerializeField]
    private bool stopAudio;

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
        //off.a = 0f;
        //textDisplay.color = off;
        textDisplay.maxVisibleCharacters = 0;

        index = 0;
        typing = false;

        nextSentence();
    }


    void nextSentence()
    {
        if (index < npcText.dialogueList.Length)
        {
            //refreshes text to start writing next sentence
            //textDisplay.text = "";
            textDisplay.maxVisibleCharacters = 0;
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

        foreach (char Character in npcText.dialogueList[index].ToCharArray())
        {
            if (textDisplay.maxVisibleCharacters % 3 == 0)
            {
                if (stopAudio)
                {
                    source.Stop();
                }
                source.PlayOneShot(npcText.sound);
            }
            textDisplay.maxVisibleCharacters++;
            yield return new WaitForSeconds(typeSpeed);
        }
        index++;
        typing = true;
    }

    void nextSentenceSkip()
    {
        if (index < npcText.dialogueList.Length)
        {
            //textDisplay.text = "";
            textDisplay.maxVisibleCharacters = 0;
            StartCoroutine(SkipSentence());
        }
        else
        {
            index = 0;
            //textDisplay.text = "";
            textDisplay.maxVisibleCharacters = 0;
            gameObject.SetActive(false);
        }
    }

    IEnumerator SkipSentence()
    {
        StopCoroutine(runningCo);
        typing = true;
        //textDisplay.text = "";
        //textDisplay.maxVisibleCharacters = 0;
        textDisplay.text = npcText.dialogueList[index];
        textDisplay.maxVisibleCharacters = npcText.dialogueList[index].Length;
        yield return new WaitForSeconds(typeSpeed);
        index++;
    }


    void Update()
    {

        //Debug.Log(textDisplay.maxVisibleCharacters);
        //Debug.Log(index);   

        if (Input.GetKeyDown(KeyCode.E))
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
