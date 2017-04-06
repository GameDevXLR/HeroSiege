using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameClockMinSec : MonoBehaviour 
{
	public int totalTime;
	public int minutes;
	public int seconds;


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
			GetComponent<Text>().text = minutes+ " : "+ seconds;
		}
	}
}
