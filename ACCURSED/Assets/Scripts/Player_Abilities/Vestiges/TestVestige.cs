using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/TestVestige")]
public class TestVestige : Ability
{
    public override void Trigger(ref PlayerReference playerRef)
    {
        ParticleSystem.MainModule mainModule = playerRef.particleSystem.main;
        mainModule.startColor = Color.red;
        playerRef.particleSystem.Play();
    }
}
