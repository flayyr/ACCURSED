using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeath : CharacterDeath
{
    public override void Die()
    {
        Debug.Log("Player Die");
    }

    public void Respawn(RespawnPointSO respawnData)
    {
        SceneManager.LoadScene(respawnData.respawnSceneName);
        transform.position = respawnData.respawnTransform.position;
    }
}
