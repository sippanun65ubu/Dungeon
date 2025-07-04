using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerForwarder : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        GameManager.instance.ChildTriggerEntered(other);
    }
}
