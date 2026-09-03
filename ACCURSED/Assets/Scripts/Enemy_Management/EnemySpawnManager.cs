using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    public static EnemySpawnManager Instance;

    public event Action OnRespawnEnemies;


    private void Awake()
    {
        Instance = this;
    }

    public void RespawnEnemies()
    {
        OnRespawnEnemies?.Invoke();
    }
}
