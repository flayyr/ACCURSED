using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    private bool dontUse;

    private PlayerInventory inven;

    private TextMeshProUGUI self;

   // private int holderIndex;

    [SerializeField]
    private GameObject textBox;

    public void clickMe()
    {
        StartCoroutine(load());
    }

    private void OnEnable()
    {
        inven = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();
        self = GetComponent<TextMeshProUGUI>();
        //inven = GetComponent<PlayerInventory>();
        //tempHolder = textBox.GetComponent<Textbox>().refSpawner.GetComponent<DialogueSpawner>().baseText;
        //holderIndex = textBox.GetComponent<Textbox>().refSpawner.GetComponent<DialogueSpawner>().baseIndex;
    }

    private IEnumerator load()
    {

        yield return null;

        var textbox = textBox.GetComponent<Textbox>();
        var spawner = textbox.refSpawner.GetComponent<DialogueSpawner>();

        //spawner.placeholder = new List<NPCDialogue>
        //{
        //branchedText
        //};

        if (branchedText.itemCheckID != "")
        {
            if (inven.HasItem(branchedText.itemCheckID))
            {
                dontUse = false;
            }
            else if(!inven.HasItem(branchedText.itemCheckID))
            {
                //gameObject.GetComponent<TextMeshProUGUI>().color = new Color32(128, 128, 128, 255);
                //GetComponent<TextMeshProUGUI>().color = new Color32(128, 128, 128, 255);
                //self.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
                dontUse = true;
            }
            else
            {
                dontUse = false;
            }

        }
        if (!dontUse)
        {
            textbox.npcText = new List<NPCDialogue>
        {
            branchedText
        };

            //spawner.baseIndex = 0;
            textbox.index = 0;
            //spawner.activeAndClear();

            if (!spawner.textBoxPrefab.activeInHierarchy)
            {
                spawner.textBoxPrefab.SetActive(true);
            }

            //textbox.nextSentence();
            textbox.stuff();

            //spawner.baseIndex++;

            textDisplay.SetActive(true);


            foreach (GameObject friend in friends)
            {
                if (friend.activeInHierarchy)
                {
                    friend.SetActive(false);
                }
            }
        }
    }

    private void OnDisable()
    {
        //textBox.GetComponent<Textbox>().refSpawner.GetComponent<DialogueSpawner>().baseText = tempHolder;
    }

}
