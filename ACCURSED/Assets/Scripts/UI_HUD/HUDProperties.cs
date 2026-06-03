using UnityEngine;
using System.Collections;

public class HUDProperties : MonoBehaviour
{
    [SerializeField] private CanvasGroup hudCanvas;

    private Coroutine HUDAppear;
    private bool isShowing;
    void Awake()
    {
        hudCanvas.alpha = 0f;
        isShowing = false;
    }

    void Update()
    {
        if (!isShowing && Input.GetKeyDown(KeyCode.F))
        {
            ShowHUD();
            isShowing = true;
        }

    }

    private void ShowHUD()
    {
        if (HUDAppear != null)
        {
            StopCoroutine(HUDAppear);
        }
        HUDAppear = StartCoroutine(HUDCoroutine());
    }

    private IEnumerator HUDCoroutine()
    {
        yield return FadeTransition(0f, 1f, 0.4f);
        yield return new WaitForSeconds(7); // change this value to increase time before fade out
        yield return FadeTransition(1f, 0f, 0.8f);
        isShowing = false;
    }

    /* To Fade In: FadeTransition(0f, 1f, X), To Fade Out: FadeTransition(1f, 0f, X)
     * Lower fadeDur = faster fade transition, higher = slower fade trnasition
     */
    private IEnumerator FadeTransition(float startAlp, float endAlp, float fadeDur)
    {
        float elapsed = 0f;

        while (elapsed < fadeDur)
        {
            elapsed += Time.deltaTime;

            hudCanvas.alpha = Mathf.Lerp(startAlp, endAlp, elapsed / fadeDur);

            yield return null;
        }

        hudCanvas.alpha = endAlp;
    }
}
