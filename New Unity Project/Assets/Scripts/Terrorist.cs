using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Terrorist : UserDefault
{
    private enum STATE_terrorismo {BUSCAR_ARMA, DIRIGIRSE_A_ARMA, USAR_ARMA};
    private STATE_terrorismo estado_terrorismo;
    private Weapon weaponObjective;
    private Vector3 aux;
    public Corpse skeleton;

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
        
        
        switch (estado_terrorismo)
        {
            case STATE_terrorismo.BUSCAR_ARMA:
                currentState = " Buscando";
                Pasear();
                if (weaponInSight())
                {
                    ShowEmoticon("Terrorist");
                    currentState = "[FSM_Terrorismo] Veo arma";
                    GoToObjective();
                    estado_terrorismo = STATE_terrorismo.DIRIGIRSE_A_ARMA;
                }
                break;
            case STATE_terrorismo.DIRIGIRSE_A_ARMA:
                if (isInObjective())
                {
                    estado_terrorismo = STATE_terrorismo.USAR_ARMA;
                    if (weaponObjective.use(this))
                    {
                        currentState = "Tengo el arma";
                        estado_terrorismo = STATE_terrorismo.USAR_ARMA;
                    }
                    else {
                        estado_terrorismo = STATE_terrorismo.BUSCAR_ARMA;
                    }
                }
                break;
            case STATE_terrorismo.USAR_ARMA:

                break;
        }
    }

    private bool weaponInSight()
    {
        bool weapon = false;
        foreach (Weapon w in world.GetComponentsInChildren<Weapon>())
        {
            Vector3 direccion = (w.transform.position - transform.position);
            if (direccion.magnitude <= visionDistance)
            {
                direccion = direccion.normalized;
                weapon = Mathf.Abs(1.0f - Vector3.Dot(direccion, transform.forward)) < visionAngle;
                if (weapon)
                {
                    weaponObjective = w;
                    objective = w.transform.position;
                }
                break;
            }
        }
        return weapon;
    }

    //Metodos para el arma//
    public void goToTerroristObjective(Vector3 obj)
    {
       
        agent.SetDestination(obj);
        isWandering = true;
    }

    public void setState(string state)
    {
        currentState = state;
    }

    public void explodeBomb()
    {
        ShowEmoticon("Explosion");
        Corpse c = Instantiate(skeleton, transform.position, Quaternion.identity, world.GetComponent<Transform>());
       
        c.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    public Vector3 getPos()
    {
        return transform.position;
    }

    public bool isInTerroristObjective(Vector3 tObjective)
    {
        bool isInObj = false;

        
        if (Mathf.Abs(transform.position.x - tObjective.x) <= 0.2f)
        {
            if (Mathf.Abs(transform.position.y - tObjective.y) <= 0.3f)
            {
                if (Mathf.Abs(transform.position.z - tObjective.z) <= 0.2f)
                {
                    isInObj = true;

                }
            }
        }
        return isInObj;
    }
}

