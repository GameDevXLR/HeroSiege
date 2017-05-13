using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMenuSettings : MonoBehaviour 
{

	// on va stocker ici des infos qu'on veut conserver pour la prochaine scene.
	//c'est sur le netmanager du coup ce sera pas détruit au chargement de scene
	public bool isItOneLane;
	public bool beginnerGuide = true;
	public Toggle isGuideActive;

	// Use this for initialization
	void Start () 
	{
		beginnerGuide = PlayerPrefsX.GetBool ("BEGINNER_GUIDE", true);
		isGuideActive.isOn = beginnerGuide;
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	public void SwitchBegGuide()
	{
		PlayerPrefsX.SetBool ("BEGINNER_GUIDE", isGuideActive.isOn);
	}
}
