using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SellSystem : MonoBehaviour
{
    #region || -- Singelton -- ||
    public static SellSystem Instance { get; set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    #endregion

    public Button sellButton;
    public TextMeshProUGUI sellAmount;
    public Button backBTN;

    public List<InventorySlot> sellSlots;
    public List<InventoryItem> itemToBeSold;

    [Header("ShopSystem")]
    public ShopSystem ShopSystem;
    public GameObject sellPanel;

    public void Start()
    {
        GetAllSlot();
        sellButton.onClick.AddListener(SellItem);
        backBTN.onClick.AddListener(ExitSellMode);
    }

    public void ExitSellMode()
    {
        if (SellPanelIsEmpty())
        {
            ShopSystem.DialogMode();
        }
    }

    public bool SellPanelIsEmpty()
    {
        if (itemToBeSold.Count <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    public void SellItem()
    {
        List<GameObject> itemToDestory = new List<GameObject>();

        int MoneyEarned = 0;
        foreach (InventoryItem item in itemToBeSold)
        {
            itemToDestory.Add(item.gameObject);
            MoneyEarned += item.sellingPrice;
        }
        InventorySystem.Instance.currentCoins += MoneyEarned;

        foreach (GameObject ob in itemToDestory)
        {
            Destroy(ob);
        }
        itemToDestory.Clear();
        itemToBeSold.Clear();

        UpdateSellAmountUI();
    }

    public void GetAllSlot()
    {
        sellSlots.Clear();
        foreach (Transform child in sellPanel.transform)
        {
            if (child.CompareTag("Slot"))
            {
                sellSlots.Add(child.GetComponent<InventorySlot>());
            }
        }
    }
    public void ScanItemInSlots()
    {
        itemToBeSold.Clear(); // Clear the list before scanning

        // Loop through all child objects in the sell panel
        foreach (Transform child in sellPanel.transform)
        {
            // Check if the child has the tag "Slot"
            if (child.CompareTag("Slot"))
            {
                // Check if the slot has an InventoryItem component
                InventoryItem item = child.GetComponentInChildren<InventoryItem>();
                if (item != null)
                {
                    // Add the item to the itemToBeSold list
                    itemToBeSold.Add(item);
                }
            }
        }
        // Update the UI to reflect the current items in the sell panel
        UpdateSellAmountUI();
    }

    public void UpdateSellAmountUI()
    {
        int totalPriceDisplay = 0;
        foreach (InventoryItem item in itemToBeSold)
        {
            totalPriceDisplay += item.sellingPrice;
        }
        sellAmount.text = totalPriceDisplay.ToString();
    }
}
