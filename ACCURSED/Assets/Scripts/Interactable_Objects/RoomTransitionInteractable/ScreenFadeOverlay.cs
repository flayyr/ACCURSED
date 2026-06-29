using System.Collections;
using UnityEngine;

public class ScreenFadeOverlay : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;

    private void Awake()
    {
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = false;
    }

    public IEnumerator FadeToBlack(float duration)
    {
        yield return Fade(0f, 1f, duration);
    }

    public IEnumerator FadeFromBlack(float duration)
    {
        yield return Fade(1f, 0f, duration);
    }

    private IEnumerator Fade(float startAlpha, float endAlpha, float duration)
    {
        float timer = 0f;

        canvasGroup.alpha = startAlpha;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;

            float t = timer / duration;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, t);

            yield return null;
        }

        canvasGroup.alpha = endAlpha;
    }
}