using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]

public class SpawnableEnemy
{
    public GameObject prefab;
    public float spawnWeight;
}
public class EnemySpawnPoint : MonoBehaviour
{
    [Header("Boss Settings")]
    public GameObject bossPrefab;
    public float bossSpawnChance = 0.3f; // 30% chance to spawn
    private GameObject activeBoss;

    [Header("Minion Settings")]
    public List<SpawnableEnemy> minions;
    public int maxMinions = 5;
    public float minionSpawnRadius = 10f;

    private List<GameObject> activeMinions = new List<GameObject>();

    public void TrySpawn()
    {
        TrySpawnBoss();
        TrySpawnMinions();
    }

    void TrySpawnBoss()
    {
        if (activeBoss != null) return;

        if (Random.value <= bossSpawnChance)
        {
            activeBoss = Instantiate(bossPrefab, transform.position, Quaternion.identity);
            Debug.Log($"[BossSpawn] Boss spawned at {transform.name}");
        }
        else
        {
            Debug.Log($"[BossSpawn] Boss spawn skipped at {transform.name}");
        }
    }

    void TrySpawnMinions()
    {
        activeMinions.RemoveAll(e => e == null);

        if (activeMinions.Count >= maxMinions) return;

        GameObject selectedMinion = GetWeightedMinion();
        if (selectedMinion == null) return;

        Vector2 offset = Random.insideUnitCircle * minionSpawnRadius;
        Vector3 spawnPos = transform.position + new Vector3(offset.x, 0, offset.y);
        GameObject minion = Instantiate(selectedMinion, spawnPos, Quaternion.identity);
        activeMinions.Add(minion);

        Debug.Log($"[BossSpawn] Minion spawned near {transform.name}");
    }

    GameObject GetWeightedMinion()
    {
        float totalWeight = 0f;
        foreach (var m in minions)
            totalWeight += m.spawnWeight;

        float roll = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var m in minions)
        {
            cumulative += m.spawnWeight;
            if (roll < cumulative)
                return m.prefab;
        }

        return null;
    }
}
