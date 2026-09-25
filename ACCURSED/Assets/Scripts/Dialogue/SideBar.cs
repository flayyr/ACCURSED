using TMPro;
using UnityEngine;

public class SideBar : MonoBehaviour
{

    [SerializeField]
    private GameObject[] buttonPrefab;

    //[SerializeField]
    //private GameObject textboxPrefab;

    [SerializeField]
    private TextMeshProUGUI nameDisplay;

    public GameObject refTextbox;

    public NPCDialogue[] tempDialogue;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    private void OnEnable()
    {
        nameDisplay.text = NPCName;

        int i = 0;

        foreach (NPCDialogue talk in bunchaDialogues)
        {
            buttonPrefab[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = buttonName[i];
            buttonPrefab[i].GetComponent<SideBarButton>().giveQuest = questBoolsSideBar[i];
            buttonPrefab[i].GetComponent<SideBarButton>().tempHolder = talk;
            buttonPrefab[i].SetActive(true);
            i++;
        }

        buttonPrefab[buttonName.Length].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Leave";
        buttonPrefab[buttonName.Length].GetComponent<SideBarButton>().buttonNum = -2;
        buttonPrefab[buttonName.Length].SetActive(true);

        tempRefIndex = refParent.GetComponent<DialogueSpawner>().baseIndexRef;

    }

    private void Update()
    {

        if(buttonClicked != -3 && buttonClicked != -2)
        {
            if (dialogueOrShop[buttonClicked])
            {
                //spawn shop
                Debug.Log("you summoned the shop!");
                this.gameObject.SetActive(false);

            }
            if (!dialogueOrShop[buttonClicked])
            {
                //if (greatQuestHighway)
                {
                    refParent.GetComponent<DialogueSpawner>().questOrNot = greatQuestHighway;
                }
                refParent.GetComponent<DialogueSpawner>().baseText[tempRefIndex] = tempDialogue[tempRefIndex];
                refParent.GetComponent<DialogueSpawner>().spawnText();
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
        buttonClicked = -3;
        nameDisplay.text = null;

        int i = 0;

        foreach (string option in buttonName)
        {
            //buttonPrefab[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = null;
            buttonPrefab[i].SetActive(false);
            i++;
        }
    }

}
