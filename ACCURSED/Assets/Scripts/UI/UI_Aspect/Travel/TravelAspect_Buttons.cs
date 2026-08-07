using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class TravelAspect_Buttons : MonoBehaviour
{
    [SerializeField] public GameObject title;
    public Button b;
    public Aspect asp;

    [SerializeField] private GameObject blackFade;
    private CanvasGroup blackFadeCanvas;
    void Awake()
    {
        b = gameObject.GetComponent<Button>();
        b.onClick.AddListener(Teleport);

        blackFadeCanvas = blackFade.GetComponent<CanvasGroup>();
        blackFade.SetActive(false);

        //Refresh(TravelMenuController.currentRegion);
    }

    void Teleport()
    {
        
    }
    private IEnumerator TransitionBlack()
    {
        blackFade.SetActive(true);
        blackFadeCanvas.alpha = 0f;

        yield return UITransitions.Instance.FadeTransition(blackFadeCanvas, 0f, 1f, 0.1f);
        yield return new WaitForSeconds(1);
        yield return UITransitions.Instance.FadeTransition(blackFadeCanvas, 1f, 0f, 0.1f);
        blackFade.SetActive(false);
    }


    void Update()
    {
        
    }

    public void Refresh(Aspect a)
    {
        MajorRegion reg = TravelMenuController.currentRegion;

        asp = a;
        title.GetComponent<TextMeshProUGUI>().text = a.locationName;
    }
}
