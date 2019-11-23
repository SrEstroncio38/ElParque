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
    protected Cleaner cleaner;
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
        
    }

   

    public Vector3 getPos() {
        return transform.position;
    }

    public virtual void clean() {
        StartCoroutine(cleanTime());
    }
    protected virtual IEnumerator cleanTime() {
        yield return new WaitForSeconds(Random.Range(minSeconds, maxSeconds));
        world.GenerateGarbage(1);
        cleaner.finishedCleaning();
        Destroy(gameObject);

    }

    protected void makeInvisible() {
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
