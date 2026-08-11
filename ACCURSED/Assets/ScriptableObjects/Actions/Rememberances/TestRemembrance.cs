using UnityEngine;

[CreateAssetMenu(fileName = "TestRemembrance", menuName = "Actions/Remembrance/TestRemembrance")]
public class TestRemembrance : AbilitySO
{
    public override void Trigger(ref PlayerReference playerRef)
    {
        ParticleSystem.MainModule mainModule = playerRef.particleSystem.main;
        mainModule.startColor = Color.blue;
        playerRef.particleSystem.Play();
    }
}
