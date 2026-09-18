using UnityEngine;

public class SideBarButton : MonoBehaviour
{
    public int buttonNum;

    public NPCDialogue tempHolder;

    public bool giveQuest;

    [SerializeField]
    private GameObject sideBar;

    public void onClick()
    {
        //if (giveQuest)
        {
            sideBar.GetComponent<SideBar>().greatQuestHighway = giveQuest;
        }
        sideBar.GetComponent<SideBar>().tempDialogue = tempHolder;
        sideBar.GetComponent<SideBar>().buttonClicked = buttonNum;
    }

}
