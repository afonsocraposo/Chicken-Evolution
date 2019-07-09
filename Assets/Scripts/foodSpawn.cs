using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class foodSpawn : MonoBehaviour
{
    private GameObject floor;
    float minX;
    float maxX;
    float minZ;
    float maxZ;

    // Start is called before the first frame update
    void Start()
    {
        floor = GameObject.FindGameObjectWithTag("Floor");
        var scale = floor.transform.localScale;
        minX = 0.9f*(-5 * scale[0]);
        maxX = 0.9f*(5 * scale[0]);
        minZ = 0.9f*(-5 * scale[2]);
        maxZ = 0.9f*(5 * scale[2]);

        randomPosition();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag == "Chicken")
        {
            randomPosition();
        }
    }

    void randomPosition()
    {
        transform.position = new Vector3(Random.Range(minX, maxX), transform.position[1], Random.Range(minZ, maxZ));   
    }
}
