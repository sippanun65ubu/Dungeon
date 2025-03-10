using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixPositionSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [Tooltip("Prefab to spawn")]
    public GameObject prefabToSpawn;

    [Tooltip("Total number of objects to spawn")]
    public int numberToSpawn = 10;

    [Tooltip("Delay between spawns (in seconds)")]
    public float spawnInterval = 0.5f;

    [Header("Spawn Points")]
    [Tooltip("Fixed spawn point positions. The script will choose one at random for each spawn.")]
    public Transform[] spawnPoints;

    private int spawnedCount = 0;
    private bool hasStartedSpawning = false;

    // Spawning starts only when the player enters the trigger collider
    private void OnTriggerEnter(Collider other)
    {
        if (!hasStartedSpawning && other.CompareTag("Player"))
        {
            hasStartedSpawning = true;
            StartCoroutine(SpawnObjects());
        }
    }

    IEnumerator SpawnObjects()
    {
        // Continue spawning until we reach the desired number
        while (spawnedCount < numberToSpawn)
        {
            if (spawnPoints.Length > 0 && prefabToSpawn != null)
            {
                // Choose a random spawn point from the array
                int randomIndex = Random.Range(0, spawnPoints.Length);
                Transform chosenSpawnPoint = spawnPoints[randomIndex];

                // Instantiate the prefab at the chosen position and rotation
                Instantiate(prefabToSpawn, chosenSpawnPoint.position, chosenSpawnPoint.rotation);

                spawnedCount++;
            }
            else
            {
                Debug.LogWarning("No spawn points or prefab assigned!");
                yield break;
            }

            // Wait for the specified interval before spawning the next object
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
