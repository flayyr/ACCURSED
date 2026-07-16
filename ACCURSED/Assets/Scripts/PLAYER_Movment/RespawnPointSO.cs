using UnityEngine;

[CreateAssetMenu(fileName = "RespawnPointSO", menuName = "Scriptable Objects/RespawnPointSO")]
public class RespawnPointSO : ScriptableObject
{
    public Transform respawnTransform;
    public string respawnSceneName;
}
