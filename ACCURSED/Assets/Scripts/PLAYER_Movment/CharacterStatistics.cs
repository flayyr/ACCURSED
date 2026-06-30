using UnityEngine;

public class CharacterStatistics : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;

    CharacterAnimator cAnimator;

    void Start()
    {
        cAnimator = GetComponent<CharacterAnimator>();
    }
}
