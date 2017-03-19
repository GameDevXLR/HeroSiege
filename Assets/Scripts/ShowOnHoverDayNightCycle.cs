using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ShowOnHoverDayNightCycle :  MonoBehaviour 
{
	public Transform panelToShow;

	// Use this for initialization
	void Start () {
		panelToShow = transform.GetChild(0);	
	}

	public  void ShowHover()
	{
		panelToShow.gameObject.SetActive (true);
		panelToShow.GetComponent<Text> ().text = "Day " + GameManager.instanceGM.Days.ToString ();
	}

	public  void HideOnEndHover()
	{
		panelToShow.gameObject.SetActive (false);

	}
}
