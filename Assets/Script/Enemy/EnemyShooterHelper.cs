using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooterHelper : MonoBehaviour
{
    public GameObject projectilePrefab;
    public int forwardforce;
    public Transform firePoint;
    public int upforce;

    public void Shoot()
    {
        Rigidbody rb = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation).GetComponent<Rigidbody>(); 
        rb.AddForce(firePoint.forward * forwardforce, ForceMode.Impulse);
        rb.AddForce(firePoint.up * upforce, ForceMode.Impulse);
    }
}
