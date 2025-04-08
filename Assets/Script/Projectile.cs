using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody rb;
    public int projectileDamage = 25;
    public int projectilePenetration = 0;

    private bool isStuck = false; // Flag to track if the arrow is stuck
    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        Destroy(gameObject, 20f); // Destroy the arrow after 5 seconds if it doesn't hit anything

        Collider arrowCollider = GetComponent<Collider>();
        Collider bodyCollider = PlayerState.Instance.playerBody.transform.Find("Character").GetComponent<Collider>();
        Collider playerCollider = PlayerState.Instance.playerBody.GetComponent<Collider>();

        if (arrowCollider != null && bodyCollider != null)
        {
            Physics.IgnoreCollision(arrowCollider, playerCollider);
            Physics.IgnoreCollision(arrowCollider, bodyCollider);
        }
    }

    // This method is called when the arrow hits a collider
    private void OnCollisionEnter(Collision collision)
    {
        // Check if the arrow is not already stuck
        if (!isStuck && !collision.transform.CompareTag("Player"))
        {
            isStuck = true; // Mark the arrow as stuck

            // Stop the movement by setting the Rigidbody's velocity and angular velocity to zero
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // Optionally, freeze the Rigidbody's movement and rotation completely
            rb.isKinematic = true;

            Debug.Log("Arrow stuck! :" + collision.transform.name);
        }

        if (collision.transform.GetComponent<Enemy>())
        {
            Enemy enemy = collision.transform.GetComponent<Enemy>();
            enemy.TakeDamage(projectileDamage, projectilePenetration);
        }
    }
}
