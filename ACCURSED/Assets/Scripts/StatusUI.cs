using UnityEngine;

public class StatusUI : MonoBehaviour
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
