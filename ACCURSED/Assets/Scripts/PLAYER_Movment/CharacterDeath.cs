using System;
using UnityEngine;

public class CharacterDeath : MonoBehaviour
{
    public Action<CharacterDeath> OnCharacterDead;

    public virtual void Die()
    {
        OnCharacterDead?.Invoke(this);
    }
}
