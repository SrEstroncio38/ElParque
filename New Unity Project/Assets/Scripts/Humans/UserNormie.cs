using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UserNormie : UserDefault
{

    protected override void FSM_Enfadarse()
    {
        currentState = "[FSM_Enfadarse] Abandonando parque";
        switch (estado_enfado)
        {
            case STATE_Enfado.EMPEZAR:
                ShowEmoticon("angry");
                ExitQueues();
                agent.SetDestination(parkExit.transform.position);
                isWandering = false;
                estado_enfado = STATE_Enfado.DIRIGIENDOSE_SALIDA;
                break;
            case STATE_Enfado.DIRIGIENDOSE_SALIDA:
                if ((transform.position - parkExit.transform.position).magnitude < 2)
                {
                    world.GenerateUser(1);
                    world.DisableCamera();
                    Destroy(gameObject);
                }
                break;
        }
    }

    public override void Kill()
    {
        base.Kill();
        world.GenerateUser(1);
    }
}
