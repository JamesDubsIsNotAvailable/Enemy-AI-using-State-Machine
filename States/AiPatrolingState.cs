using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiPatrolingState : AiState
{
    public Vector3 walkPoint;
    bool walkPointSet;

    private readonly AiAgent agent;

    public AiPatrolingState(AiAgent agent)
    {
        this.agent = agent;
    }

    public void Enter()
    {

    }

    public void Update()
    {
        if (!walkPointSet) SearchWalkPoint();
        else agent.navMeshAgent.SetDestination(walkPoint);

        if (agent.target == null)
            agent.target = FindTarget();

        Vector3 distanceToWalkPoint = agent.transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = Random.Range(-agent.stats.walkPointRange, agent.stats.walkPointRange);
        float randomX = Random.Range(-agent.stats.walkPointRange, agent.stats.walkPointRange);

        walkPoint = new Vector3(agent.transform.position.x + randomX, agent.transform.position.y, agent.transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -Vector3.up, 2f, agent.whatIsGround))
            walkPointSet = true;
    }

    private Transform FindTarget()
    {
        Transform player = Object.FindObjectOfType<PlayerManager>().transform;

        Vector3 dirToPlayer = (player.position - agent.transform.position).normalized;
        float distToPlayer = Vector3.Distance(agent.transform.position, player.position);

        if (Vector3.Angle(agent.transform.forward, dirToPlayer) < agent.stats.viewAngle / 2
        && distToPlayer <= agent.stats.sightRange)
        {
            if (!Physics.Raycast(agent.transform.position, dirToPlayer, distToPlayer, agent.whatIsObstacle))
                return player;
        }
        return null;
    }

    public void Exit()
    {

    }
}
