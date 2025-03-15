using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckEnemiesInCollider : MonoBehaviour
{
    [Tooltip("The GameObject to set active when no enemies are inside the collider.")]
    public GameObject objectToActivate;

    // Track all enemies currently in the trigger
    private HashSet<Collider> enemiesInside = new HashSet<Collider>();

    private void OnTriggerEnter(Collider other)
    {
        // If an Enemy enters, add it to the set
        if (other.CompareTag("Enemy"))
        {
            enemiesInside.Add(other);
            // Since at least one enemy is inside, ensure object is inactive
            if (objectToActivate != null)
            {
                objectToActivate.SetActive(false);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // If an Enemy leaves, remove it from the set
        if (other.CompareTag("Enemy"))
        {
            enemiesInside.Remove(other);

            // If no enemies remain, set the object active
            if (enemiesInside.Count == 0 && objectToActivate != null)
            {
                objectToActivate.SetActive(true);
            }
        }
    }
}
