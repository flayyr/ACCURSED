using System;
using UnityEngine;

public class GateTrigger : MonoBehaviour
{
    [SerializeField] AbilityTriggeredSwitch[] abilityStatues;
    [SerializeField] StatueStateController[] seeingStatues;
    [SerializeField] EnemyDeath[] enemies;
    [SerializeField] bool startActivated;
    [Header("Triggerables")]
    [SerializeReference] Triggerable[] triggerables;
    private bool unlocked;

    private void Start() {
        unlocked = startActivated;
        foreach (var statue in abilityStatues) {
            statue.OnStateChanged += CheckOpen;
        }

        foreach (var enemy in enemies) {
            enemy.OnCharacterDead += EnemyDeath;
        }

        foreach (var statue in seeingStatues)
        {
            statue.StateChanged += SeeingStatueChanged;
        }
    }

    private void OnDestroy() {
        foreach (var statue in abilityStatues) {
            statue.OnStateChanged -= CheckOpen;
        }
        foreach (var enemy in enemies) {
            enemy.OnCharacterDead -= EnemyDeath;
        }
        foreach (var statue in seeingStatues)
        {
            statue.StateChanged -= SeeingStatueChanged;
        }
    }

    private void SeeingStatueChanged(StatueVisionState state)
    {
        CheckOpen();
    }

    private void EnemyDeath(CharacterDeath cDeath) {
        CheckOpen();
    }

    private void CheckOpen() {
        foreach (var statue in abilityStatues) {
            if (!statue.IsActivated) {
                SetActivated(false);
                return;
            }
        }

        foreach (var enemy in enemies) {
            if (!enemy.IsDead()) {
                SetActivated(false);
                return;
            }
        }

        foreach (var statue in seeingStatues)
        {
            if (!statue.IsSeeing)
            {
                SetActivated(false);
                return;
            }
        }

        SetActivated(true);
    }

    public void SetActivated(bool value) {
        if (unlocked == value)
            return;

        foreach (var trigger in triggerables) {
            trigger.Trigger(value);
        }

        unlocked = value;
    }

    public bool isActivated() { return unlocked; }
}
