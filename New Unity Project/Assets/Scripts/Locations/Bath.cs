using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bath : QueuedBuilding
{

    public int minTime = 5;
    public int maxTime = 10;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (userQueue.Count > 0)
        {
            foreach (UserDefault user in userQueue)
            {
                user.LowerTolerance();
            }
            if (userRiding.Count < maxCapacity)
            {
                if (Mathf.Abs((userQueue.Peek().transform.position - queuePosition).magnitude) < 1)
                {
                    UserDefault user = userQueue.Dequeue();
                    userRiding.Enqueue(user);
                    ReajustQueue();
                    user.EnterToilet();
                    Use(user);
                }
            }
        }
    }

    public void Use(UserDefault user) {
        StartCoroutine(TimeUsing(user));
    }

    IEnumerator TimeUsing(UserDefault user)
    {
        yield return new WaitForSeconds(Random.Range(minTime, maxTime));

        user.FinishPee(exitPosition);
        userRiding.Dequeue();
    }

    void OnMouseOver()
    {
        world.SetCursorText(userRiding.Count + "/" + maxCapacity);
    }

}
