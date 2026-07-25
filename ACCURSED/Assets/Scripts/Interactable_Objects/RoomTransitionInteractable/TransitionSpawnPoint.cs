using UnityEngine;

public class TransitionSpawnPoint : MonoBehaviour
{
    [SerializeField] private string spawnID = "Entrance";
    [SerializeField] GameObject roomTransitionManager;
    [SerializeField] GameObject roomTransitionManagerPrefab;
    public string SpawnID => spawnID;

    private void Update()
    {
        if (roomTransitionManager != null)
        {
            roomTransitionManager = Instantiate(roomTransitionManagerPrefab);
            roomTransitionManager.SetActive(true);
        }
    }
}