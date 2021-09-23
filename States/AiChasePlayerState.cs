using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiChasePlayerState : AiState
{
    private readonly AiAgent agent;

    public AiChasePlayerState(AiAgent agent)
    {
        this.agent = agent;
    }

    public void Enter()
    {

    }

    public void Update()
    {
        agent.navMeshAgent.SetDestination(agent.target.position);
    }

    public void Exit()
    {

    }
}
