using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class visionRadius : MonoBehaviour
{
    private GameObject parent;
    private float scale;

    // Start is called before the first frame update
    void Start()
    {
        parent = transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (System.Math.Abs(parent.GetComponent<chickenBrain>().getVisionRadius() - scale) > 0)
        {
            scale = parent.GetComponent<chickenBrain>().getVisionRadius();
            transform.localScale = new Vector3(scale*2, 0.1f, scale*2);
        }

        transform.rotation = new Quaternion(0f, 0f, transform.rotation[2], 0f);
        transform.position = new Vector3(parent.transform.position[0], 0f, parent.transform.position[2]);

    }
}
