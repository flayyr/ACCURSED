using UnityEngine;

public class EnemyDeath : CharacterDeath
{
    Vector3 startPosition;
    CharacterStatistics stats;

    bool dead = false;

    private void Start()
    {
        startPosition = transform.position;
        stats = GetComponent<CharacterStatistics>();
        EnemySpawnManager.Instance.OnRespawnEnemies += Respawn;
    }

    public override void Die()
    {
        dead = true;
        gameObject.SetActive(false);
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
