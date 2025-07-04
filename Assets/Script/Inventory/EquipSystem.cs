using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class EquipSystem : MonoBehaviour
{
    public static EquipSystem Instance { get; set; }

    // -- UI -- //
    public GameObject quickSlotsPanel;

    public List<GameObject> quickSlotsList = new List<GameObject>();
    public List<string> itemList = new List<string>();
    public GameObject numberHolder;

    public int selectNumber = -1;
    public GameObject selectedItem;

    public GameObject selectedItemModel;
    public GameObject toolHolder;

    public void Awake()
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


    public void Start()
    {
        PopulateSlotList();
    }

    public void Update()
    {
        HandleQuickSlotSelection();
        HandleConsumableItemUse();

    }
    public void HandleQuickSlotSelection()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            SelectQuickSlot(1);
        }
        else if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            SelectQuickSlot(2);
        }
        else if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            SelectQuickSlot(3);
        }
        else if (Input.GetKeyUp(KeyCode.Alpha4))
        {
            SelectQuickSlot(4);
        }
        else if (Input.GetKeyUp(KeyCode.Alpha5))
        {
            SelectQuickSlot(5);
        }
        else if (Input.GetKeyUp(KeyCode.Alpha6))
        {
            SelectQuickSlot(6);
        }
    }

    public void SelectQuickSlot(int number)
    {
        if (checkedIfSlotIsFull(number) == true)
        {
            if(selectNumber != number)
            {
                selectNumber = number;
                // unselected the previous item in quickslot
                if (selectedItem != null)
                {
                    selectedItem.gameObject.GetComponent<InventoryItem>().isSelected = false;
                    // Disable the equippable behavior on the previously selected item
                    EquippableItem prevEquippable = selectedItem.GetComponent<EquippableItem>();
                    if (prevEquippable != null)
                    {
                        prevEquippable.enabled = false;
                        prevEquippable.animator.enabled = false;
                    }
                }
                selectedItem = GetSelectedItem(number);
                selectedItem.GetComponent<InventoryItem>().isSelected = true;

                SetEquippedModel(selectedItem);

                // Update UI colors for quick slot numbers
                foreach (Transform child in numberHolder.transform)
                {
                    child.transform.Find("Text").GetComponent<Text>().color = Color.gray;
                }

                Text toBeChange = numberHolder.transform.Find("number" + number).transform.Find("Text").GetComponent<Text>();
                toBeChange.color = Color.white;
            }
            else //select the same number or unselect
            {
                selectNumber = -1; //null
                // unselected slot
                if (selectedItem != null)
                {
                    selectedItem.gameObject.GetComponent<InventoryItem>().isSelected = false;

                    // Disable equippable behavior when deselecting
                    EquippableItem equippable = selectedItem.GetComponent<EquippableItem>();
                    if (equippable != null)
                    {
                        equippable.enabled = false;
                        equippable.animator.enabled = false;
                    }

                    selectedItem = null;
                }

                if (selectedItemModel != null)
                {
                    DestroyImmediate(selectedItemModel.gameObject);
                    selectedItemModel = null;
                }
                // change color to gray
                foreach (Transform child in numberHolder.transform)
                {
                    child.transform.Find("Text").GetComponent<Text>().color = Color.gray;
                }
            }
        }
    }

    public void SetEquippedModel(GameObject selectedItem)
    {
        if (selectedItemModel != null)
        {
            DestroyImmediate(selectedItemModel.gameObject);
            selectedItemModel = null;
        }
        string selectItemName = selectedItem.name.Replace("(Clone)", "");
        selectedItemModel = Instantiate(Resources.Load<GameObject>(selectItemName + "_Model"));
        selectedItemModel.transform.SetParent(toolHolder.transform, false); 
    }

    public GameObject GetSelectedItem(int slotnumber)
    {
        return quickSlotsList[slotnumber - 1].transform.GetChild(0).gameObject;


    }
    public bool checkedIfSlotIsFull(int slotNumber)
    {
        if (quickSlotsList[slotNumber - 1].transform.childCount > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void PopulateSlotList()
    {
        foreach (Transform child in quickSlotsPanel.transform)
        {
            if (child.CompareTag("QuickSlot"))
            {
                quickSlotsList.Add(child.gameObject);
            }
        }
    }

    public void AddToQuickSlots(GameObject itemToEquip)
    {
        // Find next free slot
        GameObject availableSlot = FindNextEmptySlot();
        // Set transform of our object
        itemToEquip.transform.SetParent(availableSlot.transform, false);

        InventorySystem.Instance.ReCalculateList();

    }


    public GameObject FindNextEmptySlot()
    {
        foreach (GameObject slot in quickSlotsList)
        {
            if (slot.transform.childCount == 0)
            {
                return slot;
            }
        }
        return new GameObject();
    }

    public bool CheckIfFull()
    {

        int counter = 0;

        foreach (GameObject slot in quickSlotsList)
        {
            if (slot.transform.childCount > 0)
            {
                counter += 1;
            }
        }

        if (counter == 6)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsHoldingWeapon()
    {
        if (selectedItem != null)
        {
            if (selectedItem.GetComponent<Weapon>() != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public int GetWeaponDamage()
    {
        if (selectedItem != null)
        {
            return selectedItem.GetComponent<Weapon>().weaponDamage;
            
            
        }
        else
        {
            return 0;
        }
    }
    public int GetWeaponPenetration()
    {
        if (selectedItem != null)
        {
            return selectedItem.GetComponent<Weapon>().penetration;

        }
        else
        {
            return 0;
        }
    }


    public void HandleConsumableItemUse()
    {
        if (selectNumber != -1 && selectedItem != null)
        {
            InventoryItem item = selectedItem.GetComponent<InventoryItem>();
            if (item != null && item.isConsumable && Input.GetMouseButtonDown(0)) // Left mouse click
            {
                ApplyConsumableEffect(item);
                DestroyConsumableItem(item);
            }
        }
    }

    public void ApplyConsumableEffect(InventoryItem item)
    {
        if (item.isConsumable)
        {
            // Call the health and stamina effect calculation methods
            InventoryItem.healthEffectCalculation(item.healthEffect);
        }
    }

    public void DestroyConsumableItem(InventoryItem item)
    {
        if (item.isConsumable)
        {
            // Destroy the selected item
            Destroy(selectedItem);

            // Destroy the selected item model
            if (selectedItemModel != null)
            {
                Destroy(selectedItemModel);
                selectedItemModel = null;
            }

            // Clear the selected item
            selectedItem = null;
            selectNumber = -1;

            // Update the UI
            InventorySystem.Instance.ReCalculateList();
        }
    }
    public void ResetToDefaults()
    {
        // 1) Deselect current slot
        selectNumber = -1;
        if (selectedItem != null)
        {
            selectedItem = null;
        }

        // 2) Destroy the in-hand model if any
        if (selectedItemModel != null)
        {
            DestroyImmediate(selectedItemModel);
            selectedItemModel = null;
        }

        // 3) Empty out all quick-slots
        foreach (var slot in quickSlotsList)
        {
            if (slot.transform.childCount > 0)
            {
                var child = slot.transform.GetChild(0).gameObject;
                DestroyImmediate(child);
            }
        }
        foreach (Transform num in numberHolder.transform)
        {
            var txt = num.Find("Text")?.GetComponent<Text>();
            if (txt != null)
                txt.color = Color.gray;
        }

        InventorySystem.Instance.ReCalculateList();
    }
}
