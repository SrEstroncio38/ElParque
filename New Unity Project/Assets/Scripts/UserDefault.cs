using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UserDefault : MonoBehaviour
{
    [Header("Controller")]
    public WorldController controller;

    [Header("Identity")]
    public bool isMale = true;
    public string name = "";

    [Header("Properties")]
    public float saciedad = 100.0f;
    public float tolerancia = 100.0f;
    public float vejiga = 100.0f;
    public float bienestar = 100.0f;
    public bool isAlive = true;

    private NavMeshAgent agent;
    private WorldController world;

    //State Machines
    private enum STATE_Pasear { PASEANDO, DIRIGIENDOSE_ATRACCIÓN, MONTARSE_ATRACCIÓN };
    private STATE_Pasear estado_pasear = STATE_Pasear.PASEANDO;

    //Variables para pasear
    private bool isWandering = false;
    private float wanderCooldown = 2;

    //Variables para encontrar cosas
    private Attraction attracionObjective;
    private Vector3 objective;
    private float visionAngle = 0.5f;
    private float visionDistance = 400.0f;

    // Start is called before the first frame update
    void Awake()
    {
        NameCreator.PersonName p = NameCreator.Generate();
        isMale = p.isMale;
        name = p.name;
        gameObject.name = name;
        world = GetComponentInParent<WorldController>();
        agent = GetComponent<NavMeshAgent>();
        world = GetComponent<WorldController>();
    }

    // Update is called once per frame
    void Update()
    {

        CalcularBienestar();

        FSM_Pasear();

    }


    private void FSM_Pasear ()
    {
        switch(estado_pasear)
        {
            case STATE_Pasear.PASEANDO:
                if (!attractionInSight())
                {
                    Pasear();
                }
                else {
                    isWandering = false;
                    estado_pasear = STATE_Pasear.DIRIGIENDOSE_ATRACCIÓN;
                    goToAttraction();
                }
                break;
            case STATE_Pasear.DIRIGIENDOSE_ATRACCIÓN:
                if (isInAttraction()) {
                    estado_pasear = STATE_Pasear.MONTARSE_ATRACCIÓN;
                    Debug.Log("Empieza la atracción");
                    attracionObjective.ride();
                }
                break;
            case STATE_Pasear.MONTARSE_ATRACCIÓN:
                
                break;
        }
    }


    private void Pasear()
    {
        wanderCooldown -= Time.deltaTime;

        if (wanderCooldown <= 0 && !isWandering)
        {
            float wanderDistance = Random.Range(10,300);
            float wanderAngle = Random.Range(0, 360);
            Vector3 wanderDestination = transform.position + Quaternion.Euler(0, wanderAngle, 0) * new Vector3(wanderDistance, 0, 0);

            Collider[] cols = Physics.OverlapSphere(wanderDestination,0.1f);
            foreach (Collider col in cols)
            {
                wanderDestination = col.ClosestPointOnBounds(wanderDestination);
            }

            agent.SetDestination(wanderDestination);
            isWandering = true;
        } else if (agent.remainingDistance < 0.1f && isWandering == true)
        {
            wanderCooldown = Random.Range(2, 5);
            isWandering = false;
        }
    }

    private void goToAttraction()
    {
        //Habrá que mejorarlo para el tema de las colisiones, pero es algo provisional para probar

        objective = attracionObjective.getPosition();
        Collider[] cols = Physics.OverlapSphere(objective, 0.4f);
        foreach (Collider col in cols)
        {
            objective = col.ClosestPointOnBounds(objective);
        }
        agent.SetDestination(objective);
        isWandering = true;
    }

    private bool isInAttraction() {
        bool isInAttraction = false;
        if (transform.position.x - objective.x <= 0.3f) {
            if (transform.position.y - objective.y <= 0.3f) {
                if (transform.position.y - objective.y <= 0.3f) {
                    isInAttraction = true;
                }
            }
        }
        return isInAttraction;
    }

    private void CalcularBienestar()
    {

        float s = saciedad * Mathf.Pow(saciedad, 0.25f);
        float t = tolerancia * Mathf.Pow(tolerancia, 0.25f);
        float v = vejiga * Mathf.Pow(vejiga, 0.25f);

        bienestar = 100 * (t + s + v) / (3 * 100 * Mathf.Pow(100, 0.25f));

    }

    void OnMouseDown()
    {
        world.mainCamera.followTarget = GetComponent<UserDefault>();
        world.SetHUDTarget(GetComponent<UserDefault>());
    }

    bool attractionInSight() {
        //Calcula un cono de vision para comprobar si puede ver atracciones
        bool attractionInSight = false;
        foreach (Attraction a in controller.getAttractions())
        {
            Vector3 direccion = (a.getPosition() - transform.position);
            if (direccion.magnitude <= visionDistance)
            {
                direccion = direccion.normalized;
                attractionInSight = Mathf.Abs(1.0f - Vector3.Dot(direccion, transform.forward)) < visionAngle;
                if (attractionInSight)
                {
                    if (attracionObjective != a) //Para que no se repita la atracción en la que monta
                    {
                        attracionObjective = a;
                        attracionObjective.addUser(this);
                    }
                }
                break;
            }
        }
        return attractionInSight;
    }

    public void finishRide() {
        Debug.Log("Terminó");
        estado_pasear = STATE_Pasear.PASEANDO;
    }
}


