using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attraction : MonoBehaviour
{

    [Header("Atrributes")]
    public float quality = 100.0f;
    public float qualityThreshold = 20.0f;

    [Header("Queue")]
    public int maxCapacity = 1;
    public float maxWait = 10;
    public Vector2 localQueuePosition = Vector2.zero;
    public Vector2 localExitPosition = Vector2.zero;
    public Vector2 localQueueDirection = Vector2.zero;
    public float queueOffset = 15;

    //Variables
    private Vector3 queuePosition = Vector3.zero;
    private Vector3 exitPosition = Vector3.zero;
    private Vector3 queueDirection = Vector3.zero;
    private Queue<UserDefault> userRiding;
    private Queue<UserDefault> userQueue;
    private bool riding = false;
    private float waitTimer = 0;
    private WorldController world;

    // Start is called before the first frame update
    void Start()
    {
        userRiding = new Queue<UserDefault>();
        userQueue = new Queue<UserDefault>();
        world = GetComponentInParent<WorldController>();
        QueueToWorld();

    }

    // Update is called once per frame
    void Update()
    {

        QueueToWorld();

        // Decrementar tiempo
        if (waitTimer > 0)
            waitTimer -= Time.deltaTime;

        // Actualizar cola
        if (userQueue.Count > 0)
        {
            foreach (UserDefault user in userQueue)
            {
                user.LowerTolerance();
            }
            if (!riding && quality > qualityThreshold)
            {
               
                for (int i = 0; i < maxCapacity; i++)
                {
                    // Añadir a la atraccion
                    if (Mathf.Abs((userQueue.Peek().transform.position - queuePosition).magnitude) < 1)
                    {
                        UserDefault nextUser = userQueue.Dequeue();
                        userRiding.Enqueue(nextUser);
                        nextUser.EnterRide();
                        waitTimer = maxWait;
                        // Recolocar cola
                        ReajustQueue();
                    } else
                    {
                        break;
                    }
                }

                // Comenzar attraccion
                if (userRiding.Count >= maxCapacity)
                {
                    Ride();
                }
                else if (userRiding.Count > 0 && waitTimer < 0)
                {
                    Ride();
                }
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

    void OnMouseOver()
    {
        int tmp = (int)quality;
        int tmp2 = (int)(quality * 10 - tmp * 10);
        string qualityString = tmp + "," + tmp2 + "%";
        world.SetCursorText(userRiding.Count + "/" + maxCapacity + "   ( " + qualityString + " )");
    }

    private void OnDrawGizmos()
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
        user.getAgent().SetDestination(dest);
        
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
          
                user.FinishRide(exitPosition);
            
        }
        userRiding.Clear();
        quality -= Random.Range(1.0f, 10.0f);
        riding = false;
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
                ReajustQueue();
            }
            aux.Clear();
        }
    }

    //Todos los que estén montando mueren
    public void Explode()
    {
        quality = 0;
        world.ShowEmoticon("Explosion", gameObject, 3);
        if (riding)
        {
            foreach (UserDefault user in userRiding)
            {
                user.kill();
            }
            userRiding.Clear();
        }
    }

}
