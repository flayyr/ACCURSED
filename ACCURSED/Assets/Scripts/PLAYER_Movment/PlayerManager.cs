using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] HurtBox hurtBox;
    [SerializeField] float healingInvincibleDuration = 1f;
    PlayerStatistics stats;

    private void Start()
    {
        stats = GetComponent<PlayerStatistics>();
    }


    //temporarily put this here until Lucas or I find a more appropriate place for it
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (stats.UseHealCharge())
            {
                hurtBox.InvinsibleForSeconds(healingInvincibleDuration);
            }
        }
    }
}
