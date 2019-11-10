using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attraction : MonoBehaviour
{
    //Variables
    private Vector3 position;
    // Start is called before the first frame update
    void Start()
    {
        //Se calcula la posicion de la atraccion. 
        position = this.transform.position;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Getter para que el mundo pueda acceder a la posicion de la atraccion
    public Vector3 getPosition() {
        return this.position;
    }
}
