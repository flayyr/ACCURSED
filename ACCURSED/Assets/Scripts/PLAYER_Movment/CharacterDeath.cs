using UnityEngine;

public class CharacterDeath : MonoBehaviour
{
    public virtual void Die()
    {
        Destroy(gameObject);
    }
}
