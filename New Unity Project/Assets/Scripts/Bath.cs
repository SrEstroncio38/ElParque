using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bath : MonoBehaviour
{
    [Header("Queue")]
    public int maxCapacity = 1;
    public int maxQueue = 1;
    public Vector3 queuePosition;

    private Queue<UserDefault> userUsing;
    private Queue<UserDefault> userQueue;

    private int minTime = 1;
    private int maxTime = 5;

    // Start is called before the first frame update
    void Start()
    {
        userUsing = new Queue<UserDefault>();
        queuePosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 getPos() {
        return queuePosition;
    }

    public void addUser(UserDefault user)
    {
        userUsing.Enqueue(user);
    }

    public void use() {
        StartCoroutine(timeUsing());
    }

    IEnumerator timeUsing()
    {
        yield return new WaitForSeconds(Random.Range(minTime, maxTime)); 
        foreach (UserDefault user in userUsing)
        {
            user.finishPee();
        }
        userUsing.Clear();
    }
}
