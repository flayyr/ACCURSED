using UnityEngine;

[CreateAssetMenu(fileName = "TestRemembrance", menuName = "Actions/Remembrance/BaseRemembrance")]
public class RemembranceSO : AbilitySO
{
    [Header("Remembrance")]
    [SerializeField] public float requiredCharge;

    public override void PlayerActionTrigger(ref PlayerReference playerRef)
    {
        base.PlayerActionTrigger(ref playerRef);

        ParticleSystem.MainModule mainModule = playerRef.particleSystem.main;
        mainModule.startColor = Color.blue;
        playerRef.particleSystem.Play();
    }
}
