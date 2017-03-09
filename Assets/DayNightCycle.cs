using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour {

	public float speed = 0.05f;
	public bool isNight;
	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () 
	{
		transform.Rotate (1f * speed * Time.deltaTime, 0f, 0f);

		if (transform.rotation.eulerAngles.x > 180f && isNight == false) 
		{
			isNight = true;
		}
		if (transform.rotation.eulerAngles.x > 0f && transform.rotation.eulerAngles.x <180f&& isNight) 
		{
			isNight = false;
		}
	
	}
}
