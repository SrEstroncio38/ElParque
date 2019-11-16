using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attraction : MonoBehaviour
{

    [Header("Atrributes")]
    public float quality = 100.0f;

    [Header("Queue")]
    public int maxCapacity = 1;
    public int maxQueue = 2;
    public Vector3 queuePosition = Vector3.zero;
    public Vector3 exitPosition = Vector3.zero;

    //Variables
    private Queue<UserDefault> userRiding;
    private Queue<UserDefault> userQueue;
    private bool riding = false;

    // Start is called before the first frame update
    void Start()
    {
        userRiding = new Queue<UserDefault>();
        userQueue = new Queue<UserDefault>();
    }

    // Update is called once per frame
    void Update()
    {
        if ((!riding)&&(userQueue.Count > 0))
        {
            Debug.Log("ATRACCION: hay usuarios");
            for (int i = 0; i < maxCapacity; i++)
            {
                UserDefault nextUser = userQueue.Dequeue();
                userRiding.Enqueue(nextUser);
                nextUser.enterRide();
            }
            ride();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(queuePosition, 5);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(exitPosition, 5);
    }
    public void addUser(UserDefault user)
    {
        userQueue.Enqueue(user);
       // user.getAgent().SetDestination(queuePosition);
        //queuePosition = user.transform.position;
        
    }

    public void ride()
    {
        riding = true;
        StartCoroutine(TimeRiding()); //Esto se debería convertir en animacion

    }

    IEnumerator TimeRiding()
    {
        yield return new WaitForSeconds(5); //Se supone que la atracción dura dos minutos de tiempo de juego, 2 segundos para nosotros
        foreach (UserDefault user in userRiding)
        {
            user.finishRide();
        }
        riding = false;
        userRiding.Clear();
    }


}
