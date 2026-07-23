using UnityEngine;

public class EnemyDeath : CharacterDeath
{
    Vector3 startPosition;
    CharacterStatistics stats;

    private void Start()
    {
        startPosition = transform.position;
        stats = GetComponent<CharacterStatistics>();
        EnemySpawnManager.Instance.OnRespawnEnemies += Respawn;
    }

    public override void Die()
    {
        gameObject.SetActive(false);
    }

    public void Respawn()
    {
        gameObject.SetActive(true);
        transform.position = startPosition;
        stats.Reset();
    }
}
