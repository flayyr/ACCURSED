using System;
using UnityEngine;

public class CharacterDeath : MonoBehaviourID {
    public Action<CharacterDeath> OnCharacterDead;

    public virtual void Die()
    {
        OnCharacterDead?.Invoke(this);
    }
}
