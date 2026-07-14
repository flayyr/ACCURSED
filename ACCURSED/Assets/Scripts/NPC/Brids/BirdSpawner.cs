using System.Collections.Generic;
using UnityEngine;

public class BirdSpawner : MonoBehaviour
{
    [Header("Bird Prefab")]
    [SerializeField] private GameObject birdPrefab;

    [Header("Group Size")]
    [Min(1)]
    [SerializeField] private int minBirds = 2;

    [Min(1)]
    [SerializeField] private int maxBirds = 4;

    [Header("Initial Spawn")]
    [SerializeField] private bool spawnOnStart = true;

    [Tooltip("require the player to be far away for the spawn")]
    [SerializeField] private bool requireDistanceForInitialSpawn = false;

    [Header("Spawn Position Mode")]
    [Tooltip("Enabled: use child locator objects.\n" + "Disabled: choose random positions inside the random spawn area.")]
    [SerializeField] private bool useChildLocators = true;

    [Header("Child Locator Settings")]
    [Tooltip("The direct children of this object are treated as bird spawn locators.")]
    [SerializeField] private Transform locatorParent;

    [Tooltip("Spawn one bird at every locator instead of using the random group size.")]
    [SerializeField] private bool useAllLocators = false;

    [Header("Random Area Settings")]
    [SerializeField] private Vector2 randomAreaSize = new Vector2(8f, 4f);
    [SerializeField] private Vector2 randomAreaOffset;

    [Tooltip("Minimum distance between birds spawned randomly.")]
    [Min(0f)]
    [SerializeField] private float minimumBirdSpacing = 0.5f;

    [Min(1)]
    [SerializeField] private int placementAttemptsPerBird = 10;

    [Header("Respawning")]
    [Min(0f)]
    [SerializeField] private float respawnDelay = 15f;

    [Min(0f)]
    [SerializeField] private float minimumPlayerDistance = 15f;

    [Header("Player")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private string playerTag = "Player";

    [Header("There is a disabled box collider below, it's for seeing the size of the spawn location.")]

    private readonly List<Transform> spawnLocators = new();
    private readonly List<GameObject> spawnedBirds = new();

    private bool waitingForRespawn;
    private float respawnTimer;
    private float playerSearchTimer;

    private void Awake()
    {
        RefreshLocators();
    }

    private void Start()
    {
        FindPlayer();

        if (!spawnOnStart)
        {
            BeginRespawnWait();
            return;
        }

        if (requireDistanceForInitialSpawn && !IsPlayerFarEnough())
        {
            BeginRespawnWait();
            return;
        }

        if (!TrySpawnGroup())
        {
            BeginRespawnWait();
            return;
        }
    }

    private void Update()
    {
        FindPlayer();

        if (!waitingForRespawn)
        {
            if (AreAllBirdsDisabled())
                BeginRespawnWait();

            return;
        }

        respawnTimer += Time.deltaTime;

        if (respawnTimer < respawnDelay)
            return;

        if (!IsPlayerFarEnough())
            return;

        if (TrySpawnGroup())
        {
            waitingForRespawn = false;
            respawnTimer = 0f;
        }
        else
        {
            respawnTimer = 0f;
        }
    }

    private bool TrySpawnGroup()
    {
        RemovePreviousBirds();

        if (useChildLocators)
            return SpawnAtLocators();

        return SpawnInRandomArea();
    }

    private bool SpawnAtLocators()
    {
        RefreshLocators();

        List<Transform> availableLocators = new List<Transform>(spawnLocators);
        ShuffleList(availableLocators);

        int birdCount;

        if (useAllLocators)
        {
            birdCount = availableLocators.Count;
        }
        else
        {
            birdCount = GetRandomGroupSize();
            birdCount = Mathf.Min(birdCount, availableLocators.Count);
        }

        for (int i = 0; i < birdCount; i++)
        {
            Transform locator = availableLocators[i];

            SpawnBird(locator.position, birdPrefab.transform.rotation);
        }

        return spawnedBirds.Count > 0;
    }

    private bool SpawnInRandomArea()
    {
        int birdCount = GetRandomGroupSize();
        List<Vector3> chosenPositions = new List<Vector3>();

        for (int i = 0; i < birdCount; i++)
        {
            Vector3 spawnPosition = FindRandomSpawnPosition(chosenPositions);
            chosenPositions.Add(spawnPosition);

            SpawnBird(spawnPosition, birdPrefab.transform.rotation);
        }

        return spawnedBirds.Count > 0;
    }

    private void SpawnBird(Vector3 position, Quaternion rotation)
    {
        GameObject newBird = Instantiate(birdPrefab, position, rotation);

        spawnedBirds.Add(newBird);
    }

    private Vector3 FindRandomSpawnPosition(List<Vector3> existingPositions)
    {
        Vector3 fallbackPosition = GetRandomPointInArea();

        for (int attempt = 0; attempt < placementAttemptsPerBird; attempt++)
        {
            Vector3 candidatePosition = GetRandomPointInArea();

            if (IsFarEnoughFromOtherBirds(candidatePosition, existingPositions))
            {
                return candidatePosition;
            }

            fallbackPosition = candidatePosition;
        }

        // If no perfectly spaced position was found, use the last attempted point.
        return fallbackPosition;
    }

    private Vector3 GetRandomPointInArea()
    {
        Vector2 center = (Vector2)transform.position + randomAreaOffset;

        float halfWidth = randomAreaSize.x * 0.5f;
        float halfHeight = randomAreaSize.y * 0.5f;

        float randomX = Random.Range(center.x - halfWidth, center.x + halfWidth);

        float randomY = Random.Range(center.y - halfHeight, center.y + halfHeight);

        return new Vector3(randomX, randomY, transform.position.z);
    }

    private bool IsFarEnoughFromOtherBirds(Vector3 candidatePosition, List<Vector3> existingPositions)
    {
        foreach (Vector3 existingPosition in existingPositions)
        {
            if (Vector2.Distance(candidatePosition, existingPosition) < minimumBirdSpacing)
                return false;
        }

        return true;
    }

    private int GetRandomGroupSize()
    {
        int validMinimum = Mathf.Max(1, minBirds);
        int validMaximum = Mathf.Max(validMinimum, maxBirds);

        // Integer Random.Range excludes the maximum, so add one.
        return Random.Range(validMinimum, validMaximum + 1);
    }

    private bool AreAllBirdsDisabled()
    {
        if (spawnedBirds.Count == 0)
            return true;

        foreach (GameObject bird in spawnedBirds)
        {
            if (bird != null && bird.activeSelf)
                return false;
        }

        return true;
    }

    private void BeginRespawnWait()
    {
        waitingForRespawn = true;
        respawnTimer = 0f;
    }

    private bool IsPlayerFarEnough()
    {
        // If the player has not been found yet, do not prevent spawning.
        if (playerTransform == null)
            return true;

        float distance = Vector2.Distance(transform.position, playerTransform.position);

        return distance >= minimumPlayerDistance;
    }

    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);

        if (playerObject != null)
            playerTransform = playerObject.transform;
    }

    private void RemovePreviousBirds()
    {
        foreach (GameObject bird in spawnedBirds)
        {
            if (bird != null)
                Destroy(bird);
        }

        spawnedBirds.Clear();
    }

    [ContextMenu("Refresh Spawn Locators")]
    public void RefreshLocators()
    {
        spawnLocators.Clear();

        Transform parent = locatorParent != null
            ? locatorParent
            : transform;

        foreach (Transform child in parent)
            spawnLocators.Add(child);
    }

    private static void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }

    private void OnValidate()
    {
        minBirds = Mathf.Max(1, minBirds);
        maxBirds = Mathf.Max(minBirds, maxBirds);

        randomAreaSize.x = Mathf.Max(0f, randomAreaSize.x);
        randomAreaSize.y = Mathf.Max(0f, randomAreaSize.y);

        respawnDelay = Mathf.Max(0f, respawnDelay);
        minimumPlayerDistance = Mathf.Max(0f, minimumPlayerDistance);
    } 
}