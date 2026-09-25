using System;
using System.Collections;
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

    public NPCDialogue tempHolder;

    [SerializeField]
    private GameObject textBox;

    public void clickMe()
    {
        StartCoroutine(load());
    }

    private void OnEnable()
    {
        tempHolder = textBox.GetComponent<Textbox>().npcText[textBox.GetComponent<Textbox>().baseIndex];
    }

    private IEnumerator load()
    {
        yield return null;
        //textBox.GetComponent<Textbox>().npcText = null;
        textBox.GetComponent<Textbox>().index = 0;
        //Debug.Log(textBox.GetComponent<Textbox>().npcText[textBox.GetComponent<Textbox>().baseIndex]);
        textBox.GetComponent<Textbox>().npcText[textBox.GetComponent<Textbox>().baseIndex] = branchedText;
        textBox.GetComponent<Textbox>().nextSentence();
        textDisplay.SetActive(true);

        foreach (GameObject friend in friends)
        {
            if (friend.activeInHierarchy)
            {
                tempHolder = null;
                friend.SetActive(false);
            }
        }

        //gameObject.SetActive(false);
        //parent.SetActive(false);
    }

    private void OnDisable()
    {
        //textBox.GetComponent<Textbox>().npcText[textBox.GetComponent<Textbox>().baseIndex] = tempHolder;
    }

}
