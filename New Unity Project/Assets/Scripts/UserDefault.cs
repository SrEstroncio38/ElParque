using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserDefault : MonoBehaviour
{

    [Header("Properties")]
    public float saciedad = 100.0f;
    public float tolerancia = 100.0f;
    public float vejiga = 100.0f;
    public float bienestar = 100.0f;

    private WorldController world;

    // Start is called before the first frame update
    void Start()
    {
        world = GetComponentInParent<WorldController>();
    }

    // Update is called once per frame
    void Update()
    {

        calcularBienestar();

    }

    private void pasear()
    {

    }

    private void calcularBienestar()
    {

        float s = saciedad * Mathf.Pow(saciedad, 0.25f);
        float t = tolerancia * Mathf.Pow(tolerancia, 0.25f);
        float v = vejiga * Mathf.Pow(vejiga, 0.25f);

        bienestar = 100 * (t + s + v) / (3 * 100 * Mathf.Pow(100, 0.25f));

    }

    void OnMouseDown()
    {
        world.mainCamera.followTarget = GetComponent<UserDefault>();
    }
}

