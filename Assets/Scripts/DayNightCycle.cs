using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DayNightCycle : NetworkBehaviour {

	[SyncVar]public float speed = 0f;
	public int nightSpeedFactor = 2;
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
			GameManager.instanceGM.nightTime = true;
			speed *= nightSpeedFactor;
		}
		if (transform.rotation.eulerAngles.x > 0f && transform.rotation.eulerAngles.x <180f&& isNight) 
		{
			isNight = false;
			GameManager.instanceGM.nightTime = false;
			speed /= nightSpeedFactor;
		}
	
	}
}
