using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccessPo : MonoBehaviour
{
    public static AccessPo Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }
    public Vector3 PlayerPosition
    {
        get => transform.position;
        set => transform.position = value;
    }
    public Quaternion PlayerRotation
    {
        get => transform.rotation;
        set => transform.rotation = value;
    }
}
