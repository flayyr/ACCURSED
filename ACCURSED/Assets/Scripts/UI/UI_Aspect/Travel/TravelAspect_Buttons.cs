using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class TravelAspect_Buttons : MonoBehaviour
{
    [SerializeField] public GameObject title;
    private Button b;
    private AspectSO asp;

    //[SerializeField] private GameObject blackFade;
    private CanvasGroup blackFadeCanvas;
    void Awake()
    {
        b = gameObject.GetComponent<Button>();
        b.onClick.AddListener(Teleport);

        //blackFadeCanvas = blackFade.GetComponent<CanvasGroup>();
        //blackFade.SetActive(false);
    }

    void Teleport()
    {
        if (AspectController.Instance.currentAspect == asp)
        {
            //Debug.Log("same aspect");
            TravelMenuController.Instance.CloseMenu();
            return;
        }

        //RoomTransitionWithoutPlayer.Instance.BeginTransition(asp.sceneName);
        RoomTransitionManager.Instance.BeginTransition(asp.sceneName, SetPlayerAfterTransition);
    }

    void SetPlayerAfterTransition()
    {
        PersistentPlayer.Instance.transform.position = asp.position;
        PersistentPlayer.controllerInstance.SetState(PlayerControlState.Normal);
    }

    //private IEnumerator TransitionBlack()
    //{
    //    blackFade.SetActive(true);
    //    blackFadeCanvas.alpha = 0f;

    //    yield return UITransitions.Instance.FadeTransition(blackFadeCanvas, 0f, 1f, 0.1f);
    //    yield return new WaitForSeconds(1);
    //    yield return UITransitions.Instance.FadeTransition(blackFadeCanvas, 1f, 0f, 0.1f);
    //    blackFade.SetActive(false);
    //}


    void Update()
    {
        
    }

    public void Refresh(AspectSO a)
    {
        MajorRegion reg = TravelMenuController.currentRegion;

        asp = a;
        title.GetComponent<TextMeshProUGUI>().text = a.locationName;
    }
}
