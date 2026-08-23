using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    void checkIfClosed()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameObject.SetActive(false);
        }
    }
    void Update()
    {
        checkIfClosed();
    }
}
