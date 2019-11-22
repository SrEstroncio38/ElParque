using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trash : MonoBehaviour
{
    private bool thereIsObject = true;
    private bool generating = false;
    public int minSeconds = 1;
    public int maxSeconds = 5;
    public int smellStrength = 500;
    private int range = 500;
    Cleaner cleaner;
    private WorldController world;
    // Start is called before the first frame update
    void Start()
    {
          world = GetComponentInParent<WorldController>();
        
        if (world == null)
        {
            Debug.Log("ERROR: Can't find world controller");
        }
       
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!thereIsObject && !generating) {
            StartCoroutine(generateTrash());
        }
    }

    IEnumerator generateTrash() {
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
        generating = false;
        thereIsObject = true;
    }

    public Vector3 getPos() {
        return transform.position;
    }

    public void clean() {
        StartCoroutine(cleanTime());
    }
    IEnumerator cleanTime() {
        yield return new WaitForSeconds(Random.Range(minSeconds, maxSeconds));
        makeInvisible();
        cleaner.finishedCleaning();
        cleaner = null;
        thereIsObject = false;
        generating = false;
    }

    private void makeInvisible() {
        transform.position.Set(transform.position.x, -10.0f, transform.position.z);
    }

    public void setCleaner(Cleaner c) {
        cleaner = c;
    }

    public Cleaner getCleaner() {
        return cleaner;
    }

    public bool isVisible() {
        return thereIsObject;
    }


}
