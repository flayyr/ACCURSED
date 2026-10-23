using UnityEngine;
using UnityEngine.UI;

public class InventorySortByButton : MonoBehaviour
{

    private Button b;

    void Start() {

        b = gameObject.GetComponent<Button>();
        b.onClick.AddListener(SortByType);
    }

    void SortByType()
    {
        Debug.Log("Sort by type");
    }

    void Update()
    {

    }

}

  
