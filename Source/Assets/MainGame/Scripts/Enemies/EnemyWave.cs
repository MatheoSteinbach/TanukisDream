using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyWave : MonoBehaviour
{

    public bool HasEnded => hasEnded;
    public bool HasStarted => hasStarted;
    private List<AIController> enemies = new List<AIController>();
    private bool hasStarted = false;
    private bool hasEnded = false;

    private void Awake()
    {
        if (GetComponentsInChildren<EnemyWave>() != null)
        {
            var enemiesArray = GetComponentsInChildren<AIController>();
            enemies.AddRange(enemiesArray);
        }
    }
    private void Start()
    {
        foreach (var enemy in enemies)
        {
            enemy.gameObject.SetActive(false);
        }
    }
    private void Update()
    {
        var enemiesToRemove = new List<AIController>();
        if (enemies.Count > 0)
        {
            foreach (var enemy in enemies)
            {
                if (enemy.IsDead)
                {
                    enemiesToRemove.Add(enemy);
                }
            }
        }
        else if (enemies.Count == 0)
        {
            hasEnded = true;
        }
        foreach (var enemy in enemiesToRemove)
        {
            enemies.Remove(enemy);
        }
    }
    public void StartWave()
    {
        hasStarted = true;
        foreach (var enemy in enemies)
        {
            enemy.gameObject.SetActive(true);
            enemy.Spawn();
        }
    }
}
