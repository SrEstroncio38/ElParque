using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worker : MonoBehaviour
{
    protected WorldController world;

    // Start is called before the first frame update
    void Start()
    {
        world = GetComponentInParent<WorldController>();
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }



}
