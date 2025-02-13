using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{


    public static InventorySystem Instance { get; set; }

    public GameObject inventoryScreenUI;
    public List<GameObject> slotList = new List<GameObject>();
    public List<string> itemList = new List<string>();
    private GameObject itemToAdd;
    private GameObject whatSlotToEquip;
    public bool isOpen;
    public GameObject ItemInfoUi;

    //pickupPopup
    public GameObject pickupAlert;
    public Text pickupName;
    public Image pickupImage;




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


    void Start()
    {
        isOpen = false;

        PopulateSlotList();

        Cursor.visible = false;
    }

    private void PopulateSlotList()
    {

        foreach (Transform chlid in inventoryScreenUI.transform)
        {
            if (chlid.CompareTag("Slot"))
            {
                slotList.Add(chlid.gameObject);
            }
        }

    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.F) && !isOpen)
        {

            inventoryScreenUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            SelectionManager.Instance.DisableSelection();
            SelectionManager.Instance.GetComponent<SelectionManager>().enabled = false;
            isOpen = true;

        }
        else if (Input.GetKeyDown(KeyCode.F) && isOpen)
        {
            inventoryScreenUI.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            SelectionManager.Instance.EnableSelection();
            SelectionManager.Instance.GetComponent<SelectionManager>().enabled = true;
            isOpen = false;
        }
    }
    
public void AddToInventory(string itemName)
    {
      whatSlotToEquip = FindEmtrySlot();

      itemToAdd = (GameObject)Instantiate(Resources.Load<GameObject>(itemName), whatSlotToEquip.transform.position, whatSlotToEquip.transform.rotation);
      itemToAdd.transform.SetParent(whatSlotToEquip.transform);

      itemList.Add(itemName);
      TriggerPickupPopUp(itemName, itemToAdd.GetComponent<Image>().sprite);
    }

void TriggerPickupPopUp(string itemName, Sprite itemSprite)
    {
        pickupAlert.SetActive(true);
        
        pickupName.text = itemName;
        pickupImage.sprite = itemSprite;


        StartCoroutine(HidePickupAfterDelay(4f));
    }

    private IEnumerator HidePickupAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        pickupAlert.SetActive(false);
    }

    public void RemoveItem(string nameToRemove, int amountToRemove)
{
    int counter = amountToRemove;

    for (var i = slotList.Count - 1; i >= 0; i--)
    {
        if (slotList[i].transform.childCount > 0)
        {
            if (slotList[i].transform.GetChild(0).name == nameToRemove + "(Clone)" && counter != 0)
            {
                DestroyImmediate(slotList[i].transform.GetChild(0).gameObject);
            }
        }
    }
}

public void ReCalculateList()
{
    itemList.Clear();
    foreach (GameObject slot in slotList)
    {
        if (slot.transform.childCount > 0)
            {
                string name = slot.transform.GetChild(0).name;
                string str1 = name;
                string str2 = "(Clone)";

                string result = name.Replace(str2, "");

                itemList.Add(result);

            }
    }
}
public bool CheckifFull() 
  {
        int counter = 0;

        foreach (GameObject slot in slotList)
        {
            if (slot.transform.childCount > 0)
            {
                counter += 1;
            }
        }

        if (counter == 18)
        {
            return true;
        }
        else
        {
            return false;
        }

  }


private GameObject FindEmtrySlot()
    {
        foreach (GameObject slot in slotList)
        {
            if (slot.transform.childCount == 0)
            {
                return slot;
            }
        }
        return new GameObject();
    }
    

}