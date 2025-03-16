using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerForwarder : MonoBehaviour
{

    [Tooltip("Reference to the parent's TotheNextStage script.")]
    public TotheNextStage parentScript;

    private void OnTriggerEnter(Collider other)
    {
        // Forward the event to the parent.
        parentScript.ChildTriggerEntered(other);
    }
}
