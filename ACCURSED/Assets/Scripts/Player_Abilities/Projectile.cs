using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Windows;

public class Projectile : MonoBehaviour
{
    [SerializeField] HitBox hitBox;

    float moveSpeed = 1f;
    float lifetime = 1f;
    Vector2 direction = Vector2.right;

    float timer;

    private void Awake()
    {
        timer = lifetime;
    }

    public void Initialize(float speed, float lifetime, Vector2 direction, AttackData attackData, PlayerStatistics playerStats, MMF_Player hitFeedback)
    {
        moveSpeed = speed;
        this.lifetime = lifetime;
        this.direction = direction;

        hitBox.SetAttackData(attackData);
        hitBox.SetPlayerStats(playerStats);
        hitBox.SetHitFeedback(hitFeedback);
    }

    private void Update()
    {
        if (timer <= 0)
        {
            Destroy(gameObject);
        }
        timer-=Time.deltaTime;

        transform.rotation = Quaternion.Euler(0,0, Mathf.Atan2(direction.x,direction.y) * Mathf.Rad2Deg);
        transform.position += (Vector3)direction.normalized * moveSpeed * Time.deltaTime;
    }
}
