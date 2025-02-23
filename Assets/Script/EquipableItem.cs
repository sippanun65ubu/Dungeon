using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EquippableItem : MonoBehaviour
{
    public Animator animator;

    public bool swingWait = false;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) &&
            InventorySystem.Instance.isOpen == false &&
            SelectionManager.Instance.handIsVisible == false && swingWait == false)
        {
            Debug.Log("Swing initiated");
            swingWait = true;
            //StartCoroutine(SwingSoundDelay());

            animator.SetTrigger("hit");
            StartCoroutine(NewSwingDelay());
        }
    }

    //IEnumerator SwingSoundDelay()
    //{
    //    yield return new WaitForSeconds(0.2f);
    //    SoundManager.Instance.PlaySound(SoundManager.Instance.toolSwingSound);
    //}

    IEnumerator NewSwingDelay()
    {
        Debug.Log("Starting swing delay coroutine");
        yield return new WaitForSeconds(1f);
        swingWait = false;
        Debug.Log("Swing delay over, swingWait reset");
    }
}
