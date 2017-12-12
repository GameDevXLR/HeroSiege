using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManagerScript : MonoBehaviour 
{
	public Light moonLight;

//	public bool isSwitchingOFF;
	public bool isSwitchingON;
	public float speed;
	public GameObject[] torches;
	int count;
	float tmpSpeed;
	float actualRange;
	// Use this for initialization

	public static LightManagerScript lightM;

	void Start()
	{
		count = torches.Length;
		if (lightM == null) {
			lightM = this;	
			
		} else 
		{
			Destroy (this);
		}

		
	}
	public void SwitchTheTorches()
	{
		
		isSwitchingON = !isSwitchingON;
		if (isSwitchingON) {
			foreach (GameObject t in torches) {
				t.GetComponent<Light> ().intensity = 3f;
			}
		} else {
			foreach (GameObject t in torches) {
				t.GetComponent<Light> ().intensity = 0f;
			}
		}
	}

	public void DelayedSwitch()
	{
		
	}
//		if (isSwitchingOFF) 
//		{
//			moonLight.intensity -= (tmpSpeed/4f);
//			if (moonLight.intensity < 0f) 
//			{
//				moonLight.intensity = 0f;
//			}
//		}
//			moonLight.intensity -= (tmpSpeed/4f);
//			if (moonLight.intensity >3f) 
//			{
//				moonLight.intensity = 3f;
//			}
//		}
//		for (int i = 0; i < count; i++) 
//		{
////			torches[i].GetComponentInChildren<Light>().range -= tmpSpeed *.1f;
////			if (i == count - 1) 
////			{
//				Light L = torches [i].GetComponentInChildren<Light> ();
//				L.intensity -= tmpSpeed *.1f
////				actualRange = torches [i].GetComponentInChildren<Light> ().range;
////			}
////		}
//		if (actualRange <= 5f && isSwitchingOFF) 
//		{
//			isSwitchingOFF = false;
//			moonLight.intensity = 0f;
//		}
//		if (actualRange >= 15f&& isSwitchingON) 
//		{
//			isSwitchingON = false;
//			moonLight.intensity = 1f;
//				}
//				}
//	}
//
//	void Update()
//	{
//		if (isSwitchingOFF) 
//		{
//			tmpSpeed = 1f * speed * Time.deltaTime;
//			SwitchTheTorches ();
//		}
//		if (isSwitchingON) 
//		{
//			tmpSpeed = -1f * speed * Time.deltaTime;
//			SwitchTheTorches ();
//		}
//	}


}
