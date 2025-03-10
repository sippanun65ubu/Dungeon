using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTeleport : MonoBehaviour
{
    [Header("Teleport Settings")]
    [Tooltip("List of fixed teleport points. The script will choose one at random.")]
    public Transform[] teleportPoints;

    [Tooltip("Optional offset applied after selecting a random teleport point.")]
    public Vector3 teleportOffset;

    [Tooltip("Key to trigger teleportation (for testing purposes).")]
    public KeyCode teleportKey = KeyCode.T;

    [Header("Target Settings")]
    [Tooltip("The GameObject to teleport. If left empty, this GameObject will be teleported.")]
    public GameObject targetObject;

    // If targetObject is not set, default to this GameObject.
    private void Awake()
    {
        if (targetObject == null)
        {
            targetObject = this.gameObject;
        }
    }

    private void Update()
    {
        // For example, pressing the specified key triggers the teleport.
        if (Input.GetKeyDown(teleportKey))
        {
            TeleportToRandomPoint();
        }
    }

    public void TeleportToRandomPoint()
    {
        if (teleportPoints == null || teleportPoints.Length == 0)
        {
            Debug.LogWarning("No teleport points set!");
            return;
        }

        // Choose a random index from the teleportPoints array.
        int randomIndex = Random.Range(0, teleportPoints.Length);
        Transform chosenPoint = teleportPoints[randomIndex];

        // Calculate the new position by adding the optional offset.
        Vector3 newPosition = chosenPoint.position + teleportOffset;

        // Teleport the target object.
        targetObject.transform.position = newPosition;
    }
}
