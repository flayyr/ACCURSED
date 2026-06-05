using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public bool isOpen;
    void checkIfClosed()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameObject.SetActive(false);
            isOpen = false;
        }
    }
    void Update()
    {
        checkIfClosed();
    }
}
