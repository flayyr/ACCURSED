using UnityEngine;
using System.Collections;

public class UITransitions : MonoBehaviour
{
    /* GLOBAL METHODS USED FOR TRANSITIONS
    * feel free to overload for more scenarios!!
    */
    public static UITransitions Instance { get; private set; }

    private void Awake()
    {
        // Singleton check (makes sure there is only one escape menu instance)
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // fade
    public IEnumerator FadeTransition(CanvasGroup canvas, float startAlp, float endAlp, float dur)
    {
        float elapsed = 0f;

        while (elapsed < dur)
        {
            elapsed += Time.unscaledDeltaTime;

            canvas.alpha = Mathf.Lerp(startAlp, endAlp, elapsed / dur);
      
            yield return null;
        }

        canvas.alpha = endAlp;
    }

    // transform horizontally AND fade
    public IEnumerator FadeTransformXTransition(CanvasGroup canvas, float startAlp, float endAlp, float startX, float endX, float dur)
    {
        float elapsed = 0f;
        RectTransform rect = GetComponent<RectTransform>();

        while (elapsed < dur)
        {
            elapsed += Time.unscaledDeltaTime;

            canvas.alpha = Mathf.Lerp(startAlp, endAlp, elapsed / dur);

            float t = elapsed / dur;
            Vector2 pos = rect.anchoredPosition;
            pos.y = Mathf.Lerp(startX, endX, t);
            rect.anchoredPosition = pos;

            yield return null;
        }

        canvas.alpha = endAlp;
    }

    // transform vertically AND fade
    public IEnumerator FadeTransformYTransition(CanvasGroup canvas, float startAlp, float endAlp, float startY, float endY, float dur)
    {
        float elapsed = 0f;
        RectTransform rect = GetComponent<RectTransform>();

        while (elapsed < dur)
        {
            elapsed += Time.unscaledDeltaTime;

            canvas.alpha = Mathf.Lerp(startAlp, endAlp, elapsed / dur);

            float t = elapsed / dur;
            Vector2 pos = rect.anchoredPosition;
            pos.y = Mathf.Lerp(startY, endY, t);
            rect.anchoredPosition = pos;

            yield return null;
        }

        canvas.alpha = endAlp;
    }
}
