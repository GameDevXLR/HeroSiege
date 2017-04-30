using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseDifficultyScript : MonoBehaviour 
{

	//permet de choisir la difficulté de la partie.
	//seul le joueur Host pourra avoir acces a ce menu / il est désactiver au démarrage sur les autres joueurs.
	// en fonction de la difficultée choisi : les inibs sont activer.
	//une fois la difficulté choisi; le gamemanger lancera la partie.
	//le gamemanager garde trace de la difficulté choisie.
	public bool isSolo;
	public GameObject difficultyPanel;
	public GameObject inib1;
	public GameObject inib2;
	public GameObject inib3;

	public GameObject inib1B;
	public GameObject inib2B;
	public GameObject inib3B;
    public GameObject spawManager; // pour le pool 

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
		StartCoroutine (StartProcedure ());
	}
	public void NormalMode()
	{
		Invoke ("NormalModeExe", 0.1f);
	}
	public void NormalModeExe()
	{
		GameManager.instanceGM.gameDifficulty = 1;
		gameMode = difficultySettings.normal;
		GameManager.instanceGM.messageManager.SendAnAlertMess ("Normal mode activated...pussy!", Color.green);
		inib1.GetComponent<SpawnManager>().enabled = true;
		if (isSolo) 
		{
			return;
		}
		inib1B.GetComponent<SpawnManager>().enabled = true;


	}
	public void HardMode()
	{
		Invoke ("HardModeExe", 0.1f);

	}
	public void HardModeExe()
	{
		GameManager.instanceGM.gameDifficulty = 2;
		gameMode = difficultySettings.hard;
		inib1.GetComponent<SpawnManager>().enabled = true;
		if (isSolo) 
		{
			return;
		}
		inib1B.GetComponent<SpawnManager>().enabled = true;



	}
	public void NightmareMode()
	{
		Invoke ("NightmareModeExe", 0.1f);

	}
	public void NightmareModeExe()
	{
		GameManager.instanceGM.gameDifficulty = 4;
		gameMode = difficultySettings.nightmare;
		inib2.GetComponent<SpawnManager>().enabled = true;
		inib3.GetComponent<SpawnManager>().enabled = true;
		if (isSolo) 
		{
			return;
		}
		inib2B.GetComponent<SpawnManager>().enabled = true;
		inib3B.GetComponent<SpawnManager>().enabled = true;


	}
	public void MadnessMode()
	{
		Invoke ("MadnessModeExe", 0.1f);

	}
	public void MadnessModeExe()
	{
		GameManager.instanceGM.gameDifficulty = 10;
		GameManager.instanceGM.messageManager.SendAnAlertMess ("Madness?! Run! You fool...", Color.red);
		gameMode = difficultySettings.madness;

		inib1.GetComponent<SpawnManager>().enabled = true;
		inib2.GetComponent<SpawnManager>().enabled = true;
		inib3.GetComponent<SpawnManager>().enabled = true;
		if (isSolo) 
		{
			return;
		}
		inib1B.GetComponent<SpawnManager>().enabled = true;
		inib2B.GetComponent<SpawnManager>().enabled = true;
		inib3B.GetComponent<SpawnManager>().enabled = true;


	}

	public void GenericStart()
	{
		if (GameManager.instanceGM.IsItSolo ()) 
		{
			isSolo = true;
		}
		GameObject.Find ("MainSun").GetComponent<DayNightCycle> ().speed = 1.5f;
		gameObject.GetComponent<RectTransform>().localScale = Vector3.zero;
	}

	IEnumerator StartProcedure()
	{
		yield return new WaitForSeconds (0.05f);
		inib1 = GameObject.Find ("Inib");
		inib2 = GameObject.Find ("Inib2");
		inib3 = GameObject.Find ("Inib3");
		inib1B = GameObject.Find ("InibB");
		inib2B = GameObject.Find ("Inib2B");
		inib3B = GameObject.Find ("Inib3B");
		spawManager = GameObject.Find("SpawnManager");
		difficultyPanel = GameObject.Find ("DifficultyPanel");
	}
}
