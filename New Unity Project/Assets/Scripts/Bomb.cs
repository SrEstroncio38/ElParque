using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : Weapon
{
    
    private enum STATE_uso{NO_ENCONTRADA, SELECCIONAR_ATRACCION, DIRIGIRSE_ATRACCION, EXPLOTAR};
    private STATE_uso estado_uso = STATE_uso.NO_ENCONTRADA;
    private Attraction objective;
    // Start is called before the first frame update
    void Start()
    {
        terrorist = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (!thereIsObject && !generating)
        {
            StartCoroutine(generateWeapon());
        }
    }

    public override void use(Terrorist t)
    {
        base.use(t);
      
        
    }


    protected override void FSM_uso()
    {
        switch (estado_uso)
        {
            case STATE_uso.NO_ENCONTRADA:

                break;
            case STATE_uso.SELECCIONAR_ATRACCION:

                break;
            case STATE_uso.DIRIGIRSE_ATRACCION:

                break;
            case STATE_uso.EXPLOTAR:

                break;
        }
    }
}
