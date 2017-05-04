using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManagerScript : MonoBehaviour 
{
	public bool isSwitchingOFF;
	public bool isSwitchingON;
	public float speed;
	public GameObject[] torches;
	int count;
	float tmpSpeed;
	float actualRange;
	// Use this for initialization
	void Start()
	{
		count = torches.Length;

		
	}
	public void SwitchTheTorches()
	{
		for (int i = 0; i < count; i++) 
		{
			torches[i].GetComponentInChildren<Light>().range -= tmpSpeed;
			if (i == count - 1) 
			{
				actualRange = torches [i].GetComponentInChildren<Light> ().range;
			}
		}
		if (actualRange <= 5f && isSwitchingOFF) 
		{
			isSwitchingOFF = false;
		}
		if (actualRange >= 15f&& isSwitchingON) 
		{
			isSwitchingON = false;
		}
	}

	void Update()
	{
		if (isSwitchingOFF) 
		{
			tmpSpeed = 1f * speed * Time.deltaTime;
			SwitchTheTorches ();
		}
		if (isSwitchingON) 
		{
			tmpSpeed = -1f * speed * Time.deltaTime;
			SwitchTheTorches ();
		}
	}


}
