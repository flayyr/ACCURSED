using UnityEngine;
using UnityEngine.UI;

public class Inventory_ItemSlot : MonoBehaviour
{

    [SerializeField] public GameObject itemSlot;
    [SerializeField] public GameObject highlight;

    public Button b;
    void Start()
    {

        b = gameObject.GetComponent<Button>();
        b.onClick.AddListener(ExecuteTask);
    }

    void ExecuteTask()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
