using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodShop : QueuedBuilding
{

    [Header("Food Atrributes")]
    public int maxStackedFood = 3;
    public float minFoodDelay = 3.0f;
    public float maxFoodDelay = 5.0f;

    protected Queue<Food> readyFood;
    protected float foodDelay;


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        readyFood = new Queue<Food>();
        foodDelay = Random.Range(minFoodDelay, maxFoodDelay);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (readyFood.Count < maxStackedFood)
            foodDelay -= Time.deltaTime;

        if (foodDelay < 0) {
            readyFood.Enqueue(new Food());
            foodDelay = Random.Range(minFoodDelay, maxFoodDelay);
        }

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
                    if (readyFood.Count > 0)
                    {
                        UserDefault customer = userQueue.Dequeue();
                        customer.GiveFood(readyFood.Dequeue());
                        ReajustQueue();
                    }
                }
            }
        }
    }
}
