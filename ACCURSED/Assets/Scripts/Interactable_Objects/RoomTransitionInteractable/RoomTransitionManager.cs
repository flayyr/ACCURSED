using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomTransitionManager : MonoBehaviour
{
    public static RoomTransitionManager Instance { get; private set; }

    [Header("Fade Prefab")]
    [SerializeField] private ScreenFadeOverlay fadeOverlayPrefab;

    [Header("Fade Settings")]
    [Min(0f)][SerializeField] private float fadeOutDuration = 0.25f;
    [Min(0f)][SerializeField] private float deathFadeOutDuration = 0.25f;
    [Min(0f)][SerializeField] private float fadeInDuration = 0.35f;
    [Min(0f)][SerializeField] private float holdBlackAfterSceneLoad = 0.1f;

    [Header("Carried Player")]
    [Tooltip("When a player is carried into a scene, remove any other active Player-tagged roots already in that scene.")]
    [SerializeField] private bool removeDuplicatePlayers = true;

    [SerializeField] private string playerTag = "Player";

    [Tooltip("Stops carried Rigidbody2D movement after placing the player at the destination spawn point.")]
    [SerializeField] private bool clearPlayerVelocityOnArrival = true;

    public bool IsTransitioning => isTransitioning;

    private bool isTransitioning;
    private ScreenFadeOverlay activeFadeOverlay;

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

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    /// <summary>
    /// Fades out, changes scene, then fades in. No player object is preserved.
    /// </summary>
    public bool BeginTransition(string sceneName)
    {
        return BeginTransitionInternal(sceneName, string.Empty, null, null);
    }

    /// <summary>
    /// Same as BeginTransition(sceneName), with an optional action invoked after
    /// the new scene loads while the screen is still black.
    /// </summary>
    public bool BeginTransition(string sceneName, Action afterSceneLoaded, bool isDeath = true)
    {
        return BeginTransitionInternal(sceneName, string.Empty, null, afterSceneLoaded, isDeath);
    }

    /// <summary>
    /// Carries the supplied player into the next scene and moves it to spawnID.
    /// The player is only placed in DontDestroyOnLoad during the loading process;
    /// it is moved back into the destination scene afterward.
    /// </summary>
    public bool BeginTransition(string sceneName, string spawnID, Transform playerToMove)
    {
        return BeginTransitionInternal(sceneName, spawnID, playerToMove, null);
    }

    private bool BeginTransitionInternal(string sceneName, string spawnID, Transform playerToMove, Action afterSceneLoaded, bool isDeath = true)
    {
        if (isTransitioning)
            return false;

        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError("RoomTransitionManager: sceneName is empty.");
            return false;
        }

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError("RoomTransitionManager: Scene '" + sceneName + "' could not be loaded. " +
                "Make sure the name is correct and the scene is included in Build Settings.");
            return false;
        }

        if (fadeOverlayPrefab == null)
        {
            Debug.LogError("RoomTransitionManager: fadeOverlayPrefab is not assigned.");
            return false;
        }

        StartCoroutine(TransitionRoutine(sceneName, spawnID, playerToMove, afterSceneLoaded, isDeath));
        return true;
    }

    private IEnumerator TransitionRoutine(string sceneName, string spawnID, Transform playerToMove, Action afterSceneLoaded, bool isDeath = true)
    {
        isTransitioning = true;

        activeFadeOverlay = Instantiate(fadeOverlayPrefab);
        DontDestroyOnLoad(activeFadeOverlay.gameObject);

        Transform carriedPlayer = PreparePlayerForSceneLoad(playerToMove);

        yield return activeFadeOverlay.FadeToBlack(isDeath ? deathFadeOutDuration : fadeOutDuration);

        yield return new WaitForSeconds(0.6f);

        yield return LoadingScreenController.Instance.OpenScreen();

        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

        if (loadOperation == null)
        {
            Debug.LogError($"RoomTransitionManager: Failed to begin loading scene '" + sceneName + "'.");
            CleanupAfterFailedTransition(carriedPlayer);
            yield break;
        }

        while (!loadOperation.isDone)
        {
            float progress = Mathf.Clamp01(loadOperation.progress / 0.9f);
            LoadingScreenController.Instance.SetProgress(progress);
            yield return null;
        }

        LoadingScreenController.Instance.SetProgress(1f);

        // Allow Awake/OnEnable/Start-related scene setup to begin before placement.
        yield return null;

        Scene destinationScene = SceneManager.GetSceneByName(sceneName);

        if (!destinationScene.IsValid() || !destinationScene.isLoaded)
            destinationScene = SceneManager.GetActiveScene();

        if (carriedPlayer != null)
        {
            // The player survives only during loading. Moving it into the destination
            // scene prevents it from remaining permanently in DontDestroyOnLoad.
            SceneManager.MoveGameObjectToScene(carriedPlayer.gameObject, destinationScene);

            if (removeDuplicatePlayers)
                RemoveOtherPlayerRoots(carriedPlayer.gameObject);

            MovePlayerToSpawn(carriedPlayer, spawnID);
        }

        if (afterSceneLoaded != null)
        {
            try
            {
                afterSceneLoaded.Invoke();
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }

        yield return new WaitForSecondsRealtime(3f);

        /*if (holdBlackAfterSceneLoad > 0f)
            yield return new WaitForSecondsRealtime(holdBlackAfterSceneLoad);*/

        yield return LoadingScreenController.Instance.CloseScreen();

        yield return new WaitForSeconds(0.6f);

        yield return activeFadeOverlay.FadeFromBlack(fadeInDuration);

        Destroy(activeFadeOverlay.gameObject);
        activeFadeOverlay = null;
        isTransitioning = false;
    }

    private Transform PreparePlayerForSceneLoad(Transform playerToMove)
    {
        if (playerToMove == null)
            return null;

        // The object passed here should be the Player root. Detaching it ensures
        // DontDestroyOnLoad can preserve it even if it was under a scene container.
        playerToMove.SetParent(null, true);
        DontDestroyOnLoad(playerToMove.gameObject);
        return playerToMove;
    }

    private void MovePlayerToSpawn(Transform playerToMove, string spawnID)
    {
        if (playerToMove == null)
            return;

        if (string.IsNullOrWhiteSpace(spawnID))
        {
            Debug.LogWarning("RoomTransitionManager: A player was carried, but no spawn ID was supplied. " +
                "The player will keep its previous position.");
            return;
        }

        TransitionSpawnPoint[] spawnPoints = FindObjectsByType<TransitionSpawnPoint>
            (FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        foreach (TransitionSpawnPoint spawnPoint in spawnPoints)
        {
            if (!string.Equals(spawnPoint.SpawnID, spawnID, StringComparison.Ordinal))
                continue;

            playerToMove.SetPositionAndRotation(spawnPoint.transform.position, spawnPoint.transform.rotation);

            if (clearPlayerVelocityOnArrival)
            {
                Rigidbody2D body = playerToMove.GetComponent<Rigidbody2D>();

                if (body != null)
                {
                    body.linearVelocity = Vector2.zero;
                    body.angularVelocity = 0f;
                }
            }

            return;
        }

        Debug.LogWarning("RoomTransitionManager: No active TransitionSpawnPoint with SpawnID '" 
            + spawnID + "' " + "was found in scene '" + SceneManager.GetActiveScene().name + "'.");
    }

    private void RemoveOtherPlayerRoots(GameObject carriedPlayerRoot)
    {
        if (string.IsNullOrWhiteSpace(playerTag))
            return;

        GameObject[] taggedObjects;

        try
        {
            taggedObjects = GameObject.FindGameObjectsWithTag(playerTag);
        }
        catch (UnityException exception)
        {
            Debug.LogError(
                $"RoomTransitionManager: The tag '{playerTag}' does not exist.\n{exception.Message}"
            );
            return;
        }

        HashSet<GameObject> processedRoots = new HashSet<GameObject>();

        foreach (GameObject taggedObject in taggedObjects)
        {
            if (taggedObject == null)
                continue;

            GameObject root = taggedObject.transform.root.gameObject;

            if (root == carriedPlayerRoot || !processedRoots.Add(root))
                continue;

            Destroy(root);
        }
    }

    private void CleanupAfterFailedTransition(Transform carriedPlayer)
    {
        if (carriedPlayer != null)
        {
            Scene activeScene = SceneManager.GetActiveScene();

            if (activeScene.IsValid() && activeScene.isLoaded)
                SceneManager.MoveGameObjectToScene(carriedPlayer.gameObject, activeScene);
        }

        if (activeFadeOverlay != null)
        {
            Destroy(activeFadeOverlay.gameObject);
            activeFadeOverlay = null;
        }

        isTransitioning = false;
    }
}
