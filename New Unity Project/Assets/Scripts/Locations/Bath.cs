using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bath : MonoBehaviour
{
    [Header("Queue")]
    public int maxCapacity = 1;
    public int maxQueue = 1;
    public Vector2 localQueuePosition;
    public Vector2 localExitPosition;
    public Vector2 localQueueDirection;
    public float queueOffset = 15;

    private List<UserDefault> userUsing;
    private Queue<UserDefault> userQueue;
    private Vector3 queuePosition = Vector3.zero;
    private Vector3 exitPosition = Vector3.zero;
    private Vector3 queueDirection = Vector3.zero;

    private int minTime = 5;
    private int maxTime = 10;

    // Start is called before the first frame update
    void Start()
    {
        userUsing = new List<UserDefault>();
        userQueue = new Queue<UserDefault>();
        QueueToWorld();
    }

    // Update is called once per frame
    void Update()
    {
        QueueToWorld();
        if (userQueue.Count > 0)
        {
            foreach (UserDefault user in userQueue)
            {
                user.LowerTolerance();
            }
            if (userUsing.Count < maxCapacity)
            {
                
                UserDefault user = userQueue.Dequeue();
                userUsing.Add(user);
                user.enterToilet();
                use(user);
            }
        }
    }
    private void QueueToWorld()
    {

        queuePosition = transform.TransformPoint(new Vector3(localQueuePosition.x, 0, localQueuePosition.y));
        exitPosition = transform.TransformPoint(new Vector3(localExitPosition.x, 0, localExitPosition.y));

        queuePosition = new Vector3(queuePosition.x, 0, queuePosition.z);
        exitPosition = new Vector3(exitPosition.x, 0, exitPosition.z);

        localQueueDirection.Normalize();
        queueDirection = transform.TransformVector(new Vector3(localQueueDirection.x, 0, localQueueDirection.y));
        queueDirection *= queueOffset;

    }

    private void OnDrawGizmos()
    {

        QueueToWorld();

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(queuePosition, 5);
        Gizmos.color = new Color(1, 0, 0, 0.25f);
        Gizmos.DrawSphere(queuePosition + queueDirection, 5);
        Gizmos.color = new Color(2, 0, 0, 0.1f);
        Gizmos.DrawSphere(queuePosition + 2 * queueDirection, 5);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(exitPosition, 5);
    }

    public Vector3 getPos() {
        return queuePosition;
    }

    public void addUser(UserDefault user)
    {
        userQueue.Enqueue(user);
    }

    public void use(UserDefault user) {
        StartCoroutine(timeUsing(user));
    }

    IEnumerator timeUsing(UserDefault user)
    {
        yield return new WaitForSeconds(Random.Range(minTime, maxTime));

        user.finishPee();
        userUsing.Remove(user);
       
    }

    public void leave(UserDefault user)
    {
        Queue<UserDefault> alternative = new Queue<UserDefault>();
        while (userQueue.Count > 0)
        {
            UserDefault u = userQueue.Dequeue();
            if (!u.Equals(user))
            {
                alternative.Enqueue(u);
            }
        }
        foreach (UserDefault u in alternative)
        {
            userQueue.Enqueue(u);

        }
        alternative.Clear();
    }


}
