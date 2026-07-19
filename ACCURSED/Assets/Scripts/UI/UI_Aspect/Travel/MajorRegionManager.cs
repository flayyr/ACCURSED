using UnityEngine;
using UnityEngine.UI;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField] private GameObject arrow;

    [SerializeField] public string region;

    private bool isSelected;

    public Button b;

    void Start()
    {
        if (region == TravelMenuController.currentRegion)
        {
            isSelected = true;
        }
        else { 
            isSelected = false; 
        }

        arrow.SetActive(isSelected);

        isSelected = false;
        b = gameObject.GetComponent<Button>();
        b.onClick.AddListener(Select);
    }

    void Update()
    {
        
    }

    void Select()
    {
        arrow.SetActive(true);
    }
}
