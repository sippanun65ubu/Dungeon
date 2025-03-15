using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_attackState1 : StateMachineBehaviour
{
    private NavMeshAgent agent;
    private Transform player;
    public float meleeRange = 4f;          // Maximum distance for melee attack

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent = animator.GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LookAtPlayer();
        float distance = Vector3.Distance(animator.transform.position, player.position);

        // If player is outside melee range, cancel melee attack state
        if (distance > meleeRange)
        {
            animator.SetBool("isAttacking1", false);
            return;
        }

    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Optionally reset attackTimer if needed
    }
    private void LookAtPlayer()
    {
        Vector3 direction = player.position - agent.transform.position;
        agent.transform.rotation = Quaternion.LookRotation(direction);

        var yRotation = agent.transform.eulerAngles.y;
        agent.transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
