using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    public TextMeshProUGUI amountTXT;
    public InventoryItem itemInSlot;

    private void Update()
    {
        InventoryItem item = CheckInventoryItem();

        if (item != null )
        {
            itemInSlot = item;
        }
        else
        {
            itemInSlot = null;
        }
    }

    private InventoryItem CheckInventoryItem()
    {
        foreach (Transform child in transform)
        {
            if (child.GetComponent<InventoryItem>())
            {
                return child.GetComponent<InventoryItem>();
            }
        }
        return null;
    }
    public void UpdateItemInSlot()
    {
        itemInSlot = CheckInventoryItem();
    }
}
