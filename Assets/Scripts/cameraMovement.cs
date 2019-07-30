using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class cameraMovement : MonoBehaviour
{

    float logTimer = 1;

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

    bool displayInfo = false;

    bool paused;

    // Start is called before the first frame update
    void Start()
    {
        WriteLine("Assets/Resources/", "log.txt", "1 - Health, 2 - Vision, 3 - Speed, 4 - Hunger 5 - Population");
        Cursor.lockState = CursorLockMode.Locked;   // keep confined to center of screen
        PauseGame();
    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetKeyDown("p")) 
        {
            if (!paused) 
            {
                PauseGame();
            }else{
                ContinueGame();   
            }
        } 

        if(Input.GetKeyDown("escape"))
        {
            Application.Quit();
        }

        chickens = GameObject.FindGameObjectsWithTag("Chicken");

        if (Input.GetKeyDown("m")) freeCamera = !freeCamera;
        
        if (Input.GetKeyDown("c"))
        {
            chicken++;
        }

        if (Input.GetKeyDown("i"))
        {
            displayInfo=!displayInfo;
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

        chickens = GameObject.FindGameObjectsWithTag("Chicken");


        if (chicken >= chickens.Length) chicken = 0;

        if (!freeCamera)
        {
            // Set the position of the camera's transform to be the same as the player's, but offset by the calculated offset distance.
            transform.position = chickens[chicken].transform.position + offset;
            transform.LookAt(chickens[chicken].transform.position);
            transform.RotateAround(chickens[chicken].transform.position, Vector3.up, cameraSpeed * Input.GetAxis("Mouse X"));
            offset = transform.position - chickens[chicken].transform.position;
        }
        
        chickens[chicken].GetComponent<chickenBrain>().setDisplayInfo(displayInfo);

        logTimer -= Time.deltaTime;
        if(logTimer<=0){
            var healthStr = "";
            var visionStr = "";
            var speedStr = "";
            var hungerStr = "";
            
            for(int i = 0; i<chickens.Length; i++){
                var chick = chickens[i];
                healthStr += chick.GetComponent<chickenBrain>().getHealth() + ", ";
                visionStr += chick.GetComponent<chickenBrain>().getVisionRadius() + ", ";
                speedStr += chick.GetComponent<chickenBrain>().getSpeed() + ", ";
                hungerStr += chick.GetComponent<chickenBrain>().getHunger() + ", ";
            }

            AppendLine("Assets/Resources/", "log.txt", healthStr);
            AppendLine("Assets/Resources/", "log.txt", visionStr);
            AppendLine("Assets/Resources/", "log.txt", speedStr);
            AppendLine("Assets/Resources/", "log.txt", hungerStr);
            AppendLine("Assets/Resources/", "log.txt", chickens.Length.ToString());
            AppendLine("Assets/Resources/", "log.txt", "");
            logTimer = 1;
        }
        
    }

    public static bool AppendLine (string path, string fileName, string data)
     {
         bool retValue = false;
         try {
             if (!Directory.Exists (path))
                 Directory.CreateDirectory (path);
             System.IO.File.AppendAllText (path + fileName, data + Environment.NewLine);
             retValue = true;
         } catch (System.Exception ex) {
             string ErrorMessages = "File Write Error\n" + ex.Message;
             retValue = false;
             Debug.LogError (ErrorMessages);
         }
         return retValue;
     }

     public static bool WriteLine (string path, string fileName, string data)
     {
         bool retValue = false;
         try {
             if (!Directory.Exists (path))
                 Directory.CreateDirectory (path);
             System.IO.File.WriteAllText (path + fileName, data + Environment.NewLine);
             retValue = true;
         } catch (System.Exception ex) {
             string ErrorMessages = "File Write Error\n" + ex.Message;
             retValue = false;
             Debug.LogError (ErrorMessages);
         }
         return retValue;
     }

     private void PauseGame()
    {
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0;
        paused = true;
        //Disable scripts that still work while timescale is set to 0
    } 
    private void ContinueGame()
    {
        Cursor.lockState = CursorLockMode.Locked;   // keep confined to center of screen
        Time.timeScale = 1;
        //enable the scripts again
        paused = false;
    }
}
