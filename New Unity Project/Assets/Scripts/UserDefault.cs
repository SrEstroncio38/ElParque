using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UserDefault : Human
{

    [Header("Identity")]
    public bool isMale = true;
    public string name = "";

    [Header("Properties")]
    public float saciedad = 100.0f;
    public float tolerancia = 100.0f;
    public float vejiga = 100.0f;
    public float bienestar = 100.0f;
    public bool isAlive = true;
    public string currentState;

    //Umbrales
    private float umbralVejiga = 30.0f;
    private float umbralSaciedad = 30.0f;
    private float umbralBienestar = 30.0f;
    private NavMeshAgent agent;
    private WorldController world;

    //Variables para pasear
    private bool isWandering = false;
    private float wanderCooldown = 2;

    //Variables para encontrar cosas
    private Attraction attracionObjective;
    private Vector3 objective;
    private Bath bathObjective;
    private FoodShop foodObjective;
    private float visionAngle = 0.5f;
    private float visionDistance = 400.0f;
    private float initY;

    //State Machines
    private enum STATE_Pasear { PASEANDO, DIRIGIENDOSE_ATRACCIÓN, ESPERANDO_ATRACCION, MONTARSE_ATRACCIÓN };
    private STATE_Pasear estado_pasear = STATE_Pasear.PASEANDO;

    private enum STATE_VejigaBaja {BUSCANDO, DIRIGIENDOSE_BAÑO, ESPERANDO_BAÑO, ORINANDO_BAÑO, ORINANDO_ENCIMA};
    private STATE_VejigaBaja estado_vejiga = STATE_VejigaBaja.BUSCANDO;

    private enum STATE_Hambre {BUSCANDO, DIRIGIENDOSE_TIENDA, ESPERANDO_COMIDA, COMIENDO, VOMITANDO};
    private STATE_Hambre estado_hambre = STATE_Hambre.BUSCANDO;

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
    }

    // Update is called once per frame
    void Update()
    {

        CalcularBienestar();

        FSM_Divertirse();

    }

    private void CalcularBienestar()
    {
        if (tolerancia > 100)
        {
            tolerancia = 100;
        }
        vejiga = vejiga - 0.05f;
        saciedad = saciedad - 0.01f;
        if (vejiga < 0)
        {
            vejiga = 0;
        }
        if (saciedad < 0)
        {
            saciedad = 0;
        }
        float s = saciedad * Mathf.Pow(saciedad, 0.25f);
        float t = tolerancia * Mathf.Pow(tolerancia, 0.25f);
        float v = vejiga * Mathf.Pow(vejiga, 0.25f);

        bienestar = 100 * (t + s + v) / (3 * 100 * Mathf.Pow(100, 0.25f));

    }

  

    private void FSM_Divertirse() {
        if ((bienestar <= umbralBienestar) && (estado_pasear != STATE_Pasear.MONTARSE_ATRACCIÓN))
        {
            currentState = "Samfadao";
        }
         else if ((vejiga <= umbralVejiga) && (estado_pasear != STATE_Pasear.MONTARSE_ATRACCIÓN))
        {
            currentState = "FSM Baño";
            exitQueues();
            FSM_VejigaBaja();
        }
        else if ((saciedad <= umbralSaciedad) && (estado_pasear != STATE_Pasear.MONTARSE_ATRACCIÓN))
        {
            currentState = "FSM Comida";
            exitQueues();
            FSM_Hambre();
        }
        else
        {
            currentState = "FSM Pasear";
            estado_hambre = STATE_Hambre.BUSCANDO;
            FSM_Pasear();
        }
    }

    private void FSM_Enfadarse()
    { }

    private void FSM_Hambre() {
        switch (estado_hambre)
        {
            case STATE_Hambre.BUSCANDO:
                if (!foodInSight())
                {
                   
                    Pasear();
                }
                else
                {
                  
                    GoToObjective();
                    estado_hambre = STATE_Hambre.DIRIGIENDOSE_TIENDA;
                }
                break;

            case STATE_Hambre.DIRIGIENDOSE_TIENDA:
                if (isInObjective())
                {
                    Debug.Log(name+" Estoy en la tienda");
                    foodObjective.addCustomer(this);
                    estado_hambre = STATE_Hambre.ESPERANDO_COMIDA;
                }
                break;
            case STATE_Hambre.ESPERANDO_COMIDA:
                
                break;
            case STATE_Hambre.COMIENDO:

                break;
            case STATE_Hambre.VOMITANDO:
                
                break;
        }
    }

    private void FSM_VejigaBaja() {
        switch (estado_vejiga) {
            case STATE_VejigaBaja.BUSCANDO:
                if (!bathInSight())
                {
                    checkPee();
                    Pasear();
                }else {
                    Debug.Log(name + "Baño encontrado");
                    estado_vejiga = STATE_VejigaBaja.DIRIGIENDOSE_BAÑO;
                    GoToObjective();
                }
                break;
            case STATE_VejigaBaja.DIRIGIENDOSE_BAÑO:
                checkPee();
                if (isInObjective())
                {
                    Debug.Log(name + "He llegado al baño");
                    bathObjective.addUser(this);
                    estado_vejiga = STATE_VejigaBaja.ESPERANDO_BAÑO;

                }
                
                break;
            case STATE_VejigaBaja.ESPERANDO_BAÑO:
                
                checkPee();
                break;
            case STATE_VejigaBaja.ORINANDO_BAÑO:

                break;
            case STATE_VejigaBaja.ORINANDO_ENCIMA:
                if (bathObjective != null)
                {
                    bathObjective.leave(this);
                }
                vejiga = 100;
                estado_vejiga = STATE_VejigaBaja.BUSCANDO;
                break;
        }
       

    }
    private void FSM_Pasear ()
    {
        switch(estado_pasear)
        {
            case STATE_Pasear.PASEANDO:
                if (!AttractionInSight())
                {
                    Pasear();
                }
                else {
                    isWandering = false;
                    estado_pasear = STATE_Pasear.DIRIGIENDOSE_ATRACCIÓN;
                   Debug.Log(name + "Voy a la atraccion");
                    GoToObjective();
                }
                break;
            case STATE_Pasear.DIRIGIENDOSE_ATRACCIÓN:
                if (isInObjective())
                {
                    estado_pasear = STATE_Pasear.ESPERANDO_ATRACCION;
                    Debug.Log(name + "Espero la atracción");
                    attracionObjective.addUser(this);
                }
                break;
            case STATE_Pasear.ESPERANDO_ATRACCION:
                
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

    //TODO
    private void GoToObjective()
    {
        //Habrá que mejorarlo para el tema de las colisiones, pero es algo provisional para probar
       
        Collider[] cols = Physics.OverlapSphere(objective, 0.1f);
        foreach (Collider col in cols)
        {
           objective = col.ClosestPointOnBounds(objective);
        }
        agent.SetDestination(objective);
        isWandering = true;
    }

    //TODO
    private bool AttractionInSight() {
        bool attractionInSight = false;
        foreach (Attraction a in world.GetComponentsInChildren<Attraction>())
        {
            Vector3 direccion = (a.transform.position - transform.position);
            if (direccion.magnitude <= visionDistance)
            {
                direccion = direccion.normalized;
                attractionInSight = Mathf.Abs(1.0f - Vector3.Dot(direccion, transform.forward)) < visionAngle;
                if ((attractionInSight) && (!a.Equals(attracionObjective))) //Para que no se repitan atracciones
                {
                    attracionObjective = a;
                    objective = a.queuePosition;

                }
                else {
                    attractionInSight = false;
                }
                break;
            }
        }
        return attractionInSight;
    }

    private bool bathInSight() {
        bool bathInSight = false;
        foreach (Bath b in world.GetComponentsInChildren<Bath>())
        {
            Vector3 direccion = (b.transform.position - transform.position);
            if (direccion.magnitude <= visionDistance)
            {
                direccion = direccion.normalized;
                bathInSight = Mathf.Abs(1.0f - Vector3.Dot(direccion, transform.forward)) < visionAngle;
                if (bathInSight)
                {
                    bathObjective = b;
                    objective = b.getPos();
                }
                break;
            }
        }
        return bathInSight;
    }

    private bool foodInSight()
    {
        bool foodInSight = false;
        foreach (FoodShop f in world.GetComponentsInChildren<FoodShop>())
        {
            Vector3 direccion = (f.transform.position - transform.position);
            if (direccion.magnitude <= visionDistance)
            {
                direccion = direccion.normalized;
                foodInSight = Mathf.Abs(1.0f - Vector3.Dot(direccion, transform.forward)) < visionAngle;
                if (foodInSight)
                {
                    Debug.Log(name + "Encuentro comida");
                    foodObjective = f;
                    objective = f.queuePos;
                }
                break;
            }
        }
        return foodInSight;
    }

    private bool isInObjective()
    {
        bool isInAttraction = false;
        if (transform.position.x - objective.x <= 0.2f)
        {
            if (transform.position.y - objective.y <= 0.3f)
            {
                if (transform.position.z - objective.z <= 0.2f)
                {
                    isInAttraction = true;
                    
                }
            }
        }
        return isInAttraction;
    }

    public void lowerTolerance()
    {
        tolerancia -= 0.05f;
        if (tolerancia < 0)
            tolerancia = 0;
    }

    void OnMouseDown()
    {
        world.mainCamera.followTarget = GetComponent<UserDefault>();
        world.SetHUDTarget(GetComponent<UserDefault>());
    }

    public void enterRide()
    {
        Debug.Log(name + "Entro en atraccion");
        estado_pasear = STATE_Pasear.MONTARSE_ATRACCIÓN;
        gameObject.SetActive(false);
    }

    public void finishRide()
    {
        tolerancia += 60;
       Debug.Log(name + "Terminó");
        transform.position = attracionObjective.exitPosition;
        transform.position.Set(transform.position.x, initY, transform.position.z);
        gameObject.SetActive(true);
        estado_pasear = STATE_Pasear.PASEANDO;
    }

    public void enterToilet() {
        Debug.Log(name + "Entro al baño");
        estado_vejiga = STATE_VejigaBaja.ORINANDO_BAÑO;
        estado_pasear = STATE_Pasear.PASEANDO;
        gameObject.SetActive(false);
    }

    public void finishPee()
    {
        tolerancia = tolerancia + 50;
        vejiga = 100.0f;
        Debug.Log(name+"Salgo del baño");
        estado_vejiga = STATE_VejigaBaja.BUSCANDO;
        gameObject.SetActive(true);
    }

    private void checkPee()
    {
        if (vejiga <= 0)
        {
            tolerancia = tolerancia - 50;
            if (tolerancia < 0)
                tolerancia = 0;
            Debug.Log(name + "Me he meado");
            estado_pasear = STATE_Pasear.PASEANDO;
            estado_vejiga = STATE_VejigaBaja.ORINANDO_ENCIMA;
            vejiga = 100;
        }
    }

    public void giveFood(Food food)
    {
        estado_hambre = STATE_Hambre.COMIENDO;
        Debug.Log(name + "Como");
        if (!food.isGood())
        {
            tolerancia -= 40;
            estado_hambre = STATE_Hambre.VOMITANDO;
            saciedad = 50.0f;
            Debug.Log(name+ "Vomito");
        }
        else
        {
            tolerancia += 40;
            saciedad = 100.0f;
        }
    }

    public NavMeshAgent getAgent()
    {
        return agent;
    }

    private void exitQueues() {
        if (attracionObjective != null)
        {
            attracionObjective.leave(this);
        }
        if (foodObjective != null)
        {
            foodObjective.leave(this);
        }
        if (bathObjective != null)
        {
            bathObjective.leave(this);
        }
    }
}


