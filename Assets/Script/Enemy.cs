using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Chase Settings")]
    public float speed = 5f; // Enemy movement speed
    public float chaseRange = 10f; // Range to start chasing the player
    public Transform player; // Reference to the player's transform

    [Header("Damage Settings")]
    public int contactDamage = 10; // Damage dealt to the enemy on contact

    private bool isChasing = false;

    void Start()
    {
        currentHealth = maxHealth; // Initialize health
    }

    void Update()
    {
        // Check if the player is within chase range
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= chaseRange)
        {
            isChasing = true;
        }
        else
        {
            isChasing = false;
        }

        // Chase the player if within range
        if (isChasing)
        {
            ChasePlayer();
        }
    }

    void ChasePlayer()
    {
        // Move towards the player's position
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        // Optional: Rotate to face the player
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"Enemy took {damage} damage. Current health: {currentHealth}");

        // Check if the enemy has died
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Enemy died!");
        // Play death animation (optional)
        // Destroy the enemy GameObject
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the collision is with the player
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerStats playerStats = collision.gameObject.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.TakeDamage(damageToPlayer);
            }
            TakeDamage(contactDamage); // Reduce enemy health on player contact
        }
    }
    [Header("Damage Settings")]
    public float damageToPlayer = 10f; // Damage dealt to the player on contact

}
