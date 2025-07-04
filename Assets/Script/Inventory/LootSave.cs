using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootSave : MonoBehaviour
{
    public string lootId;
    public string lootName;

    public void Awake()
    {
        // 1) Generate a GUID once, on first Awake.  This becomes the unique key for saving/spawning.
        if (string.IsNullOrEmpty(lootId))
            lootId = Guid.NewGuid().ToString();

        // 2) Default the lootName to the GameObject’s base name if none supplied:
        if (string.IsNullOrEmpty(lootName))
            lootName = gameObject.name.Replace("(Clone)", "").Trim();
    }
    public Vector3 GetWorldPosition()
    {
        return transform.position;
    }
}
