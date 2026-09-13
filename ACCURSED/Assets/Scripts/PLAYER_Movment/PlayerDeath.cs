using System;
using UnityEngine;

public class PlayerDeath : CharacterDeath
{
    public Action OnDeath;
    public Action OnReviveAnimStarted;
    public Action OnReviveAnimFinished;

    [SerializeField] AspectSO respawnAspect;
    [SerializeField] float deathWaitBeforeFadeTime;
    [SerializeField] Collider2D hurtBoxCollider;
    [SerializeField] AnimationClip reviveClip;

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
        RoomTransitionManager.Instance.BeginTransition(respawnAspect.sceneName, ResetPlayerPosition, StartReviveAnimation, true);
    }

    private void ResetPlayerPosition() {
        transform.position = respawnAspect.position;
    }

    private void StartReviveAnimation()
    {
        OnReviveAnimStarted?.Invoke();
        Invoke("ResetPlayerStates", reviveClip.length);
    }

    private void ResetPlayerStates() {
        OnReviveAnimFinished?.Invoke();
        hurtBoxCollider.enabled = true;
        GetComponent<PlayerController>().SetState(PlayerControlState.Normal);
        GetComponent<PlayerStatistics>().Reset();
    }
}
