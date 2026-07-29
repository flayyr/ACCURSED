using System;
using UnityEngine;

[Serializable]
public struct PlayerReference
{
    public ParticleSystem particleSystem;
    public SpriteRenderer spriteRenderer;
    public HurtBox hurtBox;
    public PlayerStatistics playerStats;
}

public class PlayerAbilities : MonoBehaviour
{
    [SerializeField] private Ability vestigeAbility;
    [SerializeField] private Ability remembranceAbility;
    [SerializeField] float healingInvincibleDuration = 1f;
    [Space]
    [SerializeField] PlayerReference playerRef;


    [HideInInspector] public event Action OnAbilityUsed;


    public void InitializeUI(AbilityUIDisplay vestigeDisplay, AbilityUIDisplay remembranceDisplay)
    {
        if(vestigeDisplay != null)
            vestigeDisplay.Initialize(vestigeAbility.abilityIcon);
        if(remembranceDisplay != null)
            remembranceDisplay.Initialize(remembranceAbility.abilityIcon);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (playerRef.playerStats.UseHealCharge())
            {
                playerRef.hurtBox.InvincibleForSeconds(healingInvincibleDuration);
            }
        }
    }

    public bool UseRemembrance()
    {
        remembranceAbility.Trigger(ref playerRef);
        OnAbilityUsed?.Invoke();
        return true;
    }

    public bool UseVestige()
    {
        vestigeAbility.Trigger(ref playerRef);
        OnAbilityUsed?.Invoke();
        return true;
    }

    public bool UseHeal()
    {
        if (playerRef.playerStats.UseHealCharge())
        {
            playerRef.hurtBox.InvincibleForSeconds(healingInvincibleDuration);
            return true;
        }
        return false;
    }

}
