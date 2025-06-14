using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitCamera : MonoBehaviour
{

    private Quaternion fixedRotation;
    public Transform player;
    public float height = 40f;

    private void Start()
    {
         fixedRotation = transform.rotation;
    }

    private void LateUpdate()
    {
        if (player != null)
        {
            Vector3 pos = player.position;
            pos.y = height;
            transform.position = pos;
        }

        transform.rotation = fixedRotation;
    }
}
