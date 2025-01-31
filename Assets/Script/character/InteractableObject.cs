using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public bool playerInRange;
    public string ItemName;

    public string GetItemName()
    {
        return ItemName;
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E) && playerInRange && SelectionManager.Instance.onTarget )
        {
            //if inventory is NOT full
            if (!InventorySystem.Instance.CheckifFull())
            {
                InventorySystem.Instance.AddToInventory(ItemName);
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Inventory is full");
            }
        }




    }

    private void OnTriggerEnter(Collider other)
    {
         if(other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}