using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DayNightCycle : NetworkBehaviour {

	[SyncVar]public float speed = 0f;
	public float nightSpeedFactor = 0.4f;
	public bool isNight;
	public float timeOfDay = 1;
	public AudioClip Night;
	public AudioClip Day;

	bool firstdayTrigger;
	// Use this for initialization
	void Start () 
	{
		
	}

	// Update is called once per frame
	void FixedUpdate () 
	{
		float x = 1f * speed * Time.fixedDeltaTime;
		transform.Rotate (x, 0f, 0f);
		timeOfDay -= x;
		if (!firstdayTrigger && timeOfDay>2f) 
		{
			isNight = true;
			GetComponent<AudioSource> ().PlayOneShot (Night);
			GameManager.instanceGM.nightTime = true;
			speed -= nightSpeedFactor;
			firstdayTrigger = true;
		}

		if (timeOfDay > 180f ) 
		{
			timeOfDay = 0f;
			if (isNight) 
			{
				isNight = false;
				GetComponent<AudioSource> ().PlayOneShot (Day);
				speed += nightSpeedFactor;
				if (isServer) {
					GameManager.instanceGM.nightTime = false;
				}
				return;
			}
			else 
			{

				GetComponent<AudioSource> ().PlayOneShot (Night);
				if (isServer) {
					GameManager.instanceGM.nightTime = true;
				}				
				speed -= nightSpeedFactor;
				isNight = true;
			}

		}
			
	
	}
}
