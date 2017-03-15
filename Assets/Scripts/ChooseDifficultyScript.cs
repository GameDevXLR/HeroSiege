using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseDifficultyScript : MonoBehaviour 
{
	public GameObject difficultyPanel;
	public GameObject inib1;
	public GameObject inib2;
	public GameObject inib3;
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
		gameMode = difficultySettings.hard;

		gameObject.SetActive (false);


	}
	public void NightmareMode()
	{
		GameManager.instanceGM.gameDifficulty = 4;
		inib2.GetComponent<SpawnManager>().enabled = true;
		inib3.GetComponent<SpawnManager>().enabled = true;

		gameMode = difficultySettings.nightmare;

		gameObject.SetActive (false);

	}
	public void MadnessMode()
	{
		GameManager.instanceGM.gameDifficulty = 10;
		inib1.GetComponent<SpawnManager>().enabled = true;
		inib2.GetComponent<SpawnManager>().enabled = true;
		inib3.GetComponent<SpawnManager>().enabled = true;
		gameMode = difficultySettings.madness;

		gameObject.SetActive (false);

	}


}
