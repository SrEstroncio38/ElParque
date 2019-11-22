using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bath : MonoBehaviour
{
    [Header("Queue")]
    public int maxCapacity = 1;
    public int maxQueue = 1;
    public Vector3 queuePosition;

    private List<UserDefault> userUsing;
    private Queue<UserDefault> userQueue;

    private int minTime = 5;
    private int maxTime = 10;

    // Start is called before the first frame update
    void Start()
    {
        userUsing = new List<UserDefault>();
        userQueue = new Queue<UserDefault>();
        queuePosition.Set(transform.position.x + 40, transform.position.y, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(queuePosition, 5);
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
