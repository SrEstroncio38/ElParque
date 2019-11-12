using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attraction : MonoBehaviour
{
    public int availableSeats;
    //Variables
    private Vector3 position;
    private UserDefault user; //Esto se convertirá en una lista de usuarios activos
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

    public void addUser(UserDefault user) {
        this.user = user;
    }

    public void ride() {
        //Tal vez aqui habría que añadir la cola
        StartCoroutine(TimeRiding()); //Esto se debería convertir en animacion
       
    }

    IEnumerator TimeRiding() {
        yield return new WaitForSeconds(2); //Se supone que la atracción dura dos minutos de tiempo de juego, 2 segundos para nosotros
        user.finishRide();
    }
}
