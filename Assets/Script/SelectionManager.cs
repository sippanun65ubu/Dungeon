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

            Enemy enemy = selectionTransform.GetComponent<Enemy>();

            if (enemy && enemy.playerInRange)
            {
                interaction_text.text = enemy.enemyName;
                interaction_Info_UI.SetActive(true);

                if (Input.GetMouseButtonDown(0) && EquipSystem.Instance.IsHoldingWeapon())
                {
                    StartCoroutine(DealDamageTo(enemy, 0.3f, EquipSystem.Instance.GetWeaponDamage()));
                }
                else
                {
                    interaction_text.text = "";
                    interaction_Info_UI.SetActive(false);
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
                }
                else
                {
                    handIcon.gameObject.SetActive(false);
                    centerDotIcon.gameObject.SetActive(true);   
                }
            }
            else
            {
                onTarget = false;
                interaction_Info_UI.SetActive(false);
                handIcon.gameObject.SetActive(false);
                centerDotIcon.gameObject.SetActive(true);
            }

        }
        else
        {
            onTarget = false;
            interaction_Info_UI.SetActive(false);
            handIcon.gameObject.SetActive(false);
            centerDotIcon.gameObject.SetActive(true);
        }
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

    IEnumerator DealDamageTo(Enemy enemy, float delay, int damage)
    {
        yield return new WaitForSeconds(delay);

        enemy.TakeDamage(damage);
    }
}
