using TMPro;
using UnityEngine;
using System.Collections;

public class FlavorPopUpUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI promptTextUI;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeAfterSeconds = 5f;

    bool closingMenu = false;

    public void SetText(string text)
    {
        promptTextUI.text = "[F] " + text;
    }

    private void Start()
    {
        StartCoroutine(MenuOpenRoutine(canvasGroup));
        Invoke("CloseMenu", fadeAfterSeconds);
    }

    public void CloseMenu()
    {
        if (!closingMenu)
        {
            StartCoroutine(MenuCloseRoutine(canvasGroup));
        }
    }

    private IEnumerator MenuOpenRoutine(CanvasGroup canvas)
    {
        canvas.alpha = 0f;

        yield return UITransitions.Instance.FadeTransition(canvas, 0f, 1f, 0.1f);
    }

    private IEnumerator MenuCloseRoutine(CanvasGroup canvas)
    {
        closingMenu = true;

        yield return UITransitions.Instance.FadeTransition(canvas, canvas.alpha, 0f, 0.1f);

        Destroy(gameObject);
    }
}
