using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Transform localTrans;
    void Start()
    {
        localTrans = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Camera.main)
        {
            localTrans.LookAt(2 * localTrans.position - Camera.main.transform.position);
        }
    }
}
