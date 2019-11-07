using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour
{

    public CameraController mainCamera;
    public Light sun;

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

    //Private

    private Canvas canvas;

    // Start is called before the first frame update
    void Start()
    {
        canvas = GetComponentInChildren<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {

        updateDayTime();

    }

    private void updateDayTime()
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
        moveSun();
        updateClock();

    }

    private void moveSun()
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

    private void updateClock()
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
}
