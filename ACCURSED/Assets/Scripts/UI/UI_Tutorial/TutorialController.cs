using UnityEngine;
using UnityEngine.InputSystem.XR;
using static UnityEditor.Progress;

public class TutorialController : MonoBehaviour

{
    public static TutorialController Instance { get; private set; }

    [SerializeField] public GameObject ui;

    [SerializeField] public TutorialSO debugPopup; //debug


    private GameObject currentTutUI;

    private bool isOpen;
    public static bool EscPressedThisFrame = false;

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

    void Update() 
    {
        //CheckIfClosed();
        DebugCommand(); //debug

        if (isOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            HideTutorial();
            EscPressedThisFrame = true;
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && EscPressedThisFrame)
        {
            EscPressedThisFrame = false;
        }
    }

    public void ShowTutorial(TutorialSO popup)
    {
        currentTutUI = Instantiate(ui);
        currentTutUI.GetComponent<TutorialUI>().Initialize(popup);

        isOpen = true;
        GamePauseController.Instance.PauseGame();
    }

    public void HideTutorial()
    {
        Destroy(currentTutUI);

        isOpen = false;
        GamePauseController.Instance.ResumeGame();
    }
    
    public bool getIsOpen()
    {
        return isOpen;
    }

    void DebugCommand() // press T for debug tutorial
    {
        if (Input.GetKeyDown(KeyCode.T) && !isOpen)
        {
            TutorialController.Instance.ShowTutorial(debugPopup);
        }

     
    }
}
