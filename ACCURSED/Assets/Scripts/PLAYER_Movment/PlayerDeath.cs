using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeath : CharacterDeath
{
    [SerializeField] AspectSO respawnAspect;
    [SerializeField] float deathWaitBeforeFadeTime;

    public void SetRespawnAspect(AspectSO aspect)
    {
        respawnAspect = aspect;
    }

    public override void Die()
    {
        GetComponent<PlayerController>().SetState( PlayerControlState.Disabled);
        Invoke("StartRespawnTransition", deathWaitBeforeFadeTime);
    }

    public void StartRespawnTransition()
    {
        //RoomTransitionWithoutPlayer.Instance.BeginTransition(respawnAspect.sceneName, ResetPlayer);
        RoomTransitionManager.Instance.BeginTransition(respawnAspect.sceneName, ResetPlayer);
    }

    private void ResetPlayer()
    {
        GetComponent<PlayerController>().SetState( PlayerControlState.Normal);
        transform.position = respawnAspect.position;
        GetComponent<PlayerStatistics>().Reset();
    }
}
