using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileEnemy : MonoBehaviour
{
    public int damage = 10;
    public float lifeTime = 6f;

    [Header("Spawn on Hit (Chance Based)")]
    public bool spawnEnemyOnHit = false;
    [Range(0f, 1f)]
    public float spawnChance = 0.3f;
    public GameObject enemyPrefab;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Apply damage to player
            PlayerState.Instance.TakeDamage(damage);


            if (spawnEnemyOnHit && enemyPrefab != null)
            {
                float roll = Random.value; // Random number between 0.0 and 1.0
                if (roll <= spawnChance)
                {
                    Instantiate(enemyPrefab, transform.position, Quaternion.identity);
                    Debug.Log("Skeleton spawned! (roll: " + roll + ")");
                }
                else
                {
                    Debug.Log("Skeleton did NOT spawn (roll: " + roll + ")");
                }
            }

            Destroy(gameObject);
        }
        else if (!other.CompareTag("Enemy") && !other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}
