using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UserNormie : UserDefault
{

    protected override void FSM_Enfadarse()
    {
        currentState = "Samfadao";
        switch (estado_enfado)
        {
            case STATE_Enfado.EMPEZAR:
                ShowEmoticon("angry");
                ExitQueues();
                objective = parkExit.transform.position;
                GoToObjective();
                estado_enfado = STATE_Enfado.DIRIGIENDOSE_SALIDA;
                break;
            case STATE_Enfado.DIRIGIENDOSE_SALIDA:
                if (isInObjective())
                {
                    gameObject.SetActive(false);
                    currentState = "Fuera";
                    estado_enfado = STATE_Enfado.FUERA;
                }
                break;
            case STATE_Enfado.FUERA:

                break;
        }
    }

    public override void Kill()
    {
        base.Kill();
        world.GenerateUser(1);
    }
}
