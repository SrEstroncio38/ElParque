﻿using System.Collections;
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
   

    //Umbrales
    protected float umbralVejiga = -1.0f;
    protected float umbralSaciedad = 30.0f;
    protected float umbralBienestar = -1.0f;
    protected NavMeshAgent agent;

    //Variables para pasear
    protected bool isWandering = false;
    protected float wanderCooldown = 2;

    //Variables para encontrar cosas
    protected Attraction attracionObjective;
    protected Bath bathObjective;
    protected FoodShop foodObjective;
    protected Vector3 objective;
    protected float visionAngle = 0.5f;
    protected float visionDistance = 400.0f;
    protected float initY;
    protected Exit parkExit;

    protected Attraction lastAttractionVisited;

    //State Machines
    protected enum STATE_VejigaBaja {BUSCANDO, DIRIGIENDOSE_BAÑO, ESPERANDO_BAÑO, ORINANDO_BAÑO, ORINANDO_ENCIMA};
    protected STATE_VejigaBaja estado_vejiga = STATE_VejigaBaja.BUSCANDO;
    
    protected enum STATE_Enfado { EMPEZAR, DIRIGIENDOSE_SALIDA, FUERA };
    protected STATE_Enfado estado_enfado = STATE_Enfado.EMPEZAR;

    /*************
     * Game Loop *
     *************/
    
    protected override void Start()
    {
        base.Start();

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
        vejiga -= Random.Range(0, 0.05f);
        saciedad -= Random.Range(0, 0.01f);
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

    void OnMouseOver()
    {
        int tmp = (int)bienestar;
        int tmp2 = (int)(bienestar * 10 - tmp * 10);
        string bienestarString = tmp + "," + tmp2 + "%";
        world.SetCursorText(userName + "   ( " + bienestarString + " )");
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

            estado_pasear = STATE_Pasear.PASEANDO;
            ExitQueues("bath");
            FSM_VejigaBaja();
        }
        else if ((saciedad <= umbralSaciedad) && (estado_pasear != STATE_Pasear.MONTARSE_ATRACCIÓN))
        {

            estado_pasear = STATE_Pasear.PASEANDO;
            ExitQueues("food");

            FSM_Hambre();
        }
        else
        {
            
            FSM_Pasear();
        }
    }

    protected virtual void FSM_Enfadarse() { }

    // Hambre

    protected enum STATE_Hambre { BUSCANDO, ESPERANDO_COMIDA, DIRIGIENDOSE_TIENDA, COMIENDO, VOMITANDO };
    protected STATE_Hambre estado_hambre = STATE_Hambre.BUSCANDO;

    protected void FSM_Hambre() {
        switch (estado_hambre)
        {
            case STATE_Hambre.BUSCANDO:
                currentState = "[FSM_Hambre] Buscando comida";
                foodObjective = FoodInSight();
                if (foodObjective == null)
                {

                    ShowEmoticon("hambre");

                    Pasear();
                }
                else
                {

                    currentState = "[FSM_Hambre] Yendo a tienda de comida";
                    ShowEmoticon("hambre");
                    GoToObjective();
                    estado_hambre = STATE_Hambre.DIRIGIENDOSE_TIENDA;
                }
                break;

            case STATE_Hambre.DIRIGIENDOSE_TIENDA:
                if (isInObjective())
                {
                    currentState = "[FSM_Hambre] Yendo a tienda de comida";

                    isWandering = false;
                    foodObjective.AddUser(this);
                    

                    estado_hambre = STATE_Hambre.ESPERANDO_COMIDA;
                    currentState = "[FSM_Hambre] Esperando en puesto de comida";
                    ShowEmoticon("hambre");
                }
                break;
            case STATE_Hambre.ESPERANDO_COMIDA:
                
                break;

            case STATE_Hambre.COMIENDO:
                currentState = "[FSM_Hambre] Comiendo";

                break;

            case STATE_Hambre.VOMITANDO:
                currentState = "[FSM_Hambre] Vomitando";
                break;
        }
    }

    protected void FSM_VejigaBaja() {
        switch (estado_vejiga) {
            case STATE_VejigaBaja.BUSCANDO:
                ShowEmoticon("PiPi");
                if (!bathInSight())
                {
                    currentState = "[FSM Baño] Buscando baño";
                    checkPee();
                    Pasear();
                    
                }else {
                    currentState = "[FSM Baño] Yendo al baño";
                    ShowEmoticon("PiPi");
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
                currentState = "[FSM Baño] Esperando al baño";
                checkPee();
                break;
            case STATE_VejigaBaja.ORINANDO_BAÑO:
                currentState = "[FSM Baño] Usando el baño";
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
                currentState = "[FSM_Pasear] Paseando";
                attracionObjective = AttractionInSight();
                if (attracionObjective == null)
                {
                    currentState = "[FSM_Pasear] Paseando";
                    Pasear();
                }
                else
                {
                    isWandering = false;
                    attracionObjective.AddUser(this);

                    lastAttractionVisited = attracionObjective;
                    estado_pasear = STATE_Pasear.ESPERANDO_ATRACCION;
                    currentState = "[FSM_Pasear] Esperando en atracción";
                    ShowEmoticon("Fun");
                }
                break;
            case STATE_Pasear.ESPERANDO_ATRACCION:
                currentState = "[FSM_Pasear] Esperando en atracción";
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

    protected bool isInObjective()
    {
        bool isInAttraction = false;
        if (Mathf.Abs(transform.position.x - objective.x) <= 1)
        {
            if (Mathf.Abs(transform.position.z - objective.z) <= 1)
            {
                isInAttraction = true;

            }
        }
        return isInAttraction;
    }

    public void LowerTolerance()
    {
        tolerancia -= Random.Range(0, 0.03f);
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
                if (attractionInSight && a != lastAttractionVisited)
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

    public void FinishRide(Vector3 exitPosition)
    {
        
        tolerancia += 60;
        Debug.Log(name + "Terminó");
        transform.position = exitPosition;
        transform.position.Set(transform.position.x, initY, transform.position.z);
        gameObject.SetActive(true);
        estado_pasear = STATE_Pasear.PASEANDO;
        currentState = "[FSM_Pasear] Paseando";
        ShowEmoticon("happy");
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
           
            ShowEmoticon("caca", 3);
            estado_pasear = STATE_Pasear.PASEANDO;
            estado_vejiga = STATE_VejigaBaja.ORINANDO_ENCIMA;
            vejiga = 100;
        }
    }

    /**********
     * Comida *
     **********/

    protected FoodShop FoodInSight()
    {
        foreach (FoodShop f in world.GetComponentsInChildren<FoodShop>())
        {
            Vector3 direccion = (f.transform.position - transform.position);
            if (direccion.magnitude <= visionDistance)
            {
                direccion = direccion.normalized;
                bool foodInSight = Mathf.Abs(1.0f - Vector3.Dot(direccion, transform.forward)) < visionAngle;
                if (foodInSight)
                {
                    return f;
                }
            }
        }
        return null;
    }

    public void GiveFood(Food food)
    {
        estado_hambre = STATE_Hambre.BUSCANDO;
       
        if (!food.isGood())
        {
            tolerancia -= 40;
            //estado_hambre = STATE_Hambre.VOMITANDO;
            saciedad = 50.0f;
            ShowEmoticon("Sick");
            estado_hambre = STATE_Hambre.BUSCANDO;
        }
        else
        {
            tolerancia += 40;
            saciedad = 100.0f;
            ShowEmoticon("yummy");
            estado_hambre = STATE_Hambre.BUSCANDO;
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
        ExitQueues("");
    }

    protected void ExitQueues(string exception)
    {
        if (attracionObjective != null && !exception.Equals("attraction"))
        {
            attracionObjective.Leave(this);
        }
        if (foodObjective != null && !exception.Equals("food"))
        {
            foodObjective.Leave(this);
        }
        if (bathObjective != null && !exception.Equals("bath"))
        {
            bathObjective.leave(this);
        }
    }

    public override void Kill()
    {
        base.Kill();
        ExitQueues();
        world.DisableCamera();
        Destroy(gameObject);
    }
}


