using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawnManager : MonoBehaviour
{
    public List<EnemySpawnPoint> bossSpots;
    public float minInterval = 20f;
    public float maxInterval = 40f;

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    System.Collections.IEnumerator SpawnLoop()
    {
        while (true)
        {
            float wait = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(wait);

            foreach (var spot in bossSpots)
            {
                spot.TrySpawn();
            }
        }
    }
}
