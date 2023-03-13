using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent agent = null;
    [SerializeField] private Targeter targeter = null;
    [SerializeField] float chaseRange = 10f;

    #region Server

    [ServerCallback]
    private void Update() 
    {
        Targetable target = targeter.GetTarget();

        if(target != null)
        {
            if((target.transform.position - transform.position).sqrMagnitude > chaseRange * chaseRange) // better in performance than Vector3.Distance() -> uses sqr root
            {
                agent.SetDestination(target.transform.position);
            }
            else if(agent.hasPath)
            {
                agent.ResetPath();
            }
            return;
        }

        if(!agent.hasPath) return;
        if(agent.remainingDistance > agent.stoppingDistance) return;

        agent.ResetPath();  
    }

    [Command] 
    public void CmdMove(Vector3 position)
    {
        targeter.ClearTarget(); // when try to move anywhere just clear the target(might be unit/bldg/etc..?)

        if(!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f , NavMesh.AllAreas)) return;

        agent.SetDestination(hit.position);
    }

    #endregion
}
