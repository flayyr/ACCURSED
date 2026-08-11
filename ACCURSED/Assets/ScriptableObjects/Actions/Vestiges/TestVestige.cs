using UnityEngine;

[CreateAssetMenu(menuName = "Actions/Vestige/TestVestige")]
public class TestVestige : AbilitySO
{
    public override void PlayerActionTrigger(ref PlayerReference playerRef)
    {
        ParticleSystem.MainModule mainModule = playerRef.particleSystem.main;
        mainModule.startColor = Color.red;
        playerRef.particleSystem.Play();
    }
}
