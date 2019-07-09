using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class chickenBrain : MonoBehaviour
{

	[Header("Chicken Values Limits")]
	public float minimumHealth;
	public float maximumHealth;
	public float minVisionRadius;
	public float maxVisionRadius;
	public float minimumSpeed;
	public float maximumSpeed;


	[Header("Chicken Characteristics")]
    public float directionChangeInterval;
    public float maxHeadingChange;
	public float visionRadius;
	public float healthLostTimeFactor;
	public float foodHealthGain;

	[Header("Unity Stuff")]
	public Image healthBar;
	public GameObject chickenInfo;

	float speed;
	float health;
	float maxHealthValue;
    float heading;

	bool foundFood = false;
	bool alive = true;

	CharacterController controller;
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

		maxHealthValue = Random.Range(minimumHealth, maximumHealth);
		health = maxHealthValue;

		speed = Random.Range(minimumSpeed, maximumSpeed);

		StartCoroutine(NewHeading());
    }

    void Update()
    {

		chickenInfo.transform.LookAt(Camera.main.gameObject.transform.position);

		if(alive){
			Vision();

			//heading = getDegrees(heading-Input.GetAxis("Horizontal")*15, 360);

			var rotation = getDegrees(getDegrees(-transform.eulerAngles[1] + 90f, 360) - heading, 180);
			//var rotation = Input.GetAxis("Horizontal")*180;

			transform.Rotate(0f, rotation * speed * Time.deltaTime, 0f, Space.Self);

			transform.position += transform.TransformDirection(Vector3.forward) * speed * Time.deltaTime;
			//transform.position += transform.TransformDirection(Vector3.forward) * speed * Input.GetAxis("Vertical") * Time.deltaTime;
		
			// update Health
			adjustHealth( -Time.deltaTime * healthLostTimeFactor);
			

		}


	}

	void adjustHealth(float value){
		health += value;
		if(health>maxHealthValue) health = maxHealthValue;
		if(health<=0){
			health=0;
			alive = false;
			anim.SetInteger("Walk", 0);
		} 
		adjustHealthBar();
	}

	void adjustHealthBar(){
		healthBar.fillAmount = health/maxHealthValue;
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


	void Eat(){
		adjustHealth(foodHealthGain);
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
			var angle = getDegrees(-transform.eulerAngles[1] + 90f, 360);
			if(Mathf.Abs(collision.contacts[0].normal[0])>0.0f){
				heading = getDegrees(180f - heading, 360);
				transform.Rotate(0f, getDegrees(angle-heading, 180), 0f, Space.Self);
			}else{
				heading = getDegrees(-heading, 360);
				transform.Rotate(0f, getDegrees(angle-heading, 180), 0f, Space.Self);
			}
		}

		if (collision.gameObject.tag == "Food")
		{
			Eat();
			NewHeadingRoutine();
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