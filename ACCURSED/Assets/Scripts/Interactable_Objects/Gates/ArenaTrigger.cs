using UnityEngine;

public class ArenaTrigger : MonoBehaviour
{
    [SerializeField] Triggerable gateTriggerable;
    [SerializeField] EnemyDeath bossEnemy;

    private void Start() {
        bossEnemy.OnCharacterDead += OpenGate;
    }

    private void OnDestroy() {
        bossEnemy.OnCharacterDead -= OpenGate;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.tag == "Player" && !bossEnemy.IsDead()) {
            gateTriggerable.Trigger(false);
        }
    }

    private void OpenGate(CharacterDeath cDeath) {
        gateTriggerable.Trigger(true);
    }
}
