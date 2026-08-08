using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class LoadingTipsUI : MonoBehaviour
{
    [SerializeField] GameObject loadingTip;
    [TextArea(3, 10)]
    [SerializeField] List<string> loadingTips;

    private int tipIndex;
    private string currentTip;
    private CanvasGroup tipCG;

    void Awake()
    {
        tipIndex = Random.Range(0, loadingTips.Count);
        currentTip = loadingTips[tipIndex];
        loadingTip.GetComponent<TextMeshProUGUI>().text = currentTip;
        tipCG = loadingTip.GetComponent<CanvasGroup>();
    }

    private void NextTip()
    {
        StartCoroutine(TransitionNextTip());
    }

    private IEnumerator TransitionNextTip()
    {

        tipCG.alpha = 0f;
        yield return UITransitions.Instance.FadeTransition(tipCG, 1f, 0f, 0.1f);

        UITransitions.Instance.FadeTransition(tipCG, 0f, 1f, 0.1f);
        if (tipIndex < loadingTips.Count - 1)
        {
            tipIndex++;
        }
        else
        {
            tipIndex = 0;
        }
        currentTip = loadingTips[tipIndex];
        loadingTip.GetComponent<TextMeshProUGUI>().text = currentTip;

        tipCG.alpha = 1f;
        yield return UITransitions.Instance.FadeTransition(tipCG, 0f, 1f, 0.1f);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            NextTip();
        }
    }

 
}
