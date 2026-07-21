using UnityEngine;

public class GlobalUIController : MonoBehaviour
{
    public static GlobalUIController Instance { get; private set; }

    [SerializeField] public GameObject Player;

    private bool isOtherUIOpen;

    void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

    }

    void Update()
    {
        //Debug.Log(isOtherUIOpen);
    }

    public bool CheckIfOtherUIOpen()
    {
        if (EscMenuController.Instance != null && EscMenuController.Instance.getIsOpen()
            || AspectController.Instance != null && AspectController.Instance.getIsOpen()
            || TravelMenuController.Instance != null && TravelMenuController.Instance.getIsOpen())
        {
            isOtherUIOpen = true;

            //|| !inventoryUI.activeSelf && !statusUI.activeSelf;

        }
        else { isOtherUIOpen = false; }
        return isOtherUIOpen;
    }
}
