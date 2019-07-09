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
        transform.position = new Vector3(parent.transform.position[0], 0f, parent.transform.position[2]);
    }

    // Update is called once per frame
    void Update()
    {
        if (System.Math.Abs(parent.GetComponent<chickenBrain>().visionRadius - scale) > 0)
        {
            scale = parent.GetComponent<chickenBrain>().visionRadius;
            transform.localScale = new Vector3(scale*2, 0.1f, scale*2);
        }

    }
}
