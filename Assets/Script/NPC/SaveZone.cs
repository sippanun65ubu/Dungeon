using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveZone : MonoBehaviour
{
    public EnemySpawnerNearPlayer spawner;
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            spawner.allowSpawning = false;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            spawner.allowSpawning = true;
        }
    }
}
