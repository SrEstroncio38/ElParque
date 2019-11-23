using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engineer : Worker
{
    private enum STATE_Working {ESPERANDO, DIRIGIENDOSE_ATRACCION, ARREGLANDO_ATRACCION, LLAMAR_TECNICO };
    private STATE_Working estado_trabajo = STATE_Working.ESPERANDO;
    private float quality;
    private float thresholdQuality = 30.0f;
    // Update is called once per frame
    void Update()
    {
        
    }
}
