using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cammeraholder : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform CameraPosition;
    // Update is called once per frame
    void Update()
    {
        transform.position = CameraPosition.position;
    }
}
