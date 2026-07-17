using UnityEngine;

[CreateAssetMenu(fileName = "TestRemembrance", menuName = "Abilities/TestRemembrance")]
public class TestRemembrance : Ability
{
    public override void Trigger(ref PlayerReference playerRef)
    {
        ParticleSystem.MainModule mainModule = playerRef.particleSystem.main;
        mainModule.startColor = Color.blue;
        playerRef.particleSystem.Play();
    }
}
