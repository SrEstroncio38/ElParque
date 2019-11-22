using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UserDefault : Human
{

    /*************
     * Variables *
     *************/

    [Header("Properties")]
    public float saciedad = 100.0f;
    public float tolerancia = 100.0f;
    public float vejiga = 100.0f;
    public float bienestar = 100.0f;
    public bool isAlive = true;

    //Umbrales
    protected float umbralVejiga = 30.0f;
    protected float umbralSaciedad = 30.0f;
    protected float umbralBienestar = 30.0f;
    protected NavMeshAgent agent;

    //Variables para pasear
    protected bool isWandering = false;
    protected float wanderCooldown = 2;

    //Variables para encontrar cosas
    protected Attraction attracionObjective;
    protected Vector3 objective;
    protected Bath bathObjective;
    protected FoodShop foodObjective;
    protected float visionAngle = 0.5f;
    protected float visionDistance = 400.0f;
    protected float initY;
    protected Exit parkExit;

    //State Machines
    protected enum STATE_VejigaBaja {BUSCANDO, DIRIGIENDOSE_BAÑO, ESPERANDO_BAÑO, ORINANDO_BAÑO, ORINANDO_ENCIMA};
    protected STATE_VejigaBaja estado_vejiga = STATE_VejigaBaja.BUSCANDO;
    
    protected enum STATE_Hambre {BUSCANDO, DIRIGIENDOSE_TIENDA, ESPERANDO_COMIDA, COMIENDO, VOMITANDO};
    protected STATE_Hambre estado_hambre = STATE_Hambre.BUSCANDO;
    
    protected enum STATE_Enfado { EMPEZAR, DIRIGIENDOSE_SALIDA, FUERA };
    protected STATE_Enfado estado_enfado = STATE_Enfado.EMPEZAR;

    /*************
     * Game Loop *
     *************/
    
    void Start()
    {
        currentState = "[FSM_Pasear] Esperando en atracción";
        agent = GetComponent<NavMeshAgent>();
        initY = transform.position.y;
        parkExit = world.GetComponentInChildren<Exit>();
    }
    
    void Update()
    {

        CalcularBienestar();

        FSM_Divertirse();

    }

    protected void CalcularBienestar()
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

    /**********************
     * Maquinas de Estado *
     **********************/
    
    protected void FSM_Divertirse() {
        if ((bienestar <= umbralBienestar) && (estado_pasear != STATE_Pasear.MONTARSE_ATRACCIÓN))
        {
            
           
            FSM_Enfadarse();
        }
         else if ((vejiga <= umbralVejiga) && (estado_pasear != STATE_Pasear.MONTARSE_ATRACCIÓN))
        {
            ExitQueues();
            FSM_VejigaBaja();
        }
        else if ((saciedad <= umbralSaciedad) && (estado_pasear != STATE_Pasear.MONTARSE_ATRACCIÓN))
        {
            ExitQueues();
            FSM_Hambre();
        }
        else
        {
            estado_hambre = STATE_Hambre.BUSCANDO;
            FSM_Pasear();
        }
    }

    protected virtual void FSM_Enfadarse() { }

    protected void FSM_Hambre() {
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

    protected void FSM_VejigaBaja() {
        switch (estado_vejiga) {
            case STATE_VejigaBaja.BUSCANDO:
                if (!bathInSight())
                {
                    checkPee();
                    Pasear();
                }else {
                  
                    estado_vejiga = STATE_VejigaBaja.DIRIGIENDOSE_BAÑO;
                    GoToObjective();
                }
                break;
            case STATE_VejigaBaja.DIRIGIENDOSE_BAÑO:
                checkPee();
                if (isInObjective())
                {
                    
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

    // Pasear

    protected enum STATE_Pasear { PASEANDO, ESPERANDO_ATRACCION, MONTARSE_ATRACCIÓN };
    protected STATE_Pasear estado_pasear = STATE_Pasear.PASEANDO;

    protected void FSM_Pasear ()
    {
        switch(estado_pasear)
        {
            case STATE_Pasear.PASEANDO:
                attracionObjective = AttractionInSight();
                if (attracionObjective == null)
                {
                    Pasear();
                }
                else
                {
                    isWandering = false;
                    attracionObjective.AddUser(this);
                
                    estado_pasear = STATE_Pasear.ESPERANDO_ATRACCION;
                    currentState = "[FSM_Pasear] Esperando en atracción";
                }
                break;
            case STATE_Pasear.ESPERANDO_ATRACCION:
                // No es necesario que haga nada
                break;
            case STATE_Pasear.MONTARSE_ATRACCIÓN:
                // No es necesario que haga nada
                break;
        }
    }

    /**********
     * Pasear *
     **********/

    protected void Pasear()
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
    protected void GoToObjective()
    {
        //Habrá que mejorarlo para el tema de las colisiones, pero es algo provisional para probar
       
        Collider[] cols = Physics.OverlapSphere(objective, 0.1f);
        foreach (Collider col in cols)
        {
           objective = col.ClosestPointOnBounds(objective);
        }
        agent.SetDestination(objective);
        isWandering = true;
       // ShowEmoticon("angry");
    }

    protected bool bathInSight() {
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

    protected bool foodInSight()
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
                    
                    foodObjective = f;
                    objective = f.queuePos;
                }
                break;
            }
        }
        return foodInSight;
    }

    protected bool isInObjective()
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

    public void LowerTolerance()
    {
        tolerancia -= 0.05f;
        if (tolerancia < 0)
            tolerancia = 0;
    }

    /****************
     * Attracciones *
     ****************/

    protected Attraction AttractionInSight()
    {
        foreach (Attraction a in world.GetComponentsInChildren<Attraction>())
        {
            Vector3 direccion = (a.transform.position - transform.position);
            if (direccion.magnitude <= visionDistance)
            {
                direccion = direccion.normalized;
                bool attractionInSight = Mathf.Abs(1.0f - Vector3.Dot(direccion, transform.forward)) < visionAngle;
                if (attractionInSight)
                {
                    return a;
                }
            }
        }

        return null;
    }

    public void EnterRide()
    {
        
        estado_pasear = STATE_Pasear.MONTARSE_ATRACCIÓN;
        currentState = "[FSM_Pasear] Montado en atracción";
        gameObject.SetActive(false);
    }

    public void FinishRide()
    {
        tolerancia += 60;
        
        transform.position = attracionObjective.exitPosition;
        transform.position.Set(transform.position.x, initY, transform.position.z);
        gameObject.SetActive(true);
        estado_pasear = STATE_Pasear.PASEANDO;
        currentState = "[FSM_Pasear] Paseando";
    }

    /**********
     * Lavabo *
     **********/

    public void enterToilet() {
      
        estado_vejiga = STATE_VejigaBaja.ORINANDO_BAÑO;
        estado_pasear = STATE_Pasear.PASEANDO;
        gameObject.SetActive(false);
    }

    public void finishPee()
    {
        tolerancia = tolerancia + 50;
        vejiga = 100.0f;
       
        estado_vejiga = STATE_VejigaBaja.BUSCANDO;
        gameObject.SetActive(true);
    }

    protected void checkPee()
    {
        if (vejiga <= 0)
        {
            tolerancia -= 50;
            if (tolerancia < 0)
                tolerancia = 0;
           
            ShowEmoticon("BebePoop", 3);
            estado_pasear = STATE_Pasear.PASEANDO;
            estado_vejiga = STATE_VejigaBaja.ORINANDO_ENCIMA;
            vejiga = 100;
        }
    }

    /**********
     * Comida *
     **********/

    public void giveFood(Food food)
    {
        estado_hambre = STATE_Hambre.COMIENDO;
       
        if (!food.isGood())
        {
            tolerancia -= 40;
            estado_hambre = STATE_Hambre.VOMITANDO;
            saciedad = 50.0f;
           
        }
        else
        {
            tolerancia += 40;
            saciedad = 100.0f;
        }
    }

    /*********
     * Otros *
     *********/

    public NavMeshAgent getAgent()
    {
        return agent;
    }

    protected void ExitQueues() {
        if (attracionObjective != null)
        {
            attracionObjective.Leave(this);
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


