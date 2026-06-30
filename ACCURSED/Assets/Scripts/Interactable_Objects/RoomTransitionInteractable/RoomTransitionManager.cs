using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoomTransitionManager : MonoBehaviour
{
    public static RoomTransitionManager Instance { get; private set; }

    [Header("Fade Settings")]
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private float fadeOutDuration = 0.25f;
    [SerializeField] private float fadeInDuration = 0.25f;
    [SerializeField] private int fadeCanvasSortingOrder = 9999;

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

        if (fadeCanvasGroup == null)
        {
            CreateFadeCanvas();
        }

        fadeCanvasGroup.alpha = 0f;
        fadeCanvasGroup.blocksRaycasts = false;
        fadeCanvasGroup.interactable = false;
    }

    public IEnumerator TransitionToScene(string sceneName, string spawnID, Transform playerToMove)
    {
        if (isTransitioning) yield break;

        isTransitioning = true;

        if (playerToMove != null)
        {
            playerToMove.SetParent(null);
            DontDestroyOnLoad(playerToMove.gameObject);
        }

        yield return Fade(0f, 1f, fadeOutDuration);

        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName);

        while (!loadOperation.isDone)
        {
            yield return null;
        }

        yield return null;

        MovePlayerToSpawn(playerToMove, spawnID);

        yield return Fade(1f, 0f, fadeInDuration);

        isTransitioning = false;
    }

    private void MovePlayerToSpawn(Transform playerToMove, string spawnID)
    {
        if (playerToMove == null) return;

        TransitionSpawnPoint[] spawnPoints = FindObjectsOfType<TransitionSpawnPoint>();

        foreach (TransitionSpawnPoint spawnPoint in spawnPoints)
        {
            if (spawnPoint.SpawnID == spawnID)
            {
                playerToMove.position = spawnPoint.transform.position;
                playerToMove.rotation = spawnPoint.transform.rotation;
                return;
            }
        }

        Debug.LogWarning("No TransitionSpawnPoint found with SpawnID: " + spawnID);
    }

    private IEnumerator Fade(float startAlpha, float endAlpha, float duration)
    {
        fadeCanvasGroup.blocksRaycasts = true;

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;

            float t = timer / duration;
            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, t);

            yield return null;
        }

        fadeCanvasGroup.alpha = endAlpha;

        if (endAlpha <= 0f)
        {
            fadeCanvasGroup.blocksRaycasts = false;
        }
    }

    private void CreateFadeCanvas()
    {
        GameObject canvasObj = new GameObject("Room Transition Fade Canvas");
        canvasObj.transform.SetParent(transform);

        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = fadeCanvasSortingOrder;

        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        GameObject imageObj = new GameObject("Black Fade Image");
        imageObj.transform.SetParent(canvasObj.transform, false);

        Image image = imageObj.AddComponent<Image>();
        image.color = Color.black;

        RectTransform rect = imageObj.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        fadeCanvasGroup = canvasObj.AddComponent<CanvasGroup>();
    }
}