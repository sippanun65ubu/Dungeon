using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_chase2 : StateMachineBehaviour
{
    private NavMeshAgent agent;
    private Transform player;

    public float chaseSpeed = 11f;
    public float stopChasingDistance = 21f; // When the player is too far, exit chase state

    // Attack ranges for two different attacks
    public float meleeAttackRange = 3f;   // Melee attack if player is very close
    public float rangedAttackRange = 7f;  // Ranged attack if player is a bit further away

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Get the player and agent references
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        agent = animator.GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            Debug.LogError("Enemy_ChaseState: NavMeshAgent is missing!");
            return;
        }

        agent.speed = chaseSpeed;
        agent.isStopped = false;

        // Reset attack flags on entering the chase state
        animator.SetBool("isAttacking", false);
        animator.SetBool("isAttacking2", false);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (player == null) return;

        // Calculate the distance to the player
        float distance = Vector3.Distance(animator.transform.position, player.position);

        // Always update destination toward the player
        agent.SetDestination(player.position);
        animator.transform.LookAt(new Vector3(player.position.x, animator.transform.position.y, player.position.z));

        // If the player is too far, exit the chase state
        if (distance > stopChasingDistance)
        {
            animator.SetBool("isChasing", false);
            agent.ResetPath();
            return;
        }
        else
        {
            animator.SetBool("isChasing", true);
        }

        // Determine which attack to use based on distance
        if (distance <= meleeAttackRange)
        {
            // Very close: perform melee attack
            if (!agent.isStopped)
            {
                agent.isStopped = true;
            }
            animator.SetBool("isAttacking", true);
            animator.SetBool("isAttacking2", false);
        }
        else if (distance <= rangedAttackRange)
        {
            // Within ranged attack distance: perform ranged attack
            if (!agent.isStopped)
            {
                agent.isStopped = true;
            }
            animator.SetBool("isAttacking2", true);
            animator.SetBool("isAttacking", false);
        }
        else
        {
            // If player is not in any attack range, resume movement
            if (agent.isStopped)
            {
                agent.isStopped = false;
            }
            animator.SetBool("isAttacking", false);
            animator.SetBool("isAttacking2", false);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (agent != null)
        {
            agent.ResetPath();
            agent.isStopped = false;
        }
    }
}
