using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechObject : MonoBehaviour
{

    public float aliveTimer = 3;
    public GameObject target = null;
    private Camera camara = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeSelf)
            aliveTimer -= Time.deltaTime;

        if (target != null && camara != null)
            UpdatePos();

        if (aliveTimer < 0)
            Destroy(gameObject);
    }

    private void UpdatePos()
    {
        transform.position = camara.WorldToScreenPoint(target.transform.position);
    }

    public void Init(string emoticon, GameObject target, Camera camera, float time)
    {
        this.camara = camera;
        this.target = target;
        this.aliveTimer = time;

        UpdatePos();
        gameObject.SetActive(true);
    }
}
