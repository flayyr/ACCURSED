using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomTransitionWithoutPlayer : MonoBehaviour
{
    public static RoomTransitionWithoutPlayer Instance { get; private set; }

    [Header("Fade Prefab")]
    [SerializeField] private ScreenFadeOverlay fadeOverlayPrefab;

    [Header("Fade Settings")]
    [SerializeField] private float fadeOutDuration = 0.25f;
    [SerializeField] private float fadeInDuration = 0.35f;

    [Header("Arrival Settings")]
    [SerializeField] private float holdBlackAfterSceneLoad = 0.1f;

    private bool isTransitioning;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void BeginTransition(string sceneName)
    {
        if (isTransitioning) return;

        StartCoroutine(TransitionRoutine(sceneName));
    }

    private IEnumerator TransitionRoutine(string sceneName)
    {
        isTransitioning = true;

        if (fadeOverlayPrefab == null)
        {
            Debug.LogError("RoomTransitionManager: fadeOverlayPrefab is not assigned.");
            isTransitioning = false;
            yield break;
        }

        ScreenFadeOverlay fadeOverlay = Instantiate(fadeOverlayPrefab);
        DontDestroyOnLoad(fadeOverlay.gameObject);

        yield return fadeOverlay.FadeToBlack(fadeOutDuration);

        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName);

        while (!loadOperation.isDone)
        {
            yield return null;
        }

        yield return null;
        yield return null;

        if (holdBlackAfterSceneLoad > 0f)
        {
            yield return new WaitForSecondsRealtime(holdBlackAfterSceneLoad);
        }

        yield return fadeOverlay.FadeFromBlack(fadeInDuration);

        Destroy(fadeOverlay.gameObject);

        isTransitioning = false;
    }
}

/*

RoomTransitionWithoutPlayer.Instance.BeginTransition("StartMenu");

*/