using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodShop : MonoBehaviour
{
    public List<Cooker> employees; //Creo que mejor lista que cola

    private Queue<Food> readyFood;
    private Queue<UserDefault> customers;
    private Queue<UserDefault> currentCustomers;
    private int maxCapacity;

    public Vector3 queuePos;

    // Start is called before the first frame update
    void Start()
    {
        customers = new Queue<UserDefault>();
        readyFood = new Queue<Food>();
        currentCustomers = new Queue<UserDefault>();
        queuePos.Set(transform.position.x + 40, transform.position.y, transform.position.z);

        foreach (Cooker cooker in employees)
        {
            cooker.setShop(this);
        }
        maxCapacity = employees.Count;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(queuePos, 5);
    }

    // Update is called once per frame
    void Update()
    {
        if ((customers.Count > 0) && (currentCustomers.Count < maxCapacity))
        {
            order();
            currentCustomers.Enqueue(customers.Dequeue());
        }
    }

    public Vector3 getPos()
    {
        return transform.position;
    }

    public void addCustomer(UserDefault user)
    {
        customers.Enqueue(user);
    }

    public void order() {
       
        //Selecciona al primer empleado que no este cocinando y le pasa la orden
        foreach(Cooker employee in employees)
        {
            if (!employee.isWorking())
            {
                employee.prepareFood();
                break;
            }
        }
        
    }

    public void foodCooked(Food food)
    {
        UserDefault customer = currentCustomers.Dequeue();
        customer.giveFood(food);
    }
 
}
