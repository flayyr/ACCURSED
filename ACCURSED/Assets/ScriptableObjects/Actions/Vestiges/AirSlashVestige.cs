using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(menuName = "Actions/Vestige/AirSlashVestige")]
public class AirSlashVestige : AbilitySO
{
    [Header("Projectile")]
    public Projectile projectileObj;
    public float moveSpeed = 1f;
    public float lifetime = 1f;

    public override void PlayerActionTrigger(ref PlayerReference playerRef)
    {
        ParticleSystem.MainModule mainModule = playerRef.particleSystem.main;
        mainModule.startColor = Color.green;
        playerRef.particleSystem.Play();

        Vector2 position = playerRef.hitBox.transform.position;
        Vector2 direction = playerRef.playerManager.GetDirection();

        Projectile projectileInstance = Instantiate(projectileObj, position, Quaternion.identity);
        projectileInstance.Initialize(moveSpeed, lifetime, direction, attackData, playerRef.playerStats, playerRef.hitFeedback);
    }
}
