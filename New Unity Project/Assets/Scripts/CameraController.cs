﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Position")]
    public Human followTarget;
    public Vector3 followingPoint = new Vector3(0, 1 , 0);
    public float cameraDistance = 1000;
    public float currentAngle = 45;
    public GameObject compass;

    [Header("Speed Settings")]
    public float movementSpeed = 5;
    public float rotateSpeed = 1.0f;
    public float zoomMultiplier = 10;

    [Header("Zoom Settings")]
    public float maxZoom = 400;
    public float minZoom = 50;

    [Header("Movement Restrictions")]
    public BoxCollider movementRestrictor;

    // Start is called before the first frame update
    void Start()
    {

        GetComponent<Camera>().farClipPlane = cameraDistance * 2;
        repositionCamera();

    }

    // Update is called once per frame
    void Update()
    {

        if (followTarget != null)
            followUser();

        movePoint();
        rotateAngle();
        checkScroll();

        repositionCamera();
        UpdateCompass();

    }

    private void followUser()
    {
        followingPoint = new Vector3(followTarget.transform.position.x, transform.position.y, followTarget.transform.position.z);
    }

    private void movePoint()
    {

        Vector3 moveDir = new Vector3(0, 0, 0);

        if (Input.GetKey(KeyCode.A))
        {
            moveDir += new Vector3(-1,0,0);
            followTarget = null;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveDir += new Vector3(1, 0, 0);
            followTarget = null;
        }
        if (Input.GetKey(KeyCode.W))
        {
            moveDir += new Vector3(0, 0, 1);
            followTarget = null;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveDir += new Vector3(0, 0, -1);
            followTarget = null;
        }

        moveDir.Normalize();
        moveDir = Quaternion.Euler(0, currentAngle, 0) * moveDir;

        followingPoint += moveDir * movementSpeed;

    }

    private void rotateAngle()
    {

        if (Input.GetKey(KeyCode.E))
        {
            currentAngle += -rotateSpeed;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            currentAngle += rotateSpeed;
        }

    }

    private void checkScroll()
    {

        GetComponent<Camera>().orthographicSize -= Input.mouseScrollDelta.y * zoomMultiplier;

        if (GetComponent<Camera>().orthographicSize > maxZoom)
        {
            GetComponent<Camera>().orthographicSize = maxZoom;
        }
        else if (GetComponent<Camera>().orthographicSize < minZoom)
        {
            GetComponent<Camera>().orthographicSize = minZoom;
        }

    }

    private void repositionCamera()
    {

        restrictCamera();

        Vector3 cameraPosition = new Vector3(0, cameraDistance, 0);
        cameraPosition = followingPoint + Quaternion.Euler(-45, currentAngle, 0) * cameraPosition;
        transform.position = cameraPosition;
        transform.rotation = Quaternion.Euler(45, currentAngle, 0);

    }

    private void restrictCamera()
    {

        if (movementRestrictor != null)
        {
            if (!movementRestrictor.bounds.Contains(followingPoint))
            {
                followingPoint = movementRestrictor.ClosestPointOnBounds(followingPoint);
            }
        }

    }

    private void UpdateCompass()
    {
        if (compass != null)
        {

            compass.transform.rotation = Quaternion.Euler(0,0,0);

            float cScale = GetComponent<Camera>().orthographicSize / maxZoom;

            compass.transform.localScale = new Vector3(cScale, cScale, cScale);

            float yCoord = GetComponent<Camera>().orthographicSize * 1 - cScale * 55;
            float xCoord = - GetComponent<Camera>().orthographicSize * 1 * GetComponent<Camera>().aspect + cScale * 60;

            compass.transform.localPosition = new Vector3(xCoord, yCoord, 100);

        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(followingPoint, 0.2f);
    }

}
