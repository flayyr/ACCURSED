using UnityEngine;

[CreateAssetMenu(menuName = "Actions/Vestige/TestVestige")]
public class TestVestige : AbilitySO
{
    public override void Trigger(ref PlayerReference playerRef)
    {
        ParticleSystem.MainModule mainModule = playerRef.particleSystem.main;
        mainModule.startColor = Color.red;
        playerRef.particleSystem.Play();
    }
}
