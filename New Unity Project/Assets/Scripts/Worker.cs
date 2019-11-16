using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worker : Human
{
    protected WorldController world;
    public string name = "";
    public bool isMale = false;

    // Start is called before the first frame update
    void Start()
    {
        world = GetComponentInParent<WorldController>();
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseDown()
    {
        world.mainCamera.followTarget = GetComponent<Worker>();
        world.SetHUDTarget(GetComponent<Worker>());
    }



}
