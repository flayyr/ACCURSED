using UnityEngine;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{

    public static InventoryUI Instance { get; private set; }


    private void Awake()
    {
        // Singleton check
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

    }


    void checkIfClosed()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (RightClickOptions.Instance.GetIsOpen())
            {
                RightClickOptions.Instance.Close();
            }
            else
            {
                InventoryController.Instance.CloseInventory();
            }
        }
    }


    void Update()
    {
        checkIfClosed();
    }

}
