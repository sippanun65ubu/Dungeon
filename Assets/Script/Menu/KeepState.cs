using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepState : MonoBehaviour
{
    public static KeepState Instance;

    public bool aftergame = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
