using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UserNormie : UserDefault
{
    // Start is called before the first frame update
    void Start()
    {
        NameCreator.PersonName p = NameCreator.Generate();
        isMale = p.isMale;
        name = p.name;
        gameObject.name = name;
        world = GetComponentInParent<WorldController>();
        agent = GetComponent<NavMeshAgent>();
        initY = transform.position.y;
        parkExit = world.GetComponentInChildren<Exit>();
    }

    // Update is called once per frame
    void Update()
    {
        CalcularBienestar();

        FSM_Divertirse();

    }

    protected override void FSM_Enfadarse()
    {
        currentState = "Samfadao";
        switch (estado_enfado)
        {
            case STATE_Enfado.EMPEZAR:
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
}
