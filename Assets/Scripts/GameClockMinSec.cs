using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameClockMinSec : MonoBehaviour 
{
	public int totalTime;
	public int minutes;
	public int seconds;
	string secondsTmp;
	string minutesTmp;

	// Use this for initialization
	void Start () 
	{
		StartCoroutine (ActivateTimer ());	
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	IEnumerator ActivateTimer ()
	{
		while (true) 
		{
			yield return new WaitForSecondsRealtime (1f);
			totalTime += 1;
			seconds = totalTime % 60;
			minutes = (totalTime / 60);
			secondsTmp = seconds.ToString();
			minutesTmp = minutes.ToString ();
			if (seconds < 10) 
			{
				secondsTmp = "0" + seconds;
			}
			if (minutes<10) 
			{
				minutesTmp = "0" + minutes;
			}

			GetComponent<Text>().text = minutesTmp+ " : "+ secondsTmp;
		}
	}
}
