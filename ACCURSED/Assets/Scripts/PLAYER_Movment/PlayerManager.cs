using System;
using UnityEngine;

[Serializable]
public struct PlayerReference
{
    public ParticleSystem particleSystem;
    public SpriteRenderer spriteRenderer;
    public HurtBox hurtBox;
    public PlayerStatistics playerStats;
}

public class PlayerManager : CharacterManager
{
    [SerializeField] PlayerReference playerRef;

    protected override void EndWind()
    {
        base.EndWind();

        currAction.actionSO.Trigger(ref playerRef);
    }
}
