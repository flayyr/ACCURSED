using TMPro;
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

    [SerializeField]
    public string NPCName;

    public bool greatQuestHighway;

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

    public NPCDialogue[] bunchaDialogues;

    private void OnEnable()
    {
        nameDisplay.text = NPCName;

        int i = 0;

        refParent.GetComponent<DialogueSpawner>().tempDialogueHolder = refParent.GetComponent<DialogueSpawner>().baseText;

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
                //refParent.GetComponent<DialogueSpawner>().baseIndex = tempRefIndex;
                this.gameObject.SetActive(false);
            }
            if (!dialogueOrShop[buttonClicked])
            {
                refParent.GetComponent<DialogueSpawner>().baseIndex = 0;
                refParent.GetComponent<DialogueSpawner>().baseText[0] = tempDialogue;
                refParent.GetComponent<DialogueSpawner>().spawnText();
                //Debug.Log("yo");
                this.gameObject.SetActive(false);
            }
        }
        else if (buttonClicked == -2)
        {
            //Debug.Log("happening");
            //Debug.Log(refParent.GetComponent<DialogueSpawner>().baseIndex);
            /*
            refParent.GetComponent<DialogueSpawner>().baseIndex = tempRefIndex;
            refParent.GetComponent<DialogueSpawner>().baseIndex++;
            */
            //Debug.Log(refParent.GetComponent<DialogueSpawner>().baseIndex);
            this.gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        buttonClicked = -3;
        nameDisplay.text = null;

        int i = 0;

        refParent.GetComponent<DialogueSpawner>().baseText = refParent.GetComponent<DialogueSpawner>().tempDialogueHolder;

        refParent.GetComponent<DialogueSpawner>().baseIndex = tempRefIndex;
        refParent.GetComponent<DialogueSpawner>().baseIndex++;

        tempRefIndex = 0;


        foreach (string option in buttonName)
        {
            //buttonPrefab[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = null;
            buttonPrefab[i].SetActive(false);
            i++;
        }
    }

}
