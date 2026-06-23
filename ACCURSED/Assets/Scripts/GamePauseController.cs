using UnityEngine;

public class GamePauseController : MonoBehaviour
{
    // This class manages globally Pausing/Resuming 
    public static GamePauseController Instance { get; private set; }

    private bool isPaused;
    void Awake()
    {
        isPaused = false;

        // Singleton check
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public bool CheckIfPaused()
    {
        return isPaused;
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
    }

    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        
    }
}
