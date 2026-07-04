using UnityEngine;

public class PersistentPlayer : MonoBehaviour
{
    public static PersistentPlayer Instance { get; private set; }

    private void Awake()
    {
        // If another persistent player already exists, remove this duplicate.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // This is the first player, so keep it forever.
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}