using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnableEnemy
{
    public GameObject prefab;
    public float spawnWeight;
}

[RequireComponent(typeof(Collider))]
public class EnemySpawnPoint : MonoBehaviour
{
    public static EnemySpawnPoint Instance { get; set; }
    [Header("Boss Settings")]
    public GameObject bossPrefab;
    private GameObject activeBoss;

    [Header("Minion Settings")]
    public List<SpawnableEnemy> minions;
    public int maxMinions = 5;
    public float minionSpawnRadius = 10f;
    private List<GameObject> activeMinions = new List<GameObject>();

    [Header("Spawn Locations")]
    public List<Transform> spawnLocations;

    [Header("Trigger Settings")]
    public string playerTag = "Player";

    void Reset()
    {
        // Make sure this collider is a trigger
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {

            if (activeBoss == null)
                SpawnBoss();

            SpawnMinions();
        }
    }

    private void SpawnBoss()
    {
        // Only spawn a boss if one isn't already alive
        if (activeBoss != null) return;

        Vector3 pos = PickSpawnPosition();
        activeBoss = Instantiate(bossPrefab, pos, Quaternion.identity);
        Debug.Log($"[BossSpawn] Boss spawned at {pos}");
    }

    private void SpawnMinions()
    {
        // Clean up any that have been destroyed already
        activeMinions.RemoveAll(e => e == null);

        // How many more we can spawn
        int toSpawn = maxMinions - activeMinions.Count;
        for (int i = 0; i < toSpawn; i++)
        {
            GameObject prefab = GetWeightedMinion();
            if (prefab == null) break;

            Vector3 basePos = PickSpawnPosition();
            Vector2 offset = Random.insideUnitCircle * minionSpawnRadius;
            Vector3 pos = basePos + new Vector3(offset.x, 0, offset.y);

            GameObject m = Instantiate(prefab, pos, Quaternion.identity);
            activeMinions.Add(m);
            Debug.Log($"[MinionSpawn] Spawned at {pos}");
        }
    }

    private GameObject GetWeightedMinion()
    {
        float total = 0f;
        foreach (var m in minions) total += m.spawnWeight;
        float roll = Random.Range(0f, total);
        float cum = 0f;

        foreach (var m in minions)
        {
            cum += m.spawnWeight;
            if (roll < cum) return m.prefab;
        }
        return null;
    }

    private Vector3 PickSpawnPosition()
    {
        if (spawnLocations != null && spawnLocations.Count > 0)
        {
            int idx = Random.Range(0, spawnLocations.Count);
            return spawnLocations[idx].position;
        }
        return transform.position;
    }
    public void ResetSpawnedEnemies()
    {
        if (activeBoss != null)
            Destroy(activeBoss);
        activeBoss = null;

        foreach (var m in activeMinions)
            if (m != null) Destroy(m);
        activeMinions.Clear();
    }

}
