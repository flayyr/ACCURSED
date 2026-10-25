using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(menuName = "Actions/Vestige/AirSlashVestige")]
public class AirSlashVestige : VestigeSO
{
    [Header("Projectile")]
    public Projectile projectileObj;
    public float moveSpeed = 1f;
    public float lifetime = 1f;

    public override void PlayerActionTrigger(ref PlayerReference playerRef)
    {
        base.PlayerActionTrigger(ref playerRef);

        Vector2 position = playerRef.hitBox.transform.position;
        //Vector2 direction = playerRef.playerManager.GetDirection();
        Vector2 direction = playerRef.playerManager.GetMouseDirection();

        Projectile projectileInstance = Instantiate(projectileObj, position, Quaternion.identity);
        projectileInstance.Initialize(moveSpeed, lifetime, direction, attackData, playerRef.playerStats, playerRef.hitFeedback);
    }
}
