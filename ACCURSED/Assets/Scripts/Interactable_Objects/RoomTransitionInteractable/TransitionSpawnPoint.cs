using UnityEngine;

public class TransitionSpawnPoint : MonoBehaviour
{
    [SerializeField] private string spawnID = "Entrance";

    public string SpawnID => spawnID;

    private void OnValidate()
    {
        spawnID = spawnID.Trim();
    }
}
