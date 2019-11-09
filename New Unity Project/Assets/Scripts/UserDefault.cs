﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UserDefault : MonoBehaviour
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

    private NavMeshAgent agent;
    private WorldController world;

    // Start is called before the first frame update
    void Start()
    {
        NameCreator.PersonName p = NameCreator.Generate();
        isMale = p.isMale;
        name = p.name;
        gameObject.name = name;
        world = GetComponentInParent<WorldController>();
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {

        CalcularBienestar();

        FSM_Pasear();

    }

    private enum STATE_Pasear { PASEANDO, DIRIGIENDOSE_ATRACCIÓN, MONTARSE_ATRACCIÓN };
    private STATE_Pasear estado_pasear = STATE_Pasear.PASEANDO;

    private void FSM_Pasear ()
    {
        switch(estado_pasear)
        {
            case STATE_Pasear.PASEANDO:
                Pasear();
                break;
            case STATE_Pasear.DIRIGIENDOSE_ATRACCIÓN:
                //code
                break;
            case STATE_Pasear.MONTARSE_ATRACCIÓN:
                //code
                break;
        }
    }
    
    private bool isWandering = false;
    private float wanderCooldown = 2;

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
}

