using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_attackState2 : StateMachineBehaviour
{
    private NavMeshAgent agent;
    private Transform player;
    public float rangedMinRange = 4.5f;      // Minimum distance for ranged attack (if player is closer, use melee)
    public float rangedMaxRange = 8f;        // Maximum distance for ranged attack


    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent = animator.GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LookAtPlayer();
        float distance = Vector3.Distance(animator.transform.position, player.position);

        // If player is too close (melee range), cancel ranged attack (let melee state handle it)
        if (distance < rangedMinRange)
        {
            animator.SetBool("isAttacking2", false);
            return;
        }

        // If player is out of ranged attack distance, cancel the ranged attack state.
        if (distance > rangedMaxRange)
        {
            animator.SetBool("isAttacking2", false);
            return;
        }
    }

    private void LookAtPlayer()
    {
        Vector3 direction = player.position - agent.transform.position;
        agent.transform.rotation = Quaternion.LookRotation(direction);

        var yRotation = agent.transform.eulerAngles.y;
        agent.transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
