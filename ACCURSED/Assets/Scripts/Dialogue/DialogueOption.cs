using System;
using System.Collections;
using UnityEngine;

public class DialogueOption : MonoBehaviour
{

    [SerializeField]
    private GameObject parent;

    [SerializeField]
    private GameObject textDisplay;

    public NPCDialogue branchedText;

    [SerializeField]
    private GameObject textBox;

    public void clickMe()
    {
        StartCoroutine(load());
    }

    private IEnumerator load()
    {
        Debug.Log("hi");
        yield return null;
        textBox.GetComponent<Textbox>().npcText = null;
        textBox.GetComponent<Textbox>().index = 0;
        textBox.GetComponent<Textbox>().npcText = branchedText;
        textBox.GetComponent<Textbox>().nextSentence();
        //textBox.SetActive(true);
        textDisplay.SetActive(true);
        parent.SetActive(false);
    }

}
