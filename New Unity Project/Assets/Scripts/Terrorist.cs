using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Terrorist : UserDefault
{
    private enum STATE_terrorismo {BUSCAR_ARMA, DIRIGIRSE_A_ARMA, USAR_ARMA};
    private STATE_terrorismo estado_terrorismo;
    private Weapon weaponObjective;

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
        currentState = "ESTOY MUY CABREADO";
        switch (estado_terrorismo)
        {
            case STATE_terrorismo.BUSCAR_ARMA:

                break;
            case STATE_terrorismo.DIRIGIRSE_A_ARMA:

                break;
            case STATE_terrorismo.USAR_ARMA:

                break;
        }
    }
}
