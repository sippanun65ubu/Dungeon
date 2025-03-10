using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnLocation : MonoBehaviour
{
    private void RegisterLocation()
    {
        PlayerState.Instance.SpawnPlayerLocation(this);
    }
}
