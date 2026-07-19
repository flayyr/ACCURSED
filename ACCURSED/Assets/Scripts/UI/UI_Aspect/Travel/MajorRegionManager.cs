using UnityEngine;
using UnityEngine.UI;

public class MajorRegionManager : MonoBehaviour
{
    [SerializeField] private GameObject arrow;

    //[SerializeField] public string region;

    private bool isSelected;

    [SerializeField] public Button b;

    [SerializeField] MajorRegion reg;

    void Start()
    {
        CheckSelection();

        arrow.SetActive(isSelected);

        isSelected = false;
        b = gameObject.GetComponent<Button>();
        b.onClick.AddListener(Select);
    }

    void Update()
    {
        CheckSelection();
        arrow.SetActive(isSelected);
    }

    void Select()
    {
        //Debug.Log("aaaaaaaa");
        TravelMenuController.currentRegion = reg;
        TravelMenuUI.Instance.RefreshUI();
    }

    void CheckSelection()
    {
        if (reg == TravelMenuController.currentRegion)
        {
            isSelected = true;
        }
        else
        {
            isSelected = false;
        }
    }
}
