using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameTipsHandler : MonoBehaviour 
{
	public Canvas gameTipsCanvas;
	public Text tipDescription;
	public Toggle showTips;


	public void CloseTheTipWindow()
	{
		gameTipsCanvas.enabled = false;
	}

	public void ChangeTipsOption()
	{
		PlayerPrefsX.SetBool ("BEGINNER_GUIDE", showTips.isOn);
	}
}
