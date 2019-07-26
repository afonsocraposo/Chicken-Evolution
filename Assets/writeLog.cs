using UnityEngine;
using UnityEditor;
using System.IO;

public class writeLog : MonoBehaviour
{

    StreamWriter writer = new StreamWriter("Assets/Resources/log.txt", true);

    float timeCounter = 1;
    
    void WriteString()
    {
        var chickens = GameObject.FindGameObjectsWithTag("Chicken");
        writer.WriteLine(chickens.Length.ToString());
    }

    void Update(){
        timeCounter -= Time.deltaTime;
        if(timeCounter<=0){
            WriteString();
            timeCounter = 1;
        }
    }

}