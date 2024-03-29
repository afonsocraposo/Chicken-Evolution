﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class chickenBrain : MonoBehaviour
{

	[Header("Chicken Values")]
	public float healthMean;
	public float healthStd;
	public float visionRadiusMean;
	public float visionRadiusStd;
	public float speedMean;
	public float speedStd;
	public float hungerMean;
	public float hungerStd;
	public float abstinenciaTime;


	[Header("Chicken Interactions")]
    public float directionChangeInterval;
    public float maxHeadingChange;
	public float foodHealthGain;

	[Header("Unity Stuff")]
	public Image healthBar;
	public GameObject chickenInfo;
    public Text nameTagText;
	public GameObject statsInfo;
	public Text healthText;
	public Text visionText;
	public Text speedText;
	public Text hungerText;

    float visionRadius;
    float speed;
	float health;
	float maxHealth;
    float heading;
	float hunger;

	float destroyTimer = 3;

    Quaternion rotation;

	bool foundFood = false;
	bool foundChicken = false;
	bool alive = true;
	bool readySex = false;
	
	float abstinenciaTimer;

    string path = "Assets/Resources/names.txt";
    string nameTag;

    CharacterController controller;
    Vector3 targetRotation;
    Animator anim;


    void Start()
    {

        anim = GetComponent<Animator>();

        // Set random initial rotation
        heading = Random.Range(0, 360);
        transform.eulerAngles = new Vector3(0, heading, 0);	

		
		visionRadius = RandomFromDistribution.RandomNormalDistribution(visionRadiusMean, visionRadiusStd);
		if (visionRadius < 0) visionRadius = 0;

		maxHealth = RandomFromDistribution.RandomNormalDistribution(healthMean, healthStd);
		if (health < 0) health = 0;
		health = maxHealth;

		speed = RandomFromDistribution.RandomNormalDistribution(speedMean, speedStd);
		if (speed <= 0)
		{
			speed = 0;
			anim.SetInteger("Walk", 0);
		}
		else
		{
			anim.SetInteger("Walk", 1);
		}
		// speed = 10;

		hunger = RandomFromDistribution.RandomNormalDistribution(hungerMean, hungerStd);
		if (hunger < 1) hunger = 1;

	

        StartCoroutine(NewHeading());

        string[] lines = System.IO.File.ReadAllLines(path);

        nameTag = lines[Random.Range(0, lines.Length)];
        nameTagText.text = nameTag;

		healthText.text = "Health: " + maxHealth.ToString("0.00");
		visionText.text = "Vision: " + visionRadius.ToString("0.00");
		speedText.text = "Speed: " + speed.ToString("0.00");
		hungerText.text = "Hunger: " + hunger.ToString("0.00");

		abstinenciaTimer = abstinenciaTime;

    }


    void Update()
    {

		statsInfo.SetActive(false);
		statsInfo.transform.LookAt(Camera.main.gameObject.transform.position);
		chickenInfo.transform.LookAt(Camera.main.gameObject.transform.position);

		if(alive){
			Vision();

			//heading = getDegrees(heading-Input.GetAxis("Horizontal")*15, 360);

			var rot = getDegrees(getDegrees(-transform.eulerAngles[1] + 90f, 360) - heading, 180);
			//var rotation = Input.GetAxis("Horizontal")*180;

			transform.Rotate(0f, rot * speed * Time.deltaTime, 0f, Space.Self);

			transform.position += transform.TransformDirection(Vector3.forward) * speed * Time.deltaTime;
			//transform.position += transform.TransformDirection(Vector3.forward) * speed * Input.GetAxis("Vertical") * Time.deltaTime;
		
			// update Health
			adjustHealth( -Time.deltaTime * hunger);

			if(!readySex){
				abstinenciaTimer-=Time.deltaTime;
				if(abstinenciaTimer<=0){
					readySex = true;
					abstinenciaTimer = abstinenciaTime;
				}
			}

        }
        else
        {
            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(transform.eulerAngles[0], transform.eulerAngles[1], 90f), 3 * Time.deltaTime);
            transform.position = new Vector3(transform.position[0], 0.5f, transform.position[2]);
			destroyTimer -= Time.deltaTime;
			if(destroyTimer<=0){
				Destroy(gameObject);
			}
        }


	}

    void adjustHealth(float value){
		health += value;
		if(health>maxHealth) health = maxHealth;
		if(health<=0){
			health=0;
			alive = false;
            GetComponent<Animator>().enabled = false;
        } 
		adjustHealthBar();
	}

	void adjustHealthBar(){
		healthBar.fillAmount = health/maxHealth;
	}

    public float getVisionRadius()
    {
        return visionRadius;
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
		if (!foundFood && !foundChicken)
		{
			heading = getDegrees(Random.Range(heading - maxHeadingChange / 2, heading + maxHeadingChange / 2), 360);
		}
    }

    void OnCollisionEnter(Collision collision)
    {
        if (alive) { 
            if (collision.gameObject.tag == "Wall" || (collision.gameObject.tag == "Chicken" && !foundFood && !foundChicken))
            {
                var d = transform.forward;
                var n = collision.contacts[0].normal;
                var r = d - 2 * Vector3.Dot(d, n) * n;

                heading = getDegrees(getAngle(0, r[0], 0, r[2]), 360);

                var angle = getDegrees(getAngle(0, d[0], 0, d[2]), 360);

                transform.Rotate(0f, getDegrees(angle - heading, 180), 0f, Space.Self);

            }

			
			if (collision.gameObject.tag == "Chicken" && isReadyToSex() && collision.gameObject.GetComponent<chickenBrain>().isReadyToSex())
			{
				readySex = false;
				collision.gameObject.GetComponent<chickenBrain>().readySex = false;
				var clone = Instantiate(this);
				var cloneVisionRadiusMean = (visionRadiusMean+collision.gameObject.GetComponent<chickenBrain>().visionRadiusMean)/2;
				var cloneHealthMean = (healthMean+collision.gameObject.GetComponent<chickenBrain>().healthMean)/2;
				var cloneSpeedMean = (speedMean+collision.gameObject.GetComponent<chickenBrain>().speedMean)/2;
				var cloneHungerMean = (hungerMean+collision.gameObject.GetComponent<chickenBrain>().hungerMean)/2;
				clone.GetComponent<chickenBrain>().setChildAttributes(cloneVisionRadiusMean, cloneHealthMean, cloneSpeedMean, cloneHungerMean); 

			}
			
			

            if (collision.gameObject.tag == "Food" && collision.gameObject.GetComponent<foodSpawn>().isReadyToEat())
            {
                Eat();
                NewHeadingRoutine();
            }
        }
	}

	int DistanceSort(Collider a, Collider b){
		return (transform.position - a.transform.position).sqrMagnitude.CompareTo((transform.position - b.transform.position).sqrMagnitude);
	}

	public void setChildAttributes(float visionRadiusMean, float healthMean, float speedMean, float hungerMean){
		
		anim = GetComponent<Animator>();

		this.visionRadiusMean = visionRadiusMean;
		this.visionRadiusStd = Mathf.Abs(visionRadius-visionRadiusMean)/2;
		//this.visionRadiusStd = Mathf.Abs(0.1f*visionRadiusMean);
		
		this.hungerMean = hungerMean;
		this.hungerStd = Mathf.Abs(hunger-hungerMean)/2;
		//this.hungerStd = Mathf.Abs(0.1f*hungerMean);

		this.healthMean = healthMean;
		this.healthStd = Mathf.Abs(health-healthMean)/2;
		//this.healthStd = Mathf.Abs(0.1f*healthMean);

		this.speedMean = speedMean;
		this.speedStd = Mathf.Abs(speed-speedMean)/2;
		//this.speedStd = Mathf.Abs(0.1f*speedMean);
	}

    void Vision()
	{
		Collider[] hitColliders = Physics.OverlapSphere(new Vector3(transform.position[0], 0f, transform.position[2]), visionRadius);
	    // order by proximity
	    System.Array.Sort(hitColliders, DistanceSort);

	    foundFood = false;
		foundChicken = false;

		foreach (Collider col in hitColliders)
		{
			switch (col.tag)
			{
				case "Wall":
					break;
				case "Floor":
					break;
				case "Chicken":
					if ((col.gameObject!=gameObject) && isReadyToSex() && col.gameObject.GetComponent<chickenBrain>().isReadyToSex())
						{
						foundChicken = true;
						var x1 = transform.position[0];
						var x2 = col.gameObject.transform.position[0];
						var y1 = transform.position[2];
						var y2 = col.gameObject.transform.position[2];
						heading = getDegrees(getAngle(x1, x2, y1, y2), 360);
						return;
					}
					break;
				case "Food":
					if (col.gameObject.GetComponent<foodSpawn>().isReadyToEat() && health<=0.9*maxHealth)
					{
						foundFood = true;
						var x1 = transform.position[0];
						var x2 = col.gameObject.transform.position[0];
						var y1 = transform.position[2];
						var y2 = col.gameObject.transform.position[2];
						heading = getDegrees(getAngle(x1, x2, y1, y2), 360);
					}
					return;
				default:
					break;
			}
		}
	}

	public float getHealth(){
		return maxHealth;
	}

	public float getSpeed(){
		return speed;
	}

	public float getHunger(){
		return hunger;
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

	public bool isReadyToSex(){
		return readySex && health>=0.5*maxHealth && alive;
	}

    public float getAngle(float x1, float x2, float y1, float y2)
	{
        return Mathf.Atan2(y2 - y1, x2-x1) * Mathf.Rad2Deg;
	}

	public void setDisplayInfo(bool displayInfo){
		statsInfo.SetActive(displayInfo);
	}

}