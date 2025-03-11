using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class village_CK : MonoBehaviour
{
    public CheckPoints goToVillage;

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
            goToVillage.isCompleted = true;
        }
    }
}
