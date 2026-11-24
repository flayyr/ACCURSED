using UnityEngine;

public class SideBarButton : MonoBehaviour
{
    public int buttonNum;

    public GameObject parent;

    public NPCDialogue tempHolder;

    //public bool giveQuest;

    [SerializeField]
    private GameObject sideBar;

    public void onClick()
    {
        //Debug.Log(sideBar.GetComponent<SideBar>().tempDialogue[sideBar.GetComponent<SideBar>().tempRefIndex]);
        //Debug.Log(sideBar.GetComponent<SideBar>().buttonClicked);

        parent.GetComponent<SideBar>().tempDialogue = tempHolder;
        sideBar.GetComponent<SideBar>().buttonClicked = buttonNum;

        this.gameObject.SetActive(false);

        //sideBar.GetComponent<SideBar>().tempDialogue[sideBar.GetComponent<SideBar>().tempRefIndex] = tempHolder;
    }

    private void OnDisable()
    {
        parent.GetComponent<SideBar>().tempDialogue = tempHolder;
    }

}
