using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "GameData", menuName = "GameStateData/GameData", order = 1)]
public class GetData : ScriptableObject
{
    [Header("Player Stats")]
    public float currentHealth;
    public float currentStamina;

    [Header("Game Stats")]
    public int enemyKillCount;
    public int totalScore;
    public float elapsedTime;

    [Header("Inventory")]
    public List<string> inventoryItems; // Stores inventory item names or IDs

    // Optionally, you can add methods to clear or initialize the inventory:
    public void ClearInventory()
    {
        if (inventoryItems != null)
            inventoryItems.Clear();
    }
}
