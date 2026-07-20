using System;
using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{
    [SerializeField] private Ability vestigeAbility;
    [SerializeField] private Ability remembranceAbility;
    [Space]
    [SerializeField] private AbilityUIDisplay vestigeDisplay;
    [SerializeField] private AbilityUIDisplay remembranceDisplay;
    [Space]
    [SerializeField] PlayerReference playerRef;

    [HideInInspector] public event Action OnAbilityUsed;

    private void Start()
    {
        if(vestigeDisplay != null)
            vestigeDisplay.Instanciate(vestigeAbility.abilityIcon);
        if(remembranceDisplay != null)
            remembranceDisplay.Instanciate(remembranceAbility.abilityIcon);
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
