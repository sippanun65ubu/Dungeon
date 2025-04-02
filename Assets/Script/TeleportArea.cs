using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportArea : MonoBehaviour
{
    public GameObject tpPoint; // The GameObject representing the teleport destination

    private Vector3 tpPosition; // The position to teleport to
    private TeleportPlayer player; // Reference to the player's TeleportPlayer script

    private void Start()
    {
        tpPosition = tpPoint.transform.position; // Store the teleport position
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            // Get the TeleportPlayer component from the player
            player = other.GetComponent<TeleportPlayer>();

            if (player != null)
            {
                StartCoroutine(DelayTeleport(tpPosition));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Reset the teleport flag when the player leaves the area
            if (player != null)
            {
                player.playerTeleport = false;
            }
        }
    }
    IEnumerator DelayTeleport(Vector3 tpPosition)
    {
        MovementManager.instance.EnableLook(false);
        MovementManager.instance.EnableMovement(false);
        yield return null;
        // Set the teleport location and flag
        player.SetTeleportLocation(tpPosition);
        player.playerTeleport = true;

        // Trigger the teleportation
        player.Teleport();
        yield return null;
        MovementManager.instance.EnableLook(true);
        MovementManager.instance.EnableMovement(true);
    }
}
