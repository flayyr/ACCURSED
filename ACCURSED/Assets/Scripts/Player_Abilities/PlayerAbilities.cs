using System;
using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{
    [SerializeField] private AbilitySO vestigeAbility;
    [SerializeField] private AbilitySO remembranceAbility;
    [SerializeField] private ActionSO healAction;


    [HideInInspector] public event Action OnAbilityUsed;

    private ActionQueuer actionQueuer;
    private PlayerStatistics playerStatistics;

    private void Awake()
    {
        actionQueuer = GetComponent<ActionQueuer>();
        playerStatistics = GetComponent<PlayerStatistics>();
    }

    public void InitializeUI(AbilityUIDisplay vestigeDisplay, AbilityUIDisplay remembranceDisplay)
    {
        if(vestigeDisplay != null)
            vestigeDisplay.Initialize(vestigeAbility.abilityIcon);
        if(remembranceDisplay != null)
            remembranceDisplay.Initialize(remembranceAbility.abilityIcon);
    }

    public bool UseRemembrance()
    {
        actionQueuer.QueueAction(remembranceAbility);

        OnAbilityUsed?.Invoke();
        return true;
    }

    public bool UseVestige()
    {
        actionQueuer.QueueAction(vestigeAbility);

        OnAbilityUsed?.Invoke();
        return true;
    }

    public bool UseHeal()
    {
        if (playerStatistics.CanHeal())
        {
            actionQueuer.QueueAction(healAction);

            return true;
        }
        return false;
    }

}
