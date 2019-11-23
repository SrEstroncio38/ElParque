using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engineer : Worker
{
    private enum STATE_Working {ESPERANDO, DIRIGIENDOSE_ATRACCION, ARREGLANDO_ATRACCION, LLAMAR_TECNICO};
    private STATE_Working estado_trabajo = STATE_Working.ESPERANDO;
    private float quality;
    private float thresholdQuality = 30.0f;
    private Attraction attractionObjective;
    private Vector3 objective;
    private int minWait = 5;
    private int maxWait = 15;
    // Update is called once per frame
    void Update()
    {
        FSM_work();   
    }

    private void FSM_work()
    {
        switch (estado_trabajo)
        {
            case STATE_Working.ESPERANDO:
                Wander();
                currentState = "Esperando";
                break;
            case STATE_Working.DIRIGIENDOSE_ATRACCION:
                currentState = "Atraccion rota";
                if (isInAttraction())
                {
                    repair();
                    estado_trabajo = STATE_Working.ARREGLANDO_ATRACCION;
                    ShowEmoticon("llaveInglesa");
                }
                break;
            case STATE_Working.ARREGLANDO_ATRACCION:
                currentState = "arreglando";
                break;
                

        }
    }

    public void brokenAttraction(Attraction attraction)
    {
        
        attractionObjective = attraction;
        goToAttraction();
        estado_trabajo = STATE_Working.DIRIGIENDOSE_ATRACCION;
    }

    private void goToAttraction()
    {
        objective = attractionObjective.transform.position;
        Collider[] cols = Physics.OverlapSphere(objective, 0.1f);
        foreach (Collider col in cols)
        {
            objective = col.ClosestPointOnBounds(objective);
        }
        agent.SetDestination(objective);
    }

    private bool isInAttraction()
    {
        bool isInAttraction = false;
        if (Mathf.Abs(transform.position.x - objective.x) <= 2)
        {
            if (Mathf.Abs(transform.position.z - objective.z) <= 2)
            {
                isInAttraction = true;

            }
        }
        return isInAttraction;
    }

    private void repair()
    {
        StartCoroutine(timeRepairing());
    }

    IEnumerator timeRepairing()
    {
        yield return new WaitForSeconds(Random.Range(minWait, maxWait));
        quality = Random.Range(0, 100);
        if (quality < thresholdQuality)
        {
            ShowEmoticon("angry");
            estado_trabajo = STATE_Working.ESPERANDO;
            attractionObjective.repairedWrong();
            
        }
        else
        {
            estado_trabajo = STATE_Working.ESPERANDO;
            attractionObjective.repairedWell();
            ShowEmoticon("happy");
        }
        attractionObjective = null;
    }

    public Attraction GetAttraction()
    {
        return attractionObjective;
    }

   
}
