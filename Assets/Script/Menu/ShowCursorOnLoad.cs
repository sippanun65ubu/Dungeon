using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowCursorOnLoad : MonoBehaviour
{
    public bool unlockAndShowCursor = true;

    void Start()
    {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
    }

    private void Update()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
