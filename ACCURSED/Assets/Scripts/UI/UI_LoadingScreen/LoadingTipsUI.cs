using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public struct loadingTipData
{
    [TextArea(1, 10)]
    public string loadingTipHeader;
    [TextArea(3, 10)]
    public string loadingTipDesc;
}
public class LoadingTipsUI : MonoBehaviour
{
    [SerializeField] GameObject loadingTipHeaderGO;
    [SerializeField] GameObject loadingTipDescGO;
    [SerializeField] CanvasGroup tipCG;

    [SerializeField] List<loadingTipData> loadingTips;

    //Regional Loading tips
    [SerializeField] List<loadingTipData> loadingTipsAltar;
    [SerializeField] List<loadingTipData> loadingTipsAltarLateGame;
    [SerializeField] List<loadingTipData> loadingTipsVita;
    [SerializeField] List<loadingTipData> loadingTipsRitus;
    [SerializeField] List<loadingTipData> loadingTipsMors;
    [SerializeField] List<loadingTipData> loadingTipsNihil;

    private int tipIndex;
    private loadingTipData currentTip;

    void Awake()
    {
        tipIndex = Random.Range(0, loadingTips.Count);
        currentTip = loadingTips[tipIndex];
        loadingTipDescGO.GetComponent<TextMeshProUGUI>().text = currentTip.loadingTipDesc;
        tipCG = loadingTipDescGO.GetComponent<CanvasGroup>();

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
        //currentTip = loadingTips[tipIndex];
        loadingTipDescGO.GetComponent<TextMeshProUGUI>().text = currentTip.loadingTipDesc;

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
