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
  

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        gameObject.name = "[T] " + userName;
    }

    protected override void FSM_Enfadarse()
    {
        
        switch (estado_terrorismo)
        {
            case STATE_terrorismo.BUSCAR_ARMA:
                currentState = " Buscando";
                weaponObjective = WeaponInSight();
                if (weaponObjective == null)
                {
                    Pasear();
                }
                else
                {
                    ShowEmoticon("Terrorist");
                    currentState = "[FSM_Terrorismo] Veo arma";
                    agent.SetDestination(weaponObjective.transform.position);
                    estado_terrorismo = STATE_terrorismo.DIRIGIRSE_A_ARMA;
                }
                break;
            case STATE_terrorismo.DIRIGIRSE_A_ARMA:
                if (agent.pathStatus == NavMeshPathStatus.PathComplete)
                {
                    estado_terrorismo = STATE_terrorismo.USAR_ARMA;
                    if (weaponObjective.use(this))
                    {
                        currentState = "[FSM_Terrorismo] Tengo el arma";
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

    private Weapon WeaponInSight()
    {
        foreach (Weapon w in world.GetComponentsInChildren<Weapon>())
        {
            Vector3 direccion = (w.transform.position - transform.position);
            if (direccion.magnitude <= visionDistance)
            {
                direccion = direccion.normalized;
                bool weapon = Mathf.Abs(1.0f - Vector3.Dot(direccion, transform.forward)) < visionAngle;
                if (weapon)
                {
                    return w;
                }
            }
        }
        return null;
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
        Kill();
    }

    public Vector3 getPos()
    {
        return transform.position;
    }

    public bool isInTerroristObjective(Vector3 tObjective)
    {
        bool isInObj = false;

        
        if (Mathf.Abs(transform.position.x - tObjective.x) <= 3)
        {
            if (Mathf.Abs(transform.position.z - tObjective.z) <= 3)
            {
                isInObj = true;

            }
        }
        return isInObj;
    }

    public override void Kill()
    {
        base.Kill();
        world.GenerateTerrorist(1);
    }
}

