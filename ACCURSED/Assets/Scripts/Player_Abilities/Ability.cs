using UnityEngine;

public abstract class Ability : ScriptableObject
{
    public Sprite abilityIcon;

    public abstract void Trigger(ref PlayerReference playerRef);
}
