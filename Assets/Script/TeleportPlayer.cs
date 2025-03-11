using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPlayer : MonoBehaviour
{
    public bool playerTeleport;

    [SerializeField] private Vector3 teleportLocation;
    //[SerializeField] GameObject dungeon1;
    //[SerializeField] GameObject dungeon2;
    //[SerializeField] GameObject dungeon3;

    //Vector3 dungeon1Location;
    //Vector3 dungeon2Location;
    //Vector3 dungeon3Location;
    // Start is called before the first frame update
    //void Start()
    //{
    //    dungeon1Location = dungeon1.transform.position;
    //    dungeon2Location = dungeon2.transform.position;
    //    dungeon3Location = dungeon3.transform.position;

    //    playerTeleport = false;
    //}

    //Update is called once per frame
    //void Update()
    //{

    //}
    public void Teleport()
    {
        if (playerTeleport)
        {
            StartCoroutine(DelayTeleport(teleportLocation));
        }
    }

    public void SetTeleportLocation(Vector3 location)
    {
        teleportLocation = location;
    }
    void TeleportOurPlayer(Vector3  tpLocation)
    {
        gameObject.transform.position = tpLocation;
    }
    IEnumerator DelayTeleport(Vector3 tpLocation)
    {
        MovementManager.instance.EnableMovement(false); // Disable player movement
        yield return null; // Wait for one frame
        transform.position = tpLocation; // Teleport the player
        yield return null; // Wait for one frame
        MovementManager.instance.EnableMovement(true); // Re-enable player movement
        playerTeleport = false; // Reset the teleport flag
    }
    //IEnumerator DelayTeleport(Vector3 tpLocation)
    //{
    //    MovementManager.instance.EnableMovement(false);
    //    yield return null;
    //    TeleportOurPlayer(tpLocation);
    //    yield return null;
    //    MovementManager.instance.EnableMovement(true);
    //}
}
