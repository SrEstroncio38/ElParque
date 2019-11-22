using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    protected bool thereIsObject = true;
    protected bool generating = false;
    public int minSeconds = 1;
    public int maxSeconds = 5;
    protected int range = 500;
    protected Terrorist terrorist;
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


    protected IEnumerator generateWeapon()
    {
        generating = true;
        yield return new WaitForSeconds(Random.Range(minSeconds, maxSeconds));
        Vector3 newPos = new Vector3(Random.Range(-range, range), 0.0f, Random.Range(-range, range));

        //Para evitar que aparezca dentro de una atracción, que el de limmpieza no llega
        Collider[] cols = Physics.OverlapSphere(newPos, 0.1f);
        foreach (Collider col in cols)
        {
            newPos = col.ClosestPointOnBounds(newPos);
        }
        transform.position = newPos;
        gameObject.SetActive(true);
        generating = false;
        thereIsObject = true;
    }

    protected void makeInvisible()
    {
        gameObject.SetActive(false);
    }

    public virtual bool use(Terrorist t) {
        if (terrorist != null)
        {
            return false;
        }
        else
        {
            terrorist = t;
            return true;
        }
    }

    protected virtual void setTerroristObjective()
    {

    }

    protected virtual bool isInTerroristObjective()
    {
        return false;
    }

    protected virtual void FSM_uso() { }
}
