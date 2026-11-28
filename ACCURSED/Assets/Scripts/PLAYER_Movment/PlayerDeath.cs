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

    PlayerData pData;

    private void Awake() {
        pData = GetComponent<PlayerData>();
    }

    public void SetRespawnAspect(AspectSO aspect)
    {
        respawnAspect = aspect;
    }

    public override void Die()
    {
        base.Die();
        PlayerController playerController = GetComponent<PlayerController>();
        if (playerController != null) {
            playerController.SetState(PlayerControlState.Disabled);
        }

        if(pData != null)
            pData.controlEnabled = false;

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
        if(GetComponent<PlayerController>() != null)
            GetComponent<PlayerController>().SetState(PlayerControlState.Normal);
        if(GetComponent<PlayerStatistics>() != null)
            GetComponent<PlayerStatistics>().Reset();

        if (pData != null)
            pData.Reset();
    }
}
