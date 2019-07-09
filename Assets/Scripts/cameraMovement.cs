using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraMovement : MonoBehaviour
{

    public float cameraSpeedMovement= 10.0f;
    Vector2 movement = new Vector2(0f, 0f);

    public float cameraSpeedRotation = 10.0f;

    float yaw = 0.0f;
    float pitch = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        movement[0] = Input.GetAxisRaw("Horizontal");
        movement[1] = Input.GetAxisRaw("Vertical");

        if (movement != Vector2.zero)
        {

            movement = movement / (float)Math.Pow(Math.Pow(movement[0], 2) + Math.Pow(movement[1], 2), 0.5);

            transform.Translate(movement[0] * cameraSpeedMovement * Time.deltaTime, 0f, movement[1] * cameraSpeedMovement * Time.deltaTime);

        }
        yaw += cameraSpeedRotation * Input.GetAxis("Mouse X");
        pitch -= cameraSpeedRotation * Input.GetAxis("Mouse Y");

        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
    }
}
