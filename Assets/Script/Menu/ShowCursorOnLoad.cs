using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowCursorOnLoad : MonoBehaviour
{
    public bool unlockAndShowCursor = true;

    void Start()
    {
        if (unlockAndShowCursor)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
