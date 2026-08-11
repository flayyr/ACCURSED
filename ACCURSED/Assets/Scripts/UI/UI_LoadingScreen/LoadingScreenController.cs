using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LoadingScreenController : MonoBehaviour
{
    public static LoadingScreenController Instance { get; private set; }

    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private CanvasGroup menuCanvas;
    [SerializeField] private Slider slider;

    private bool isOpen = false;

    private void Awake()
    {
        loadingScreen.SetActive(false);

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public IEnumerator OpenScreen()
    {
        loadingScreen.SetActive(true);
        isOpen = true;

        menuCanvas.alpha = 0f;
        slider.value = 0f;

        yield return UITransitions.Instance.FadeTransition(menuCanvas, 0f, 1f, 0.5f);
    }

    public IEnumerator CloseScreen()
    {
        isOpen = false;

        menuCanvas.alpha = 1f;
        slider.value = 1f;

        yield return UITransitions.Instance.FadeTransition(menuCanvas, 1f, 0f, 0.5f);

        loadingScreen.SetActive(false);

    }

    public bool getIsOpen()
    {
        return isOpen;
    }

    public void SetProgress(float progress)
    {
        slider.value = progress;
    }
}