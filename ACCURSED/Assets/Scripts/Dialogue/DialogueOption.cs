using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DialogueOption : MonoBehaviour
{

    [SerializeField]
    private GameObject parent;

    [SerializeField]
    private GameObject[] friends;

    [SerializeField]
    public GameObject textDisplay;

    public NPCDialogue branchedText;

    public NPCDialogue[] tempHolder;



   // private int holderIndex;

    [SerializeField]
    private GameObject textBox;

    public void clickMe()
    {
        StartCoroutine(load());
    }

    private void OnEnable()
    {
        //tempHolder = textBox.GetComponent<Textbox>().refSpawner.GetComponent<DialogueSpawner>().baseText;
        //holderIndex = textBox.GetComponent<Textbox>().refSpawner.GetComponent<DialogueSpawner>().baseIndex;
    }

    private IEnumerator load()
    {

        //Debug.Log("AAAAAAAAA");

        //Debug.Log(parent.GetComponent<DialogueSpawner>().baseText);
        //tempHolder = parent.GetComponent<DialogueSpawner>().baseText;

        yield return null;

        /*
        for (int i = 0; i < textBox.GetComponent<Textbox>().npcText.Length; i++)
        {
            textBox.GetComponent<Textbox>().index = 0;
            //textBox.GetComponent<Textbox>().refSpawner.GetComponent<DialogueSpawner>().placeholder = null;
            //
            //textBox.GetComponent<Textbox>().refSpawner.GetComponent<DialogueSpawner>().placeholder[i] = branchedText;
            //textBox.GetComponent<Textbox>().refSpawner.GetComponent<DialogueSpawner>().baseText[i] = branchedText;
        }
        */
        //textBox.GetComponent<Textbox>().index = 0;
        //textBox.GetComponent<Textbox>().refSpawner.GetComponent<DialogueSpawner>().baseText[0] = branchedText;
        //textBox.GetComponent<Textbox>().refSpawner.GetComponent<DialogueSpawner>().placeholder[0] = branchedText;
        //textBox.GetComponent<Textbox>().refSpawner.GetComponent<DialogueSpawner>().placeholderOptions = textBox.GetComponent<Textbox>().refSpawner.GetComponent<DialogueSpawner>().inUse;

        //textBox.GetComponent<Textbox>().refSpawner.GetComponent<DialogueSpawner>().placeholder = textBox.GetComponent<Textbox>().refSpawner.GetComponent<DialogueSpawner>().baseText;
        //textBox.GetComponent<Textbox>().npcText = textBox.GetComponent<Textbox>().refSpawner.GetComponent<DialogueSpawner>().placeholder;
        //textBox.GetComponent<Textbox>().refSpawner.GetComponent<DialogueSpawner>().baseText.Clear(textBox.GetComponent<Textbox>().refSpawner.GetComponent<DialogueSpawner>().baseText, null, textBox.GetComponent<Textbox>().refSpawner.GetComponent<DialogueSpawner>().baseText.Length);
        // textBox.GetComponent<Textbox>().refSpawner.GetComponent<DialogueSpawner>().baseText = new NPCDialogue[0];
        //textBox.GetComponent<Textbox>().refSpawner.GetComponent<DialogueSpawner>().baseText[0] = branchedText;
        //textBox.GetComponent<Textbox>().refSpawner.GetComponent<DialogueSpawner>().baseText.Clear();
        //textBox.GetComponent<Textbox>().refSpawner.GetComponent<DialogueSpawner>().baseText.Add(branchedText);
        //textBox.GetComponent<Textbox>().refSpawner.GetComponent<DialogueSpawner>().spawnText();

        //textBox.GetComponent<Textbox>().refSpawner.GetComponent<DialogueSpawner>().baseText.Add(branchedText);
        //textBox.GetComponent<Textbox>().refSpawner.GetComponent<DialogueSpawner>().spawnText();

        //var temp = textBox.GetComponent<Textbox>().refSpawner.GetComponent<DialogueSpawner>().inUse;

        //textBox.GetComponent<Textbox>().refSpawner.GetComponent<DialogueSpawner>().baseText = textBox.GetComponent<Textbox>().refSpawner.GetComponent<DialogueSpawner>().inUse;

        //textBox.GetComponent<Textbox>().npcText.Clear();
        //textBox.GetComponent<Textbox>().npcText.Add(branchedText);
        //textBox.GetComponent<Textbox>().refSpawner.GetComponent<DialogueSpawner>().spawnText();

        //textBox.GetComponent<Textbox>().npcText.Clear();
        //textBox.GetComponent<Textbox>().refSpawner.GetComponent<DialogueSpawner>().doIt();
        /*
        textBox.GetComponent<Textbox>().index = 0;
        textBox.GetComponent<Textbox>().npcText.Add(branchedText);

        textBox.GetComponent<Textbox>().nextSentence();

        textBox.GetComponent<Textbox>().refSpawner.GetComponent<DialogueSpawner>().doIt();
        */

        /*
        textBox.GetComponent<Textbox>().refSpawner.GetComponent<DialogueSpawner>().placeholder.Add(branchedText);
        textBox.GetComponent<Textbox>().special = true;
        textBox.GetComponent<Textbox>().indexRefTemp = textBox.GetComponent<Textbox>().refSpawner.GetComponent<DialogueSpawner>().baseIndex;
        textBox.GetComponent<Textbox>().refSpawner.GetComponent<DialogueSpawner>().baseIndex++;
        textBox.GetComponent<Textbox>().baseIndexRef = textBox.GetComponent<Textbox>().refSpawner.GetComponent<DialogueSpawner>().placeholder.Count-1;
        textBox.GetComponent<Textbox>().nextSentence();
        */
        /*
        textBox.GetComponent<Textbox>().refSpawner.GetComponent<DialogueSpawner>().placeholder.Clear();
        textBox.GetComponent<Textbox>().refSpawner.GetComponent<DialogueSpawner>().placeholder.Add(branchedText);
        textBox.GetComponent<Textbox>().refSpawner.GetComponent<DialogueSpawner>().baseIndex = 0;
        textBox.GetComponent<Textbox>().refSpawner.GetComponent<DialogueSpawner>().activeAndClear();
        */

        var textbox = textBox.GetComponent<Textbox>();
        var spawner = textbox.refSpawner.GetComponent<DialogueSpawner>();

        spawner.placeholder = new List<NPCDialogue>
        {
            branchedText
        };

        spawner.baseIndex = 0;
        spawner.activeAndClear();


        textDisplay.SetActive(true);


        foreach (GameObject friend in friends)
        {
            if (friend.activeInHierarchy)
            {
                friend.SetActive(false);
            }
        }

    }

    private void OnDisable()
    {
        //textBox.GetComponent<Textbox>().refSpawner.GetComponent<DialogueSpawner>().baseText = tempHolder;
    }

}
