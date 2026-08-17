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

    AbilityUIDisplay vestigeUI;
    float vestigeCDTimer;

    private void Awake()
    {
        actionQueuer = GetComponent<ActionQueuer>();
        playerStatistics = GetComponent<PlayerStatistics>();
    }

    public void InitializeUI(AbilityUIDisplay vestigeDisplay, AbilityUIDisplay remembranceDisplay)
    {
        if (vestigeDisplay != null)
        {
            vestigeDisplay.Initialize(vestigeAbility.abilityIcon);
            vestigeDisplay.SetFrameFill(1f);
            vestigeCDTimer = vestigeAbility.vestigeCoolDown;

            vestigeUI = vestigeDisplay;
        }
        if(remembranceDisplay != null)
            remembranceDisplay.Initialize(remembranceAbility.abilityIcon);
    }

    private void Update()
    {
        if (vestigeCDTimer < vestigeAbility.vestigeCoolDown)
        {
            vestigeCDTimer += Time.deltaTime;
            vestigeCDTimer = MathF.Min(vestigeCDTimer, vestigeAbility.vestigeCoolDown);
            vestigeUI.SetFrameFill(vestigeCDTimer/vestigeAbility.vestigeCoolDown);
        }
    }

    public bool UseRemembrance()
    {
        actionQueuer.QueueAction(remembranceAbility);

        OnAbilityUsed?.Invoke();
        return true;
    }

    public bool UseVestige()
    {
        if (vestigeCDTimer < vestigeAbility.vestigeCoolDown) return false;

        actionQueuer.QueueAction(vestigeAbility);
        vestigeCDTimer = 0f;

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
