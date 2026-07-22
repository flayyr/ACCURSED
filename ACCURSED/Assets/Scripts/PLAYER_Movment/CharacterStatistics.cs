using System;
using UnityEngine;

public class CharacterStatistics : MonoBehaviour
{
    public event Action OnHealthUpdate;

    public float maxHealth;
    public float currentHealth;

    CharacterAnimator cAnimator;
    CharacterDeath cDeath;

    bool dead = false;

    protected virtual void Start()
    {
        cAnimator = GetComponent<CharacterAnimator>();
        cDeath = GetComponent<CharacterDeath>();

        Reset();

        OnHealthUpdate?.Invoke();
    }

    public float UpdateHealth(float healthChange)
    {
        if (dead) return currentHealth;

        currentHealth += healthChange;
        currentHealth = Mathf.Clamp(currentHealth,0, maxHealth);

        if(currentHealth == 0)
        {
            cDeath.Die();
            dead = true;
        }

        OnHealthUpdate?.Invoke();

        return currentHealth;
    }

    public virtual void Reset()
    {
        dead = false;
        UpdateHealth(maxHealth);
    }
}
