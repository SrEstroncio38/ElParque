﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cooker : Worker
{
    private bool working = false;
    private FoodShop foodShop;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setShop(FoodShop fs)
    {
        this.foodShop = fs;
    }
    public bool isWorking() {
        return working;
    }

    public void prepareFood()
    {
        working = true;
        StartCoroutine(cook());
    }

    IEnumerator cook() {
        yield return new WaitForSeconds(2);
        foodShop.foodCooked(new Food());
        working = false;
    }
}