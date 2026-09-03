using System;
using UnityEngine;

public class PlayerDeath : CharacterDeath
{
    public Action OnDeath;
    public Action OnRevive;

    [SerializeField] AspectSO respawnAspect;
    [SerializeField] float deathWaitBeforeFadeTime;

    public void SetRespawnAspect(AspectSO aspect)
    {
        respawnAspect = aspect;
    }

    public override void Die()
    {
        GetComponent<PlayerController>().SetState( PlayerControlState.Disabled);
        OnDeath?.Invoke();
        Invoke("StartRespawnTransition", deathWaitBeforeFadeTime);
    }

    public void StartRespawnTransition()
    {
        RoomTransitionManager.Instance.BeginTransition(respawnAspect.sceneName, ResetPlayer);
    }

    private void ResetPlayer()
    {
        GetComponent<PlayerController>().SetState( PlayerControlState.Normal);
        transform.position = respawnAspect.position;
        GetComponent<PlayerStatistics>().Reset();
        OnRevive?.Invoke();
    }
}
