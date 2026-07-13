using UnityEngine;

public class TutorialController : MonoBehaviour

{
    public static TutorialController Instance { get; private set; }

    [SerializeField] public GameObject ui;

    [SerializeField] public TutorialSO debugPopup; //debug


    private GameObject currentTutUI;

    private bool isOpen;

    void Awake()
    {
        isOpen = false;

        // Singleton check
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public bool CheckIfOpen()
    {
        return isOpen;
    }

    void Update() 
    {
        CheckIfClosed();
        DebugCommand(); //debug
    }

    private void ShowTutorial(TutorialSO popup)
    {
        currentTutUI = Instantiate(ui);
        ui.GetComponent<TutorialUI>().Initialize(popup);

        isOpen = true;
        GamePauseController.Instance.PauseGame();
    }

    private void HideTutorial()
    {
        Destroy(currentTutUI);

        isOpen = false;
        GamePauseController.Instance.ResumeGame();
    }
    
    private void CheckIfClosed()
    {
        if (isOpen && Input.GetMouseButtonDown(0))
        {
            HideTutorial();
        }
    }

    void DebugCommand() // press T for debug tutorial
    {
        if (Input.GetKeyDown(KeyCode.T) && !isOpen)
        {
            TutorialController.Instance.ShowTutorial(debugPopup);
        }

     
    }
}
