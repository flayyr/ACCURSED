using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class summonTextbox : MonoBehaviour
{
    [SerializeField]
    private GameObject textBox;
    public NPCDialogue dialogue;

    private bool canTalk;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //summons textbox object
        if ((Input.GetKeyDown(KeyCode.E) && !textBox.activeInHierarchy) && canTalk)
        {
            textBox.GetComponent<Textbox>().index = 0;
            textBox.GetComponent<Textbox>().npcText = dialogue;
            textBox.SetActive(true);
        }
    }

    //checks if player is in range to talk or not
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Guy"))
        {
            canTalk = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Guy"))
        {
            canTalk = false;
        }
    }
}
