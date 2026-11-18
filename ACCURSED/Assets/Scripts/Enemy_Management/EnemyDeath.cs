using UnityEngine;

public class EnemyDeath : CharacterDeath
{
    [SerializeField] bool canRespawn = true;

    Vector3 startPosition;
    CharacterStatistics stats;

    bool dead = false;

    private void Start()
    {
        if (canRespawn) {
            startPosition = transform.position;
            stats = GetComponent<CharacterStatistics>();
            EnemySpawnManager.Instance.OnRespawnEnemies += Respawn;
        } else {
            bool alreadyDead = dead;
            if (SavesManager.Instance.TryLoadData("Enemy" + ID, out alreadyDead) && alreadyDead) {
                Die();
            }
        }
    }

    public override void Die()
    {
        dead = true;
        gameObject.SetActive(false);
        if (!canRespawn) {
            SavesManager.Instance.AddData("Enemy" + ID, true);
        }

        base.Die();
    }

    public void Respawn()
    {
        gameObject.SetActive(true);
        transform.position = startPosition;
        stats.Reset();
    }

    public bool IsDead() { return dead; }
}
