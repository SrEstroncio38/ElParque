using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour
{

    [Header("Scene Settings")]
    public CameraController mainCamera;
    public Light sun;
    

    [Header("Canvas Settings")]
    public Camera userCamera;
    public GameObject noTargetHUD;
    public GameObject userHUD;
    public GameObject userHUDVariables;
    public SpeechObject speechObject;
    public GameObject speechLocation;

    [Header("Day Color Settings")]
    public Color dayColor = new Color(0.3921569f, 0.5019608f, 0.6745098f);
    public Color nightColor = new Color(0.1921569f, 0.3019608f, 0.4745098f);
    public Color dawnColor = new Color(0.5f, 0, 0);
    public Color duskColor = new Color(0.5f, 0, 0);

    [Header("Time Settings")]
    public float worldCurrentTime = 0;
    public float dawnTime = 700;
    public float duskTime = 2100;
    public float dayLength = 2400;
    public float duskLength = 50;
    public float timeStepMultiplier = 3;

    [Header("Music Settings")]
    public AudioClip dayTheme;
    public AudioClip nightTheme;
    public float dayVolume = 0.2f;
    public float nightVolume = 0.2f;

    [Header("Park Settings")]
    public int parkCapacity;

    // Canvas
    private Canvas canvas;

    private UnityEngine.UI.Image mouseHUD;
    private string mouseHUDText = "";

    private Human currentTarget;
    private UnityEngine.UI.Text charName;
    private UnityEngine.UI.Image userHUD1;
    private UnityEngine.UI.Image userHUD2;
    private UnityEngine.UI.Image userHUD3;
    private UnityEngine.UI.Image userHUD4;
    private UnityEngine.UI.Text userHUDState;
   

    // Start is called before the first frame update
    void Start()
    {

        InitCanvas();
        AdjustDisplay2();

    }

    private void InitCanvas()
    {
        canvas = GetComponentInChildren<Canvas>();

        foreach (UnityEngine.UI.Text t in userHUD.GetComponentsInChildren<UnityEngine.UI.Text>())
        {
            if (t.gameObject.name.Equals("User Name"))
                charName = t;
        }
        foreach (UnityEngine.UI.Image g in userHUD.GetComponentsInChildren<UnityEngine.UI.Image>())
        {
            if (g.gameObject.name.Equals("User HUD 1"))
                userHUD1 = g;
            else if (g.gameObject.name.Equals("User HUD 2"))
                userHUD2 = g;
            else if (g.gameObject.name.Equals("User HUD 3"))
                userHUD3 = g;
            else if (g.gameObject.name.Equals("User HUD 4"))
                userHUD4 = g;
        }
        foreach (UnityEngine.UI.Image g in canvas.GetComponentsInChildren<UnityEngine.UI.Image>())
        {
            if (g.gameObject.name.Equals("Mouse HUD"))
                mouseHUD = g;
        }

        foreach (UnityEngine.UI.Text t in userHUD4.GetComponentsInChildren<UnityEngine.UI.Text>())
        {
            if (t.name.Equals("Estado State"))
                userHUDState = t;
        }

        userHUDVariables.SetActive(false);
        userHUD.gameObject.SetActive(false);
        noTargetHUD.SetActive(false);
        mouseHUD.gameObject.SetActive(false);

    }

    private void AdjustDisplay2()
    {
        userCamera.pixelRect = new Rect(new Vector2(12,12), new Vector2(72,96));
        userCamera.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

        noTargetHUD.SetActive(false);
        UpdateDayTime();
        UpdateHUD();
        UpdateMouseHUD();
        UpdateMusic();

    }

    private void UpdateDayTime()
    {

        worldCurrentTime += Time.deltaTime * timeStepMultiplier;
        while (worldCurrentTime > dayLength)
        {
            worldCurrentTime -= dayLength;
        }

        Color backgroundColor = dayColor;
        if (worldCurrentTime < dawnTime || worldCurrentTime > duskTime)
        {
            backgroundColor = nightColor;
        }

        float duskProgress = Mathf.Abs(dawnTime - worldCurrentTime);
        if (duskProgress < duskLength)
        {
            float alpha1 = duskProgress / duskLength;
            float alpha2 = 1 - alpha1;
            backgroundColor = alpha1 * backgroundColor + alpha2 * dawnColor;
        }
        duskProgress = Mathf.Abs(duskTime - worldCurrentTime);
        if (duskProgress < duskLength)
        {
            float alpha1 = duskProgress / duskLength;
            float alpha2 = 1 - alpha1;
            backgroundColor = alpha1 * backgroundColor + alpha2 * duskColor;
        }

        mainCamera.GetComponent<Camera>().backgroundColor = backgroundColor;
        MoveSun();
        UpdateClock();

    }

    private void MoveSun()
    {
        float sunAngle = 0;
        float sunIntensity = 1;

        //Angle
        float dawnTime2 = dawnTime - duskLength * 0.5f;
        float duskTime2 = duskTime + duskLength * 0.5f;
        if (worldCurrentTime < duskTime2 && worldCurrentTime > dawnTime2)
        {
            sunAngle = 180 * (worldCurrentTime - dawnTime2) / (duskTime2 - dawnTime2);
        } else
        {
            float nightProgress = 0;
            if (worldCurrentTime >= duskTime2)
            {
                nightProgress = worldCurrentTime - duskTime2;
            } else
            {
                nightProgress = dayLength - duskTime2 + worldCurrentTime;
            }
            float nightLength = dayLength - duskTime2 + dawnTime2;
            sunAngle = 180 + 180 * nightProgress / nightLength;
        }

        //Intensity
        if (worldCurrentTime < duskTime2 && Mathf.Abs(worldCurrentTime - duskTime2) < duskLength)
        {
            sunIntensity = Mathf.Abs(worldCurrentTime - duskTime2) / duskLength;
        } else if (worldCurrentTime > dawnTime2 && Mathf.Abs(worldCurrentTime - dawnTime2) < duskLength)
        {
            sunIntensity = Mathf.Abs(worldCurrentTime - dawnTime2) / duskLength;
        } else if (worldCurrentTime < dawnTime || worldCurrentTime > duskTime)
        {
            sunIntensity = 0;
        }

        sun.transform.rotation = Quaternion.Euler(sunAngle, -30, 0);
        sun.intensity = sunIntensity;
    }

    private void UpdateClock()
    {

        GameObject clock = canvas.transform.Find("Clock").gameObject;

        string timeString = "";

        int firstDigits = (int)(24 * worldCurrentTime / dayLength);
        if (firstDigits < 10)
            timeString += "0";
        timeString += firstDigits + ":";

        int secondDigits = (int)(worldCurrentTime - firstDigits * dayLength / 24);
        secondDigits = (int)(secondDigits * 60.0f / 100.0f);
        if (secondDigits < 10)
            timeString += "0";
        timeString += secondDigits;

        clock.GetComponent<UnityEngine.UI.Text>().text = timeString;


    }

    private void UpdateHUD()
    {

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            if (currentTarget != null)
                SetLayerRecursively(currentTarget.gameObject, 9);
            currentTarget = null;
            userHUD.gameObject.SetActive(false);
            userCamera.enabled = false;
        }
        if (currentTarget == null)
            return;

        //Update
        UserDefault user = null;
        try
        {
            user = (UserDefault) currentTarget;
        } catch (Exception e) { }
        if (user != null)
        {
            userHUDVariables.SetActive(true);
            UpdateHUDVariables();
        } else
        {
            userHUDVariables.SetActive(false);
        }
        userHUDState.text = currentTarget.currentState;

        if (!currentTarget.gameObject.activeSelf)
        {
            noTargetHUD.SetActive(true);
        }

    }

    private void UpdateHUDVariables()
    {
        UnityEngine.UI.RawImage saciedad = null;
        float saciedadWidth = 0;
        UnityEngine.UI.RawImage tolerancia = null;
        float toleranciaWidth = 0;
        UnityEngine.UI.RawImage vejiga = null;
        float vejigaWidth = 0;
        UnityEngine.UI.RawImage bienestar = null;
        float bienestarWidth = 0;

        foreach (UnityEngine.UI.RawImage i in userHUDVariables.GetComponentsInChildren<UnityEngine.UI.RawImage>())
        {
            if (i.gameObject.name.Equals("Saciedad Inactive"))
            {
                saciedadWidth = i.rectTransform.sizeDelta.x;
            }
            else if (i.gameObject.name.Equals("Tolerancia Inactive"))
            {
                toleranciaWidth = i.rectTransform.sizeDelta.x;
            }
            else if (i.gameObject.name.Equals("Vejiga Inactive"))
            {
                vejigaWidth = i.rectTransform.sizeDelta.x;
            }
            else if (i.gameObject.name.Equals("Bienestar Inactive"))
            {
                bienestarWidth = i.rectTransform.sizeDelta.x;
            }

            else if (i.gameObject.name.Equals("Saciedad Active"))
            {
                saciedad = i;
            }
            else if (i.gameObject.name.Equals("Tolerancia Active"))
            {
                tolerancia = i;
            }
            else if (i.gameObject.name.Equals("Vejiga Active"))
            {
                vejiga = i;
            }
            else if (i.gameObject.name.Equals("Bienestar Active"))
            {
                bienestar = i;
            }
        }

        if (saciedad != null)
        {
            float saciedadValue = ((UserDefault)currentTarget).saciedad;
            saciedad.rectTransform.sizeDelta = new Vector2(saciedadValue * saciedadWidth / 100, 8);
            float currentColor = saciedadValue / 200.0f + 0.5f;
            saciedad.color = new Color(1, currentColor, currentColor);
        }
        if (tolerancia != null)
        {
            float toleranciaValue = ((UserDefault)currentTarget).tolerancia;
            tolerancia.rectTransform.sizeDelta = new Vector2(toleranciaValue * toleranciaWidth / 100, 8);
            float currentColor = toleranciaValue / 200.0f + 0.5f;
            tolerancia.color = new Color(1, currentColor, currentColor);
        }
        if (vejiga != null)
        {
            float vejigaValue = ((UserDefault)currentTarget).vejiga;
            vejiga.rectTransform.sizeDelta = new Vector2(vejigaValue * vejigaWidth / 100, 8);
            float currentColor = vejigaValue / 200.0f + 0.5f;
            vejiga.color = new Color(1, currentColor, currentColor);
        }
        if (bienestar != null)
        {
            float bienestarValue = ((UserDefault)currentTarget).bienestar;
            bienestar.rectTransform.sizeDelta = new Vector2(bienestarValue * bienestarWidth / 100, 8);
            float currentColor = bienestarValue / 200.0f + 0.5f;
            bienestar.color = new Color(1, currentColor, currentColor);
        }
    }

    public void SetCursorText(string text)
    {
        mouseHUDText = text;
    }

    private void UpdateMouseHUD()
    {
        if (mouseHUDText.Equals(""))
        {
            mouseHUD.gameObject.SetActive(false);
        } else
        {
            mouseHUD.gameObject.SetActive(true);
            UnityEngine.UI.Text textBox = mouseHUD.GetComponentInChildren<UnityEngine.UI.Text>();
            textBox.text = mouseHUDText;
            mouseHUD.rectTransform.sizeDelta = new Vector2(textBox.preferredWidth + 16, mouseHUD.rectTransform.sizeDelta.y);
            mouseHUD.rectTransform.position = Input.mousePosition + new Vector3(8, 0, -8);
        }

        mouseHUDText = "";
    }

    public void SetHUDTarget(Human target)
    {
        foreach (Human nonTarget in GetComponentsInChildren<Human>())
        {
            SetLayerRecursively(nonTarget.gameObject, 9);
        }
        SetLayerRecursively(target.gameObject, 10);
        userCamera.transform.parent = target.transform;
        float distance = 75;
        float height = target.GetComponent<CapsuleCollider>().center.y;
        distance = distance / target.transform.localScale.z;
        userCamera.transform.localPosition = Quaternion.Euler(0, 40, 0) * new Vector3(0, height, distance);
        userCamera.transform.localScale = new Vector3(1, 1, 1);
        userCamera.transform.localRotation = Quaternion.Euler(0, 220, 0);
        userCamera.enabled = true;

        currentTarget = target;
        charName.text = target.name;

        float x = charName.preferredWidth + 16;

        userHUD1.rectTransform.sizeDelta = new Vector2(x, userHUD1.rectTransform.sizeDelta.y);
        userHUD2.rectTransform.position = new Vector2(x, userHUD2.rectTransform.position.y);
        userHUD2.rectTransform.sizeDelta = new Vector2(canvas.GetComponent<RectTransform>().rect.width - x, userHUD2.rectTransform.sizeDelta.y);

        x = userHUD3.rectTransform.sizeDelta.x;
        userHUD4.rectTransform.position = new Vector2(x, userHUD4.rectTransform.position.y);
        userHUD4.rectTransform.sizeDelta = new Vector2(canvas.GetComponent<RectTransform>().rect.width - x, userHUD4.rectTransform.sizeDelta.y);

        userHUD.gameObject.SetActive(true);
        userCamera.enabled = true;

        
    }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    public void ShowEmoticon(string emoticon, GameObject target, float time)
    {
        SpeechObject so = Instantiate(speechObject);
        so.transform.SetParent(speechLocation.transform);
        so.Init(emoticon,target,mainCamera.gameObject.GetComponent<Camera>(),time);
    }

    private void UpdateMusic()
    {

        AudioSource audioSource = gameObject.GetComponent<AudioSource>();

        AudioClip currentSong = dayTheme;
        float currentVolume = dayVolume;
        if (worldCurrentTime < dawnTime || worldCurrentTime > duskTime)
        {
            currentSong = nightTheme;
            currentVolume = nightVolume;
        }

        if (audioSource.clip != currentSong)
        {
            audioSource.clip = currentSong;
            audioSource.Play();
        }

        audioSource.volume = currentVolume;

    }
   
}
