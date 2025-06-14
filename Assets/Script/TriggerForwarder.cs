using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerForwarder : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GameManager.instance.ChildTriggerEntered(other);
    }
}
