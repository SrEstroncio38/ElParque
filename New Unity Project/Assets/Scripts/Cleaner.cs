using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Cleaner : Worker
{
    private Trash objective;
    private bool isWandering = false;
    private float wanderCooldown = 2;
    private NavMeshAgent agent;
    private enum STATE_working {SEARCHING, GOING_TO_TRASH, CLEANING_TRASH};
    STATE_working state_working = STATE_working.SEARCHING;
    void Start()
    {
        world = GetComponentInParent<WorldController>();
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        FSMWork();
    }

    void FSMWork() {
        switch (state_working)
        {
            case (STATE_working.SEARCHING):
                if (!smellsLikeGarbage())
                {
                    wander();
                }
                else {
                    
                    goToTrash();
                    state_working = STATE_working.GOING_TO_TRASH;
                }
                break;
            case (STATE_working.GOING_TO_TRASH):
                if (isInTrash())
                {
                    state_working = STATE_working.CLEANING_TRASH;
                   
                    objective.clean();
                }
                break;
            case (STATE_working.CLEANING_TRASH):
                
                break;
        }
    }

    private void wander() {
        wanderCooldown -= Time.deltaTime;

        if (wanderCooldown <= 0 && !isWandering)
        {
            float wanderDistance = Random.Range(10, 300);
            float wanderAngle = Random.Range(0, 360);
            Vector3 wanderDestination = transform.position + Quaternion.Euler(0, wanderAngle, 0) * new Vector3(wanderDistance, 0, 0);

            Collider[] cols = Physics.OverlapSphere(wanderDestination, 0.1f);
            foreach (Collider col in cols)
            {
                wanderDestination = col.ClosestPointOnBounds(wanderDestination);
            }

            agent.SetDestination(wanderDestination);
            isWandering = true;
        }
        else if (agent.remainingDistance < 0.1f && isWandering == true)
        {
            wanderCooldown = Random.Range(2, 5);
            isWandering = false;
        }
    }


    private void goToTrash() {
        agent.SetDestination(objective.getPos());
    }

    private bool smellsLikeGarbage() {
        bool garbage = false;
        foreach (Trash trash in world.GetComponentsInChildren<Trash>()) {
            if ((Vector3.Distance(trash.getPos(), transform.position) <= trash.smellStrength) && trash.isVisible()) {
                if (trash.getCleaner() == null) {
                    objective = trash;
                    trash.setCleaner(this);
                    garbage = true;
                    break;
                }
            }
        }
        return garbage;
    }

    private bool isInTrash() {
        bool isInTrash = false;
        if (transform.position.x - objective.getPos().x <= 0.3f)
        {
            if (transform.position.y - objective.getPos().y <= 0.3f)
            {
                if (transform.position.z - objective.getPos().z <= 0.3f)
                {
                    isInTrash = true;
                }
            }
        }
        return isInTrash;
    }

    public void finishedCleaning() {
        state_working = STATE_working.SEARCHING;
    }
}
