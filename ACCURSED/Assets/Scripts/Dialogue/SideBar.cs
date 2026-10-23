using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class SideBar : MonoBehaviour
{

    [SerializeField]
    private GameObject[] buttonPrefab;

    //[SerializeField]
    //private GameObject textboxPrefab;

    [SerializeField]
    private NPCDialogue error;

    [SerializeField]
    private TextMeshProUGUI nameDisplay;

    public GameObject refTextbox;

    public NPCDialogue tempDialogue;

    public GameObject refParent;

    public List<NPCDialogue> temptempDialogue;

    [SerializeField]
    public string NPCName;

    //public bool greatQuestHighway;
    private List<NPCDialogue> greatNPCHighway;

    [SerializeField]
    private GameObject shop;

    [Header("This should start off as -3 in the inspector")]
    public int buttonClicked;

    [Header("What each of the buttons will say")]
    [SerializeField]
    public string[] buttonName;

    [Header("Leave untagged for dialogue and tagged for shop")]
    [SerializeField]
    public bool[] dialogueOrShop;

    public bool[] questBoolsSideBar;

    public int tempRefIndex;

    public List<NPCDialogue> bunchaDialogues;

    private void OnEnable()
    {
        greatNPCHighway = new List<NPCDialogue>(refParent.GetComponent<DialogueSpawner>().placeholder);
        nameDisplay.text = NPCName;

        int i = 0;

        //refParent.GetComponent<DialogueSpawner>().tempDialogueHolder = refParent.GetComponent<DialogueSpawner>().baseText;
        refParent.GetComponent<DialogueSpawner>().tempDialogueHolder = new List<NPCDialogue>(refParent.GetComponent<DialogueSpawner>().placeholder);
        //Debug.Log(temptempDialogue);
        temptempDialogue = new List<NPCDialogue>(refParent.GetComponent<DialogueSpawner>().placeholder);

        foreach (NPCDialogue talk in bunchaDialogues)
        {
            buttonPrefab[i].GetComponent<SideBarButton>().parent = this.gameObject;
            buttonPrefab[i].GetComponent<SideBarButton>().tempHolder = talk;
            buttonPrefab[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = buttonName[i];
            //buttonPrefab[i].GetComponent<SideBarButton>().tempHolder = talk;
            buttonPrefab[i].SetActive(true);
            i++;
        }

        var tempLength = buttonName.Length;

        buttonPrefab[tempLength].GetComponent<SideBarButton>().parent = this.gameObject;
        buttonPrefab[tempLength].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Leave";
        buttonPrefab[tempLength].GetComponent<SideBarButton>().buttonNum = -2;
        buttonPrefab[tempLength].GetComponent<SideBarButton>().tempHolder = error;
        buttonPrefab[tempLength].SetActive(true);

        tempRefIndex = refParent.GetComponent<DialogueSpawner>().baseIndex;
        //Debug.Log(tempRefIndex);

    }

    private void Update()
    {
        if(buttonClicked != -3 && buttonClicked != -2)
        {
            if (dialogueOrShop[buttonClicked])
            {
                //implement shop here
                this.gameObject.SetActive(false);
            }
            if (!dialogueOrShop[buttonClicked])
            {

                var source = refParent.GetComponent<DialogueSpawner>();

                //refParent.GetComponent<DialogueSpawner>().baseIndex = 0;
                //refParent.GetComponent<DialogueSpawner>().baseText[0] = tempDialogue;
                // refParent.GetComponent<DialogueSpawner>().placeholder[0] = tempDialogue;
                //Debug.Log("THIS");
                //refParent.GetComponent<DialogueSpawner>().spawnText();
                //Debug.Log("yo");

                source.baseIndex = 0;
                source.placeholder.Clear();
                //source.placeholder.Add(tempDialogue);
                source.placeholder = new List<NPCDialogue>
                {
                    tempDialogue
                };
                //refTextbox.GetComponent<Textbox>().nextSentence();
                //source.spawnText();

                source.activeAndClear();
                refTextbox.GetComponent<Textbox>().stuff();


                this.gameObject.SetActive(false);
            }
        }

        else if (buttonClicked == -2)
        {
            this.gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        //Debug.Log("OR THIS");
        if (refParent != null)
        {
            var source = refParent.GetComponent<DialogueSpawner>();

            buttonClicked = -3;
            nameDisplay.text = null;

            int i = 0;

            //refParent.GetComponent<DialogueSpawner>().baseText = refParent.GetComponent<DialogueSpawner>().tempDialogueHolder;
            /*
            refParent.GetComponent<DialogueSpawner>().placeholder = new List<NPCDialogue>(refParent.GetComponent<DialogueSpawner>().tempDialogueHolder);
            refParent.GetComponent<DialogueSpawner>().placeholder = temptempDialogue;
            //refParent.GetComponent<DialogueSpawner>().placeholder = greatNPCHighway;
            refParent.GetComponent<DialogueSpawner>().baseIndex = tempRefIndex;
            refParent.GetComponent<DialogueSpawner>().baseIndex++;
            refParent.GetComponent<DialogueSpawner>().highway = new List<NPCDialogue>(greatNPCHighway);
            */

            //source.placeholder = new List<NPCDialogue>(refParent.GetComponent<DialogueSpawner>().tempDialogueHolder);
            /*
            source.placeholder = temptempDialogue;
            source.baseIndex = tempRefIndex;
            source.baseIndex++;
            source.highway = new List<NPCDialogue>(greatNPCHighway);
            */



            tempRefIndex = 0;


            foreach (string option in buttonName)
            {
                //buttonPrefab[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = null;
                buttonPrefab[i].SetActive(false);
                i++;
            }
        }
    }

}
