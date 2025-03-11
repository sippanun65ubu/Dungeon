using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CheckPoint", menuName = "ScriptableObjects/CheckPoints", order = 1)]

public class CheckPoints : ScriptableObject
{

    public string name;
    public bool isCompleted;

}
