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
        yield return UITransitions.Instance.FadeTransition(hudCanvas, 0f, 1f, 0.3f);
        yield return new WaitForSeconds(7); // change this value to increase time before fade out
        yield return UITransitions.Instance.FadeTransition(hudCanvas, 1f, 0f, 0.4f);
        isShowing = false;
    }
}
