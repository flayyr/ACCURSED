using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomTransitionManager : MonoBehaviour
{
    public static RoomTransitionManager Instance { get; private set; }

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

    public void BeginTransition(string sceneName, string spawnID, Transform playerToMove)
    {
        if (isTransitioning) return;

        StartCoroutine(TransitionRoutine(sceneName, spawnID, playerToMove));
    }

    private IEnumerator TransitionRoutine(string sceneName, string spawnID, Transform playerToMove)
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

        if (playerToMove != null)
        {
            playerToMove.SetParent(null);
            DontDestroyOnLoad(playerToMove.gameObject);
        }

        yield return fadeOverlay.FadeToBlack(fadeOutDuration);

        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName);

        while (!loadOperation.isDone)
        {
            yield return null;
        }

        yield return null;
        yield return null;

        MovePlayerToSpawn(playerToMove, spawnID);

        if (holdBlackAfterSceneLoad > 0f)
        {
            yield return new WaitForSecondsRealtime(holdBlackAfterSceneLoad);
        }

        yield return fadeOverlay.FadeFromBlack(fadeInDuration);

        Destroy(fadeOverlay.gameObject);

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
}