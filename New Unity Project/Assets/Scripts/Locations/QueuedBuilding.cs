using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueuedBuilding : MonoBehaviour
{

    [Header("Queue")]
    public int maxCapacity = 1;
    public Vector2 localQueuePosition = Vector2.zero;
    public Vector2 localExitPosition = Vector2.zero;
    public Vector2 localQueueDirection = Vector2.zero;
    public float queueOffset = 15;

    protected Vector3 queuePosition = Vector3.zero;
    protected Vector3 exitPosition = Vector3.zero;
    protected Vector3 queueDirection = Vector3.zero;

    protected Queue<UserDefault> userRiding;
    protected Queue<UserDefault> userQueue;

    protected WorldController world;

    protected virtual void Start()
    {
        userRiding = new Queue<UserDefault>();
        userQueue = new Queue<UserDefault>();
        world = GetComponentInParent<WorldController>();
        QueueToWorld();
    }

    protected virtual void Update()
    {
        QueueToWorld();
    }

    protected virtual void QueueToWorld()
    {

        queuePosition = transform.TransformPoint(new Vector3(localQueuePosition.x, 0, localQueuePosition.y));
        exitPosition = transform.TransformPoint(new Vector3(localExitPosition.x, 0, localExitPosition.y));

        queuePosition = new Vector3(queuePosition.x, 0, queuePosition.z);
        exitPosition = new Vector3(exitPosition.x, 0, exitPosition.z);
        
        queueDirection = transform.TransformVector(new Vector3(localQueueDirection.x, 0, localQueueDirection.y));
        queueDirection.Normalize();
        queueDirection *= queueOffset;

    }

    protected void ReajustQueue()
    {
        int j = 0;
        foreach (UserDefault user in userQueue)
        {
            user.getAgent().SetDestination(queuePosition + queueDirection * j);
            j++;
        }
    }

    public void AddUser(UserDefault user)
    {
        userQueue.Enqueue(user);
        Vector3 dest = queuePosition + queueDirection * (userQueue.Count - 1);
        Debug.Log(userQueue.Count);
        user.getAgent().SetDestination(dest);
    }

    public void Leave(UserDefault user)
    {
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
                ReajustQueue();
            }
            aux.Clear();
        }
    }

    protected virtual void OnDrawGizmos()
    {

        QueueToWorld();

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(queuePosition, 5);
        Gizmos.color = new Color(1, 0, 0, 0.25f);
        Gizmos.DrawSphere(queuePosition + queueDirection, 5);
        Gizmos.color = new Color(2, 0, 0, 0.1f);
        Gizmos.DrawSphere(queuePosition + 2 * queueDirection, 5);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(exitPosition, 5);
    }

 

}