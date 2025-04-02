using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class EnemySpawnData
{
    public GameObject prefab;
    public float spawnWeight; // Higher = more likely
}
public class EnemySpawnerNearPlayer : MonoBehaviour
{
    public List<EnemySpawnData> enemySpawnTable;
    public Transform player;

    public float minSpawnInterval = 5f;
    public float maxSpawnInterval = 10f;
    public float spawnRadius = 10f;
    public int maxActiveEnemies = 5;

    private List<GameObject> activeEnemies = new List<GameObject>();

    private void Start()
    {
        StartCoroutine(SpawnEnemiesAtRandom());
    }

    private IEnumerator SpawnEnemiesAtRandom()
    {
        while (true)
        {
            float waitTime = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(waitTime);

            activeEnemies.RemoveAll(e => e == null);

            if (activeEnemies.Count < maxActiveEnemies)
            {
                SpawnEnemyNearPlayer();
            }
        }
    }

    void SpawnEnemyNearPlayer()
    {
        GameObject enemyToSpawn = GetWeightedRandomEnemy();

        if (enemyToSpawn == null || player == null)
            return;

        Vector2 offset = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPos = player.position + new Vector3(offset.x, 0, offset.y);
        spawnPos.y = 0f;

        GameObject newEnemy = Instantiate(enemyToSpawn, spawnPos, Quaternion.identity);
        activeEnemies.Add(newEnemy);
    }

    GameObject GetWeightedRandomEnemy()
    {
        float totalWeight = 0f;
        foreach (var enemy in enemySpawnTable)
        {
            totalWeight += enemy.spawnWeight;
        }

        float roll = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var enemy in enemySpawnTable)
        {
            cumulative += enemy.spawnWeight;
            if (roll < cumulative)
            {
                return enemy.prefab;
            }
        }

        return null; // fallback
    }
}
