using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseDifficultyScript : MonoBehaviour 
{
	public GameObject difficultyPanel;
	public GameObject inib1;
	public GameObject inib2;
	public GameObject inib3;

	public GameObject inib1B;
	public GameObject inib2B;
	public GameObject inib3B;

	public enum difficultySettings
	{
		normal,
		hard,
		nightmare,
		madness
	}

	public difficultySettings gameMode;

	void Start()
	{
		inib1 = GameObject.Find ("Inib");
		inib2 = GameObject.Find ("Inib2");
		inib3 = GameObject.Find ("Inib3");
		inib1B = GameObject.Find ("InibB");
		inib2B = GameObject.Find ("Inib2B");
		inib3B = GameObject.Find ("Inib3B");
		difficultyPanel = GameObject.Find ("DifficultyPanel");

	}

	public void NormalMode()
	{
		GameManager.instanceGM.gameDifficulty = 1;
		GameManager.instanceGM.messageManager.SendAnAlertMess ("Normal mode activated...pussy!", Color.green);
		inib1.GetComponent<SpawnManager>().enabled = true;
		gameMode = difficultySettings.normal;
		gameObject.SetActive (false);
	}
	public void HardMode()
	{
		GameManager.instanceGM.gameDifficulty = 2;
		inib1.GetComponent<SpawnManager>().enabled = true;
		inib1B.GetComponent<SpawnManager>().enabled = true;

		gameMode = difficultySettings.hard;

		gameObject.SetActive (false);


	}
	public void NightmareMode()
	{
		GameManager.instanceGM.gameDifficulty = 4;
		inib2.GetComponent<SpawnManager>().enabled = true;
		inib3.GetComponent<SpawnManager>().enabled = true;
		inib2B.GetComponent<SpawnManager>().enabled = true;
		inib3B.GetComponent<SpawnManager>().enabled = true;
		gameMode = difficultySettings.nightmare;

		gameObject.SetActive (false);

	}
	public void MadnessMode()
	{
		GameManager.instanceGM.gameDifficulty = 10;
		GameManager.instanceGM.messageManager.SendAnAlertMess ("Madness?! Run! You fool...", Color.red);

		inib1.GetComponent<SpawnManager>().enabled = true;
		inib2.GetComponent<SpawnManager>().enabled = true;
		inib3.GetComponent<SpawnManager>().enabled = true;
		inib1B.GetComponent<SpawnManager>().enabled = true;
		inib2B.GetComponent<SpawnManager>().enabled = true;
		inib3B.GetComponent<SpawnManager>().enabled = true;
		gameMode = difficultySettings.madness;

		gameObject.SetActive (false);

	}



}
