using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiAttackPlayerState : AiState
{
    private readonly AiAgent agent;

    public AiAttackPlayerState(AiAgent agent)
    {
        this.agent = agent;
    }

    public void Enter()
    {

    }

    public void Update()
    {
        float distToPlayer = Vector3.Distance(agent.transform.position, agent.target.position);

        if (distToPlayer <= agent.stats.attackRange) {
            agent.navMeshAgent.SetDestination(agent.transform.position);
            agent.transform.LookAt(agent.target);
            agent.currentWeapon.OnTriggerHold();

            if (agent.bombLuncher.readyToThrow)
            {
                agent.bombLuncher.Lunch(agent.target.position);
            }

        } else {
            agent.currentWeapon.OnTriggerRelease();
        }
    }

    public void Exit()
    {

    }
}
