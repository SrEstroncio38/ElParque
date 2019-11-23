using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Cleaner : Worker
{
    private Trash objective;
   
    private enum STATE_working {SEARCHING, GOING_TO_TRASH, CLEANING_TRASH};
    STATE_working state_working = STATE_working.SEARCHING;
    

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
                    currentState = "Buscando basura";
                    wander();
                }
                else {
                    currentState = "Voy a la basura";
                    goToTrash();
                    state_working = STATE_working.GOING_TO_TRASH;
                }
                break;
            case (STATE_working.GOING_TO_TRASH):
                if (isInTrash())
                {
                    currentState = "Limpiando basura";
                    objective.clean();
                    state_working = STATE_working.CLEANING_TRASH;
                }
                break;
            case (STATE_working.CLEANING_TRASH):
                
                break;
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
        if (transform.position.x - objective.getPos().x == 0)
        {
            if (transform.position.y - objective.getPos().y <= 0.3f)
            {
                if (transform.position.z - objective.getPos().z == 0)
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
