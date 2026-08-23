using System;
using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{
    [SerializeField] private VestigeSO vestigeAbility;
    [SerializeField] private RemembranceSO remembranceAbility;
    [SerializeField] private ActionSO healAction;
    [SerializeField] private ActionSO parryAction;


    [HideInInspector] public event Action OnAbilityUsed;

    private ActionQueuer actionQueuer;
    private PlayerStatistics playerStatistics;

    AbilityUIDisplay vestigeUI;
    AbilityUIDisplay remembranceUI;
    float vestigeCDTimer;

    private void Awake()
    {
        actionQueuer = GetComponent<ActionQueuer>();
        playerStatistics = GetComponent<PlayerStatistics>();
        playerStatistics.SetMaxRemembranceCharge(remembranceAbility.requiredCharge);
    }
    private void OnEnable()
    {
        playerStatistics.OnRemembranceChargeUpdate += UpdateRemembranceChargeUI;
    }
    private void OnDisable()
    {
        playerStatistics.OnRemembranceChargeUpdate -= UpdateRemembranceChargeUI;
    }

    public void InitializeUI(AbilityUIDisplay vestigeDisplay, AbilityUIDisplay remembranceDisplay)
    {
        if (vestigeDisplay != null)
        {
            vestigeUI = vestigeDisplay;

            vestigeUI.Initialize(vestigeAbility.abilityIcon);
            vestigeUI.SetFrameFill(1f);
            vestigeCDTimer = vestigeAbility.vestigeCoolDown;
        }
        if (remembranceDisplay != null)
        {
            remembranceUI = remembranceDisplay;

            remembranceUI.Initialize(remembranceAbility.abilityIcon);
            remembranceUI.SetFrameFill(1f);
        }
    }

    private void Update()
    {
        UpdateVestigeChargeUI();
    }

    void UpdateVestigeChargeUI()
    {
        if (vestigeCDTimer < vestigeAbility.vestigeCoolDown)
        {
            vestigeCDTimer += Time.deltaTime;
            vestigeCDTimer = MathF.Min(vestigeCDTimer, vestigeAbility.vestigeCoolDown);
            vestigeUI.SetFrameFill(vestigeCDTimer / vestigeAbility.vestigeCoolDown);
        }
    }

    void UpdateRemembranceChargeUI()
    {
        remembranceUI.SetFrameFill(playerStatistics.currentRemembranceCharge / remembranceAbility.requiredCharge);
    }




    public bool UseRemembrance()
    {
        if (playerStatistics.currentRemembranceCharge < remembranceAbility.requiredCharge) return false;

        actionQueuer.QueueAction(remembranceAbility);
        playerStatistics.UpdateRemembranceCharge(-remembranceAbility.requiredCharge);

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

    public bool UseParry()
    {
        actionQueuer.QueueAction(parryAction);
        return true;
    }

}
