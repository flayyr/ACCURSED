using UnityEngine;

[CreateAssetMenu(menuName = "Actions/Vestige/BaseVestige")]
public class VestigeSO : AbilitySO
{
    [Header("Vestige")]
    public float vestigeCoolDown = 1f;
    public override void PlayerActionTrigger(ref PlayerReference playerRef)
    {
        ParticleSystem.MainModule mainModule = playerRef.particleSystem.main;
        mainModule.startColor = Color.red;
        playerRef.particleSystem.Play();
    }
}
