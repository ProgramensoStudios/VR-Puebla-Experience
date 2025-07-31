using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCountManager : MonoBehaviour
{
    private List<Enemy> enemies = new List<Enemy>();
    private WaveManager waveManager;

    private void Awake()
    {
        FindEnemies();
    }

    public void FindEnemies()
    {
        enemies.Clear();
        var type = FindObjectsOfType<Enemy>();
        for (var index = 0; index < type.Length; index++)
        {
            var enemy = type[index];
            AddEnemy(enemy);
        }
    }

    private void AddEnemy(Enemy enemy)
    {
        if (!enemies.Contains(enemy))
        {
            enemies.Add(enemy);
            enemy.OnDeath += HandleEnemyDeath;
        }
    }

    private void HandleEnemyDeath(Enemy deadEnemy)
    {
        RemoveEnemy(deadEnemy);
    }

    private void RemoveEnemy(Enemy enem)
    {
        if (!enemies.Contains(enem)) return;
        enemies.Remove(enem);
        Debug.Log($"Enemy removed. Remaining: {enemies.Count}");

        if (enemies.Count == 0)
        {
            Debug.Log("Wave complete!");
            waveManager.WaveEnded();
        }
    }
}
