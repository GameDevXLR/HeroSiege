using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameTipsHandler : MonoBehaviour 
{
	public Canvas gameTipsCanvas;
	public Text tipDescription;
	public Toggle showTips;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void CloseTheTipWindow()
	{
		gameTipsCanvas.enabled = false;
	}

	public void ChangeTipsOption()
	{
		PlayerPrefsX.SetBool ("BEGINNER_GUIDE", showTips.isOn);
	}
}
