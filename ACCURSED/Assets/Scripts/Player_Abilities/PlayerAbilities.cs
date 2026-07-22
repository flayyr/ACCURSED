using System;
using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{
    [SerializeField] private Ability vestigeAbility;
    [SerializeField] private Ability remembranceAbility;
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
        if (Input.GetKeyDown(KeyCode.R))
        {
            remembranceAbility.Trigger(ref playerRef);
            OnAbilityUsed?.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            vestigeAbility.Trigger(ref playerRef);
            OnAbilityUsed?.Invoke();
        }
    }
}
