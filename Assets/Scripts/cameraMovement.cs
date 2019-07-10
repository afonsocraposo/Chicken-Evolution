using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraMovement : MonoBehaviour
{

    Vector3 offset = new Vector3(10f, 5f, 10f);         //Private variable to store the offset distance between the player and camera

    public bool freeCamera = true;

    public float cameraSpeed;
    Vector2 movement = new Vector2(0f, 0f);

    public float mouseSpeed;

    float yaw = 0.0f;
    float pitch = 0.0f;
    private GameObject[] chickens;
    private int chicken = 0;

    Vector3 rotatePos;
    Vector3 prevRotatePos;

    // Start is called before the first frame update
    void Start()
    {
        chickens = GameObject.FindGameObjectsWithTag("Chicken");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("m")) freeCamera = !freeCamera;
        
        if (Input.GetKeyDown("c"))
        {
            chicken++;
            if (chicken >= chickens.Length) chicken = 0;
        }

        if (freeCamera) { 
            movement[0] = Input.GetAxisRaw("Horizontal");
            movement[1] = Input.GetAxisRaw("Vertical");

            if (movement != Vector2.zero)
            {

                movement = movement / (float)Math.Pow(Math.Pow(movement[0], 2) + Math.Pow(movement[1], 2), 0.5);

                transform.Translate(movement[0] * cameraSpeed * Time.deltaTime, 0f, movement[1] * cameraSpeed * Time.deltaTime);

            }
            yaw += mouseSpeed * Input.GetAxis("Mouse X");
            pitch -= mouseSpeed * Input.GetAxis("Mouse Y");

            transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);

        }
    }

    void LateUpdate()
    {
        if (!freeCamera)
        {
            // Set the position of the camera's transform to be the same as the player's, but offset by the calculated offset distance.
            transform.position = chickens[chicken].transform.position + offset;
            transform.LookAt(chickens[chicken].transform.position);
            transform.RotateAround(chickens[chicken].transform.position, Vector3.up, cameraSpeed * Input.GetAxis("Mouse X"));
            offset = transform.position - chickens[chicken].transform.position;
        }
    }
}
