using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attraction : MonoBehaviour
{

    [Header("Atrributes")]
    public float quality = 100.0f;

    [Header("Queue")]
    public int maxCapacity = 1;
    public int maxQueue = 1;
    public Vector3 queuePosition = Vector3.zero;

    //Variables
    private Queue<UserDefault> userRiding;
    private Queue<UserDefault> userQueue;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
