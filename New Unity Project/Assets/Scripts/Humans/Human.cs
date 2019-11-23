using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : MonoBehaviour
{

    [Header("Identity")]
    public bool isMale = true;
    public string userName = "";
    public string currentState = "";
    public bool isAlive = true;
    public Corpse skeleton;
    public GameObject skeletonDestination;

    protected WorldController world;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        NameCreator.PersonName p = NameCreator.Generate();
        isMale = p.isMale;
        userName = p.name;
        gameObject.name = userName;
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

    void OnMouseOver()
    {
        world.SetCursorText(userName);
    }

    protected void ShowEmoticon(string emoticon, float time)
    {
        world.ShowEmoticon(emoticon, gameObject, time);
    }

    protected void ShowEmoticon(string emoticon)
    {
        ShowEmoticon(emoticon, 3);
    }
    
    public virtual void Kill()
    {

        Corpse c = Instantiate(skeleton, transform.position, transform.rotation, skeletonDestination.transform);
        isAlive = false;
        c.gameObject.name = "Corpse (" + userName + ")";
        c.gameObject.SetActive(true);
    }
}
