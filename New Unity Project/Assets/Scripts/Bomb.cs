using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : Weapon
{
    
    private enum STATE_uso{NO_ENCONTRADA, DIRIGIRSE_ATRACCION, EXPLOTAR};
    private STATE_uso estado_uso = STATE_uso.NO_ENCONTRADA;
    private Attraction objective;
    private Vector3 terroristObjective;
    private WorldController world;
    // Start is called before the first frame update
    void Start()
    {
        terrorist = null;

        world = GetComponentInParent<WorldController>();
        Attraction[] attractions = world.GetComponentsInChildren<Attraction>();
        objective = attractions[Random.Range(0, attractions.Length - 1)];
        terroristObjective = objective.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!thereIsObject && !generating)
        {
            StartCoroutine(generateWeapon());
        }
       
        FSM_uso();
    }



    public override bool use(Terrorist t)
    {
        bool value = base.use(t);
        if (value)
        {
            estado_uso = STATE_uso.DIRIGIRSE_ATRACCION;
            setTerroristObjective();
        }
        return value;
        
    }

    protected override void setTerroristObjective()
    {
        base.setTerroristObjective();
       
        Collider[] cols = Physics.OverlapSphere(terroristObjective, 0.1f);
        foreach (Collider col in cols)
        {
            terroristObjective = col.ClosestPointOnBounds(terroristObjective);
        }
        terrorist.goToTerroristObjective(terroristObjective);
    }

    protected override bool isInTerroristObjective()
    {
        return terrorist.isInTerroristObjective(terroristObjective);
    }

    private void explode()
    {
        objective.explode();
        terrorist.explodeBomb();
        gameObject.SetActive(false);
        thereIsObject = false;
    }

    protected override void FSM_uso()
    {
        switch (estado_uso)
        {
            case STATE_uso.NO_ENCONTRADA:

                break;
            case STATE_uso.DIRIGIRSE_ATRACCION:
                transform.position = terrorist.transform.position;
                if (isInTerroristObjective())
                {
                    explode();
                    estado_uso = STATE_uso.EXPLOTAR;
                }
                break;
            case STATE_uso.EXPLOTAR:
                
                break;
        }
    }

}
