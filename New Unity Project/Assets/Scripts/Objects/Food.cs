using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food 
{
    private float quality;
    private float BAD_QUALITY = 30.0f;
    public Food() {
        this.quality = Random.Range(0, 100);
    }

    public bool isGood()
    {
        return quality>=BAD_QUALITY;
    }
}
