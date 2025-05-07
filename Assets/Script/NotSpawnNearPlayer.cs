using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotSpawnNearPlayer : MonoBehaviour
{
    public EnemySpawnerNearPlayer enemySpawner;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (enemySpawner != null)
            {
                enemySpawner.allowSpawning = false;
                Debug.Log("Player entered safe zone. Enemy spawning disabled.");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (enemySpawner != null)
            {
                enemySpawner.allowSpawning = true;
                Debug.Log("Player left safe zone. Enemy spawning enabled.");
            }
        }
    }
}
