using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodShop : MonoBehaviour
{
    public List<Cooker> employees; //Creo que mejor lista que cola

    private Queue<Food> readyFood;
    private Queue<UserDefault> customers;
    // Start is called before the first frame update
    void Start()
    {
        customers = new Queue<UserDefault>();
        readyFood = new Queue<Food>();

        foreach (Cooker cooker in employees)
        {
            cooker.setShop(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 getPos()
    {
        return transform.position;
    }

    public void addCustomer(UserDefault user)
    {
        customers.Enqueue(user);
        order();
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
        UserDefault customer = customers.Dequeue();
        customer.giveFood(food);
    }
 
}
