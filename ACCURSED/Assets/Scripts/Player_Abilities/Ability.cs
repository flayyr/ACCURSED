using System;
using UnityEngine;

[Serializable]
public struct PlayerReference
{
    public ParticleSystem particleSystem;
    public SpriteRenderer spriteRenderer;
}

public abstract class Ability : ScriptableObject
{
    public Sprite abilityIcon;

    public abstract void Trigger(ref PlayerReference playerRef);
}
