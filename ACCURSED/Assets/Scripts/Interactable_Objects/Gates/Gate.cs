using UnityEngine;

public class Gate : MonoBehaviour
{
    [SerializeField] AbilityTriggeredSwitch[] abilityStatues;
    [SerializeField] StatueStateController[] seeingStatues;
    [SerializeField] EnemyDeath[] enemies;
    [SerializeField] bool startUnlocked;
    private bool unlocked;

    private void Start() {
        unlocked = startUnlocked;
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
                return;
            }
        }

        foreach (var enemy in enemies) {
            if (!enemy.IsDead()) {
                return;
            }
        }

        foreach (var statue in seeingStatues)
        {
            if (!statue.IsSeeing)
            {
                return;
            }
        }

        OpenWall();
        unlocked = true;
    }

    public void OpenWall() {
        Destroy(gameObject);
    }

    public bool isUnlocked() { return unlocked; }
}
