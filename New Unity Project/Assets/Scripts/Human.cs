﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : MonoBehaviour
{

    [Header("Identity")]
    public bool isMale = true;
    public string name = "";
    public string currentState = "";

    protected WorldController world;

    // Start is called before the first frame update
    void Start()
    {
        NameCreator.PersonName p = NameCreator.Generate();
        isMale = p.isMale;
        name = p.name;
        gameObject.name = name;
        world = GetComponentInParent<WorldController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseDown()
    {
        world.mainCamera.followTarget = GetComponent<Human>();
        world.SetHUDTarget(GetComponent<Human>());
    }

    protected void ShowEmoticon(string emoticon, float time)
    {
        world.ShowEmoticon(emoticon, gameObject, time);
    }

    protected void ShowEmoticon(string emoticon)
    {
        ShowEmoticon(emoticon, 3);
    }
}
