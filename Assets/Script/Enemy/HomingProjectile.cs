using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingProjectile : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 10f;
    public float rotateSpeed = 200f;
    public float lifeTime = 6f;

    [Header("Damage")]
    public int damage = 10;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;
        Destroy(gameObject, lifeTime);
    }

    void FixedUpdate()
    {
        // Always steer toward the player
        Vector3 targetPos = AccessPo.Instance.PlayerPosition;
        Vector3 direction = (targetPos - transform.position).normalized;

        float maxRadians = rotateSpeed * Mathf.Deg2Rad * Time.fixedDeltaTime;
        Vector3 newDir = Vector3.RotateTowards(transform.forward, direction, maxRadians, 0f);
        rb.MoveRotation(Quaternion.LookRotation(newDir));

        // Keep moving forward
        rb.velocity = transform.forward * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Apply damage just like ProjectileEnemy does
            PlayerState.Instance.TakeDamage(damage);
            Destroy(gameObject);
        }
        else if (!other.CompareTag("Enemy") && !other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}
