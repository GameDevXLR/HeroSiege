﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseDifficultyScript : MonoBehaviour 
{
	public GameObject inib1;
	public GameObject inib2;
	public GameObject inib3;
	
	void Start()
	{
		inib1 = GameObject.Find ("Inib");
		inib2 = GameObject.Find ("Inib2");
		inib3 = GameObject.Find ("Inib3");

	}

	public void NormalMode()
	{
		GameManager.instanceGM.gameDifficulty = 1;
		inib1.GetComponent<SpawnManager>().enabled = true;

		gameObject.SetActive (false);
	}
	public void HardMode()
	{
		GameManager.instanceGM.gameDifficulty = 2;
		inib1.GetComponent<SpawnManager>().enabled = true;

		gameObject.SetActive (false);


	}
	public void NightmareMode()
	{
		GameManager.instanceGM.gameDifficulty = 4;
		inib2.GetComponent<SpawnManager>().enabled = true;
		inib3.GetComponent<SpawnManager>().enabled = true;


		gameObject.SetActive (false);

	}
	public void MadnessMode()
	{
		GameManager.instanceGM.gameDifficulty = 10;
		inib1.GetComponent<SpawnManager>().enabled = true;
		inib2.GetComponent<SpawnManager>().enabled = true;
		inib3.GetComponent<SpawnManager>().enabled = true;

		gameObject.SetActive (false);

	}


}
