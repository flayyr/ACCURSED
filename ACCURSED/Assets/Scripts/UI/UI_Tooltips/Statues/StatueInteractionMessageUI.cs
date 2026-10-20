using System.Collections;
using TMPro;
using UnityEngine;

public class StatueInteractionMessageUI : MonoBehaviour
{
    public static StatueInteractionMessageUI Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text messageText;

    [Header("Timing")]
    [Min(0.1f)]
    [SerializeField] private float defaultVisibleDuration = 1.5f;

    [Min(0f)]
    [SerializeField] private float fadeDuration = 0.15f;

    private Coroutine messageRoutine;

    public bool IsShowing { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        SetVisibleImmediate(false);
    }

    public void ShowMessage(string message)
    {
        ShowMessage(message, defaultVisibleDuration);
    }

    public void ShowMessage(string message, float visibleDuration)
    {
        if (messageRoutine != null)
            StopCoroutine(messageRoutine);

        messageRoutine = StartCoroutine(ShowRoutine(message, visibleDuration));
    }

    private IEnumerator ShowRoutine(string message, float visibleDuration)
    {
        IsShowing = true;

        if (messageText != null)
            messageText.text = message;

        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;

            if (fadeDuration <= 0f)
            {
                canvasGroup.alpha = 1f;
            }
            else
            {
                float timer = 0f;
                float startAlpha = canvasGroup.alpha;

                while (timer < fadeDuration)
                {
                    timer += Time.unscaledDeltaTime;
                    canvasGroup.alpha = Mathf.Lerp(startAlpha, 1f, Mathf.Clamp01(timer / fadeDuration));
                    yield return null;
                }

                canvasGroup.alpha = 1f;
            }
        }

        yield return new WaitForSecondsRealtime(Mathf.Max(0f, visibleDuration));

        if (canvasGroup != null && fadeDuration > 0f)
        {
            float timer = 0f;
            float startAlpha = canvasGroup.alpha;

            while (timer < fadeDuration)
            {
                timer += Time.unscaledDeltaTime;
                canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, Mathf.Clamp01(timer / fadeDuration));
                yield return null;
            }
        }

        SetVisibleImmediate(false);
        messageRoutine = null;
    }

    private void SetVisibleImmediate(bool visible)
    {
        IsShowing = visible;

        if (canvasGroup != null)
        {
            canvasGroup.alpha = visible ? 1f : 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }
    }
}
