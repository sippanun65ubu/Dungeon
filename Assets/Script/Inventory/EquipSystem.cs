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


    private void Start()
    {
        PopulateSlotList();
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            SelectQuickSlot(1);
        }
        else if(Input.GetKeyUp(KeyCode.Alpha2))
        {
            SelectQuickSlot(2);
        }
        else if(Input.GetKeyUp(KeyCode.Alpha3))
        {
            SelectQuickSlot(3);
        }
        else if(Input.GetKeyUp(KeyCode.Alpha4))
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

    void SelectQuickSlot(int number)
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

    private void SetEquippedModel(GameObject selectedItem)
    {
        if (selectedItemModel != null)
        {
            DestroyImmediate(selectedItemModel.gameObject);
            selectedItemModel = null;
        }
        string selectItemName = selectedItem.name.Replace("(Clone)", "");
        selectedItemModel = Instantiate(Resources.Load<GameObject>(selectItemName + "_Model"),
            new Vector3(0.75f, -0.43f , 1.35f), Quaternion.Euler(-80f, 0, -90f));
        selectedItemModel.transform.SetParent(toolHolder.transform, false); 
    }

    GameObject GetSelectedItem(int slotnumber)
    {
        return quickSlotsList[slotnumber - 1].transform.GetChild(0).gameObject;


    }
    bool checkedIfSlotIsFull(int slotNumber)
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

    private void PopulateSlotList()
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


    private GameObject FindNextEmptySlot()
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

    internal bool IsHoldingWeapon()
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

    internal int GetWeaponDamage()
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

    internal bool IsThereASwingLock()
    {
        if (selectedItemModel && selectedItemModel.GetComponent<EquippableItem>())
        {
            return selectedItemModel.GetComponent<EquippableItem>().swingWait;
        } 
        else
        {
            return false; 
        }
    }
}
