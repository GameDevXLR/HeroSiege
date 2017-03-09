using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowOnHover : MonoBehaviour {

	public Transform panelToShow;

	// Use this for initialization
	void Start () {
		panelToShow = transform.GetChild(0);	
	}
	
	public void ShowHover(){
		panelToShow.gameObject.SetActive (true);
		
	}

	public void HideOnEndHover()
	{
		panelToShow.gameObject.SetActive (false);

	}
}
