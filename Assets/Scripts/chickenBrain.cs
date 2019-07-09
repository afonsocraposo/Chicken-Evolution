using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chickenBrain : MonoBehaviour
{

    public float speed = 5;
    public float directionChangeInterval = 1;
    public float maxHeadingChange = 30;

	public float minVisionRadius;
	public float maxVisionRadius;
	public float visionRadius;
	public float rotationSpeed;

	bool foundFood = false;

	CharacterController controller;
    float heading;
    Vector3 targetRotation;

    Animator anim;


    void Start()
    {

        anim = GetComponent<Animator>();

        // Set random initial rotation
        heading = Random.Range(0, 360);
        transform.eulerAngles = new Vector3(0, heading, 0);

        anim.SetInteger("Walk", 1);

		visionRadius = Random.Range(minVisionRadius, maxVisionRadius);
		//visionRadius = 10f;

		StartCoroutine(NewHeading());

    }

    void Update()
    {
		Vision();

		var rotation = getDegrees(getDegrees(-transform.eulerAngles[1] + 90f, 360) - heading, 180);
		//var rotation = Input.GetAxis("Horizontal")*180;

		transform.Rotate(0f, rotation * rotationSpeed * Time.deltaTime, 0f, Space.Self);

	    transform.position += transform.TransformDirection(Vector3.forward) * speed * Time.deltaTime;
		//transform.position += transform.TransformDirection(Vector3.forward) * speed * Input.GetAxis("Vertical") * Time.deltaTime;
	}

	/// <summary>
	/// Repeatedly calculates a new direction to move towards.
	/// Use this instead of MonoBehaviour.InvokeRepeating so that the interval can be changed at runtime.
	/// </summary>
	IEnumerator NewHeading()
    {
        while (true)
        {
            NewHeadingRoutine();
            yield return new WaitForSeconds(directionChangeInterval);
        }
    }


    void NewHeadingRoutine()
    {
		if (!foundFood)
		{
			heading = getDegrees(Random.Range(heading - maxHeadingChange / 2, heading + maxHeadingChange / 2), 360);
		}
    }

	void OnCollisionEnter(Collision collision)
	{ 
        
		if (collision.gameObject.tag == "Wall" || (collision.gameObject.tag == "Chicken" && !foundFood))
		{
			// Set the altered rotation back
			transform.Rotate(0f, 180f, 0f, Space.Self);

			heading = getDegrees(heading + 180, 360);
            
		}
	}

     int DistanceSort(Collider a, Collider b){
         return (transform.position - a.transform.position).sqrMagnitude.CompareTo((transform.position - b.transform.position).sqrMagnitude);
     }

    void Vision()
	{
		Collider[] hitColliders = Physics.OverlapSphere(new Vector3(transform.position[0], 0f, transform.position[2]), visionRadius);
	    // order by proximity
	    System.Array.Sort(hitColliders, DistanceSort);

	    foundFood = false;

		foreach (Collider collider in hitColliders)
		{
			switch (collider.tag)
			{
				case "Wall":
					break;
				case "Floor":
					break;
				case "Chicken":
					break;
				case "Food":
					if (!foundFood)
					{
						foundFood = true;
						var x1 = transform.position[0];
						var x2 = collider.gameObject.transform.position[0];
						var y1 = transform.position[2];
						var y2 = collider.gameObject.transform.position[2];
						heading = getDegrees(getAngle(x1, x2, y1, y2), 360);
					}
					return;
				default:
					break;
			}
		}
	}

	public float getDegrees(float degree, int mode)
    {
		if (mode == 360)
		{
			if (degree >= 360)
			{
				return degree % 360;
			}
			else if (degree >= 0)
			{
				return degree;
			}
			else
			{
				return getDegrees(degree + 360f, 360);
			}
		}else if(mode == 180)
		{
			if (degree >= 180)
			{
				return getDegrees(degree - 360, 180);
			}
			else if (degree >= -180)
			{
				return degree;
			}
			else
			{
				return getDegrees(degree + 360f, 180);
			}
		}
		else
		{
			return 0f;
		}
    }

    public float getAngle(float x1, float x2, float y1, float y2)
	{
        return Mathf.Atan2(y2 - y1, x2-x1) * Mathf.Rad2Deg;
	}


}