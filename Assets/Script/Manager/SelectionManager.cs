using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance { get; set; }


    public bool onTarget;
    public GameObject interaction_Info_UI;
    public GameObject selectedObject;
    Text interaction_text;
    public Image centerDotIcon;
    public Image handIcon;

    public bool handIsVisible;
    private void Start()
    {
        onTarget = false;
        interaction_text = interaction_Info_UI.GetComponent<Text>();
    }
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
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {   
            var selectionTransform = hit.transform;

            ShopSystem shop = selectionTransform.GetComponent<ShopSystem>();

            if (shop && shop.playerInRange)
            {
                if (shop.isTalkingWithPlayer == false)
                {
                    interaction_text.text = "Talk";
                    interaction_Info_UI.SetActive(true);
                }
                else
                {
                    interaction_text.text = "";
                    interaction_Info_UI.SetActive(false);
                }

                if (Input.GetKeyDown(KeyCode.E) && shop.isTalkingWithPlayer == false)
                {
                    shop.Talk();
                }
            }

            NPC npc = selectionTransform.GetComponent<NPC>();

            if (npc && npc.playerInRange)
            {
                interaction_text.text = "Talk";
                interaction_Info_UI.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E) && npc.isTalkingWithPlayer == false)
                {
                    npc.StartConversation();
                }

                if (DialogSystem.instance.dialogUIActive)
                {
                    interaction_Info_UI.SetActive(false);
                    centerDotIcon.gameObject.SetActive(false);
                }
            }

            InteractableObject interactable = selectionTransform.GetComponent<InteractableObject>();

            if (interactable && interactable.playerInRange)
            {
                onTarget = true;
                selectedObject = interactable.gameObject;
                interaction_text.text = interactable.GetItemName();
                interaction_Info_UI.SetActive(true);


                if (interactable.CompareTag("pickable"))
                {
                    centerDotIcon.gameObject.SetActive(false);
                    handIcon.gameObject.SetActive(true);

                    handIsVisible = true;
                }
            }


            Enemy enemy = selectionTransform.GetComponent<Enemy>();

            if (enemy && enemy.playerInRange)
            {
                if (enemy.isDead)
                {
                    interaction_text.text = "loot";
                    interaction_Info_UI.SetActive(true);

                    centerDotIcon.gameObject.SetActive(false);
                    handIcon.gameObject.SetActive(true);

                    handIsVisible = true;

                    if (Input.GetMouseButtonDown(0))
                    {
                        Lootable lootable = enemy.GetComponent<Lootable>();
                        loot(lootable);
                    }
                }
                else
                {
                    interaction_text.text = enemy.enemyName;
                    interaction_Info_UI.SetActive(true);

                    centerDotIcon.gameObject.SetActive(true);  
                    handIcon.gameObject.SetActive(false);
                }

            }
            if (!interactable && !enemy && !npc)
            {
                onTarget = false; 
                handIsVisible = false;

                centerDotIcon.gameObject.SetActive(true);
                handIcon.gameObject.SetActive(false);
            }

            if (!interactable && !enemy && !npc && !shop)
            {
                interaction_text.text = "";
                interaction_Info_UI.SetActive(false);
            }
        }
    }

    private void loot(Lootable lootable)
    {
        if (lootable.wasLootCalculated == false)
        {
            List<LootRecieved> recievedLoot  = new List<LootRecieved>();

            foreach (LootPossibility loot in lootable.possibleLoot)
            {
                var lootamount = UnityEngine.Random.Range(loot.amountMin, loot.amountMax +1);
                if (lootamount != 0)
                {
                    LootRecieved it = new LootRecieved();
                    it.item = loot.item;
                    it.amount = lootamount;

                    recievedLoot.Add(it);
                }
            }
            
            lootable.finalLoot = recievedLoot;
            lootable.wasLootCalculated = true;
        }
        // spawn loot on the ground
        Vector3 LootSpawnPosition = lootable.gameObject.transform.position;

        foreach (LootRecieved lootRecieved in lootable.finalLoot)
        {
            for (int i = 0; i < lootRecieved.amount; i++)
            {
                GameObject lootSpawn = Instantiate(Resources.Load<GameObject>(lootRecieved.item.name+"_Model"),
                    new Vector3(LootSpawnPosition.x, LootSpawnPosition.y+0.2f, LootSpawnPosition.z),
                    Quaternion.Euler(0,0,0));
            }

        }
        Destroy(lootable.gameObject);
    }



    public void DisableSelection()
    {
        handIcon.enabled = false;
        centerDotIcon.enabled = false;
        interaction_Info_UI.SetActive(false);

        selectedObject = null;

    }

    public void EnableSelection()
    {
        handIcon.enabled = true;
        centerDotIcon.enabled = true;
        interaction_Info_UI.SetActive(true);

    }

}
