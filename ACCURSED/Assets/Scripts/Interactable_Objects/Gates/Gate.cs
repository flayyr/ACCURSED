using UnityEngine;

public class Gate : MonoBehaviour
{
    [SerializeField] AbilityTriggeredSwitch[] statues;
    [SerializeField] EnemyDeath[] enemies;
    [SerializeField] bool startUnlocked;
    private bool unlocked;

    private void Start() {
        unlocked = startUnlocked;
        foreach (var statue in statues) {
            statue.OnStateChanged += CheckOpen;
        }

        foreach (var enemy in enemies) {
            enemy.OnCharacterDead += EnemyDeath;
        }
    }

    private void OnDestroy() {
        foreach (var statue in statues) {
            statue.OnStateChanged -= CheckOpen;
        }
        foreach (var enemy in enemies) {
            enemy.OnCharacterDead -= EnemyDeath;
        }
    }

    private void EnemyDeath(CharacterDeath cDeath) {
        CheckOpen();
    }

    private void CheckOpen() {
        foreach (var statue in statues) {
            if (!statue.IsActivated) {
                return;
            }
        }

        foreach (var enemy in enemies) {
            if (!enemy.IsDead()) {
                Debug.Log("not dead????");
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
