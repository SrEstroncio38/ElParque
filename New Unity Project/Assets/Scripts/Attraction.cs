﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attraction : MonoBehaviour
{

    [Header("Atrributes")]
    public float quality = 100.0f;

    [Header("Queue")]
    public int maxCapacity = 1;
    public float maxWait = 10;
    public Vector3 queuePosition = Vector3.zero;
    public Vector3 exitPosition = Vector3.zero;
    public Vector3 queueDirection = Vector3.zero;

    //Variables
    private Queue<UserDefault> userRiding;
    private Queue<UserDefault> userQueue;
    private bool riding = false;
    private float waitTimer = 0;

    // Start is called before the first frame update
    void Start()
    {
        userRiding = new Queue<UserDefault>();
        userQueue = new Queue<UserDefault>();

        queueDirection = new Vector3(queueDirection.x, 0, queueDirection.z);
        queueDirection.Normalize();
        queueDirection *= 10;

    }

    // Update is called once per frame
    void Update()
    {

        // Decrease Timer
        if (waitTimer > 0)
            waitTimer -= Time.deltaTime;

        // Try to ride
        if (userQueue.Count > 0)
        {
            foreach (UserDefault user in userQueue)
            {
                user.lowerTolerance();
            }
            if (!riding)
            {
                Debug.Log("ATRACCION: hay usuarios");
                for (int i = 0; i < maxCapacity; i++)
                {
                    if (Mathf.Abs((userQueue.Peek().transform.position - queuePosition).magnitude) < 0.1f)
                    {
                        UserDefault nextUser = userQueue.Dequeue();
                        userRiding.Enqueue(nextUser);
                        nextUser.enterRide();
                        waitTimer = maxWait;
                        int j = 0;
                        foreach (UserDefault user in userQueue)
                        {
                            user.getAgent().SetDestination(queuePosition + queueDirection * j);
                        }
                    } else
                    {
                        break;
                    }
                }
            }
        }

        if (userRiding.Count >= maxCapacity)
        {
            Ride();
        } else if (userRiding.Count > 0 && waitTimer < 0)
        {
            Ride();
        }
        
    }

    public int GetQueueLength()
    {
        return userQueue.Count;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(queuePosition, 5);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(exitPosition, 5);
    }
    public void AddUser(UserDefault user)
    {
        userQueue.Enqueue(user);
        Vector3 dest = queuePosition + queueDirection * (userQueue.Count - 1);
        user.getAgent().SetDestination(dest);
        Debug.Log("Destination: " + dest);
    }

    public void Ride()
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

    public void Leave(UserDefault user) {
        if (userQueue.Contains(user))
        {
            Queue<UserDefault> aux = new Queue<UserDefault>();
            while (userQueue.Count > 0)
            {
                UserDefault u = userQueue.Dequeue();
                if (!u.Equals(user))
                {
                    aux.Enqueue(u);
                }
            }
            foreach (UserDefault u in aux)
            {
                userQueue.Enqueue(u);

                //TODO desplazarse en la cola nuevamente

            }
            aux.Clear();
        }
    }

}
