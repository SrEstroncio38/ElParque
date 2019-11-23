using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corpse : Trash
{
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    protected override IEnumerator cleanTime()
    {
        yield return new WaitForSeconds(Random.Range(minSeconds, maxSeconds));
        cleaner.finishedCleaning();
        Destroy(gameObject);
    }
}
