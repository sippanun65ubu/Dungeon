using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EquippableItem : MonoBehaviour
{
    public Animator animator;
    public bool swingWait = false;
    private Weapon damage;

    void Start()
    {
        animator = GetComponent<Animator>();
        damage = GetComponent<Weapon>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) &&
            InventorySystem.Instance.isOpen == false &&
            SelectionManager.Instance.handIsVisible == false && swingWait == false)
        {
            
            swingWait = true;
            Debug.Log("Swing initiated");
            StartCoroutine(NewSwingDelay());
        }
    }

    IEnumerator NewSwingDelay()
    {
        Debug.Log("Starting swing delay coroutine");
        animator.SetTrigger("hit");
        yield return new WaitForSeconds(1f);
        swingWait = false;
        Debug.Log("Swing delay over, swingWait reset");
    }
}
