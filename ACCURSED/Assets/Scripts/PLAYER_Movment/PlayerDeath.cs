using System;
using UnityEngine;

public class PlayerDeath : CharacterDeath
{
    public Action OnDeath;
    public Action OnRevive;

    [SerializeField] AspectSO respawnAspect;
    [SerializeField] float deathWaitBeforeFadeTime;
    [SerializeField] Collider2D hurtBoxCollider;

    public void SetRespawnAspect(AspectSO aspect)
    {
        respawnAspect = aspect;
    }

    public override void Die()
    {
        GetComponent<PlayerController>().SetState( PlayerControlState.Disabled);
        hurtBoxCollider.enabled = false;
        OnDeath?.Invoke();
        Invoke("StartRespawnTransition", deathWaitBeforeFadeTime);
    }

    public void StartRespawnTransition()
    {
        RoomTransitionManager.Instance.BeginTransition(respawnAspect.sceneName, ResetPlayerPosition, ResetPlayerStates, true);
    }

    private void ResetPlayerPosition() {
        transform.position = respawnAspect.position;
    }

    private void ResetPlayerStates()
    {
        hurtBoxCollider.enabled = true;
        GetComponent<PlayerController>().SetState( PlayerControlState.Normal);
        GetComponent<PlayerStatistics>().Reset();
        OnRevive?.Invoke();
    }
}
