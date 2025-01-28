using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Animator animator;                // Reference to Animator for attack animations
    public Transform attackPoint;           // Point where the attack is checked
    public float attackRange = 1f;          // Range of the attack
    public LayerMask enemyLayers;           // Layer mask for enemies
    public int attackDamage = 20;           // Damage dealt per attack
    public float attackRate = 1f;           // Time between attacks
    private float nextAttackTime = 0f;      // Timer for next attack

    void Update()
    {
        // Check if the player presses the attack button
        if (Input.GetButtonDown("Fire1") && Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + 1f / attackRate;
            Attack();
        }
    }

    void Attack()
    {
        // Play the attack animation
        animator.SetTrigger("Attack");

        // Detect enemies within range of the attack
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

        // Apply damage to each enemy
        foreach (Collider enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the attack range in the editor
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
