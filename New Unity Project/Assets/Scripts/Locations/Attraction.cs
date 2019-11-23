using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attraction : QueuedBuilding
{

    [Header("Atrributes")]
    public float quality = 100.0f;
    public float qualityThreshold = 20.0f;

    [Header("Queue")]
    public float maxWait = 10;
    public Vector2 localEngineerPos;
    private Vector3 engineerPos;

    //Variables
    private bool riding = false;
    private float waitTimer = 0;
    private Engineer currentEngineer;
    private int lastEngineer;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        currentEngineer = null;

    }

    // Update is called once per frame
    protected override void Update()
    {

        base.Update();

        if (quality < 0)
        {
            quality = 0;
        }

        // Decrementar tiempo
        if (waitTimer > 0)
            waitTimer -= Time.deltaTime;

        if ((quality < qualityThreshold) && (currentEngineer == null))
        {
            brokenAttraction();
        }
        // Actualizar cola
        if (userQueue.Count > 0)
        {
            foreach (UserDefault user in userQueue)
            {
                user.LowerTolerance();
            }
            if (!riding && (quality > qualityThreshold))
            {
                // Añadir a la atraccion
                if (userRiding.Count < maxCapacity)
                {
                    if (Mathf.Abs((userQueue.Peek().transform.position - queuePosition).magnitude) < 1)
                    {
                        UserDefault nextUser = userQueue.Dequeue();
                        userRiding.Enqueue(nextUser);
                        nextUser.EnterRide();
                        waitTimer = maxWait;
                        // Recolocar cola
                        ReajustQueue();
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

    void OnMouseOver()
    {
        int tmp = (int)quality;
        int tmp2 = (int)(quality * 10 - tmp * 10);
        string qualityString = tmp + "," + tmp2 + "%";
        world.SetCursorText(userRiding.Count + "/" + maxCapacity + "   ( " + qualityString + " )");
    }

    public void Ride()
    {
        riding = true;
        StartCoroutine(TimeRiding()); //Esto se debería convertir en animacion
    }

    IEnumerator TimeRiding()
    {
        yield return new WaitForSeconds(6); //Se supone que la atracción dura dos minutos de tiempo de juego, 2 segundos para nosotros
        foreach (UserDefault user in userRiding)
        {
          
                user.FinishRide(exitPosition);
            
        }
        userRiding.Clear();
        quality -= Random.Range(1.0f, 10.0f);
        riding = false;
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
                user.Kill();
            }
            userRiding.Clear();
        }
    }


    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.gray;
        Gizmos.DrawSphere(engineerPos, 5);
    }

    protected override void QueueToWorld()
    {

        base.QueueToWorld();

        engineerPos = transform.TransformPoint(new Vector3(localEngineerPos.x, 1, localEngineerPos.y));
       

    }
    //********************
    //**Rotura y arreglo**
    //********************
    public void brokenAttraction()
    {
        //Elige un ingeniero random que la arregle
        Engineer[] engineers = world.GetComponentsInChildren<Engineer>();
        do
        {
            int index = Random.Range(0, engineers.Length);
            currentEngineer = engineers[index];
        } while (currentEngineer.GetAttraction() != null);


        currentEngineer.brokenAttraction(this);
    }

    public void repairedWrong()
    {
        currentEngineer = null;
        brokenAttraction();
    }

    public void repairedWell()
    {
        currentEngineer = null;
        quality = 100;
    }

    public Vector3 getEngineerPos()
    {
        return engineerPos;
    }
}
