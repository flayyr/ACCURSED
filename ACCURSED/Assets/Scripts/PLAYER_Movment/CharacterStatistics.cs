using System;
using UnityEngine;

public class CharacterStatistics : MonoBehaviour
{
    public event Action OnHealthUpdate;

    public float maxHealth;
    public float currentHealth;

    CharacterAnimator cAnimator;
    CharacterDeath cDeath;

    protected virtual void Start()
    {
        cAnimator = GetComponent<CharacterAnimator>();
        cDeath = GetComponent<CharacterDeath>();
        currentHealth = maxHealth;
    }

    public float UpdateHealth(float healthChange)
    {
        currentHealth += healthChange;
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        if(currentHealth <= 0)
        {
            cDeath.Die();
        }

        OnHealthUpdate?.Invoke();

        return currentHealth;
    }
}
