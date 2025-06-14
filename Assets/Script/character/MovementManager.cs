using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : MonoBehaviour
{

    #region  || --- Singeton --||
    public static MovementManager instance { get; set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }
    #endregion

    public bool canMove = true;
    public bool canLookAround = true;

    public void EnableMovement(bool trigger)
    {
        canMove = trigger;
    }

    public void EnableLook(bool trigger)
    {
        canLookAround = trigger;
    }
}


