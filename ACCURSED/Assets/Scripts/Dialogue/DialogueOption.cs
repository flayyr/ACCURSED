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

    [SerializeField]
    private GameObject textBox;

    public void clickMe()
    {
        StartCoroutine(load());
    }

    private IEnumerator load()
    {
        yield return null;
        textBox.GetComponent<Textbox>().npcText = null;
        textBox.GetComponent<Textbox>().index = 0;
        textBox.GetComponent<Textbox>().npcText = branchedText;
        textBox.GetComponent<Textbox>().nextSentence();
        textDisplay.SetActive(true);

        foreach (GameObject friend in friends)
        {
            if (friend.activeInHierarchy)
            {
                friend.SetActive(false);
            }
        }

        //gameObject.SetActive(false);
        //parent.SetActive(false);
    }

}
