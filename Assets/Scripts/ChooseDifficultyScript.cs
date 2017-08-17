using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.EventSystems;
using UnityEngine.Events;


public class ChooseDifficultyScript : NetworkBehaviour,  IEventSystemHandler
{

	public UnityEvent GameMode;
//	public UnityEvent SelectedHero;
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

	[Header("Difficulty select buttons")]
	public Button normalBtn;
	public Button hardBtn;
	public Button nightmareBtn;
	public Button madnessBtn;

	public Button startGameBtn;

	public enum difficultySettings
	{
		normal,
		hard,
		nightmare,
		madness
	}

	[SyncVar(hook="SyncDifficulty")]
	public int diffLvl;

	public void SyncDifficulty (int diff)
	{
		normalBtn.animator.SetTrigger ("Normal");
		hardBtn.animator.SetTrigger ("Normal");
		nightmareBtn.animator.SetTrigger ("Normal");
		madnessBtn.animator.SetTrigger ("Normal");
		diffLvl = diff;
		switch (diff) 
		{
		case 1:
			normalBtn.animator.SetTrigger ("Highlighted");
			break;
		case 2:
			hardBtn.animator.SetTrigger ("Highlighted");
			break;
		case 3:
			nightmareBtn.animator.SetTrigger ("Highlighted");
			break;
		case 4:
			madnessBtn.animator.SetTrigger ("Highlighted");
			break;
			
		default:
			break;
		}
	}

	public difficultySettings gameMode;

	void Start()
	{
		if (GameMode == null) 
		{
			GameMode = new UnityEvent ();
		}
		GameMode.AddListener (NormalModeExe);
		StartCoroutine (StartProcedure ());
	}
	public void NormalMode()
	{
		diffLvl = 1;
		GameMode.RemoveAllListeners ();
		GameMode.AddListener (NormalModeExe);
//		Invoke ("NormalModeExe", 0.2f);
	}
	public void NormalModeExe()
	{
		GameManager.instanceGM.gameDifficulty = 1;
		gameMode = difficultySettings.normal;
		if (PlayerPrefs.GetString ("LANGAGE") == "Fr") 
		{
			GameManager.instanceGM.messageManager.SendAnAlertMess ("Mode Normal activé.", Color.green);

		} else 
		{
			GameManager.instanceGM.messageManager.SendAnAlertMess ("Normal mode activated.", Color.green);
		}
		inib1.GetComponent<SpawnManager>().enabled = true;
		if (isSolo) 
		{
			return;
		}
		inib1B.GetComponent<SpawnManager>().enabled = true;


	}
	public void HardMode()
	{
		diffLvl = 2;
		GameMode.RemoveAllListeners ();
		GameMode.AddListener (HardModeExe);
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
		diffLvl = 3;
		GameMode.RemoveAllListeners ();
		GameMode.AddListener (NightmareModeExe);
	}
	public void NightmareModeExe()
	{
		GameManager.instanceGM.gameDifficulty = 3;
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
		diffLvl = 4;
		GameMode.RemoveAllListeners ();
		GameMode.AddListener (MadnessModeExe);
	}
	public void MadnessModeExe()
	{
		GameManager.instanceGM.gameDifficulty = 4;
		if (PlayerPrefs.GetString ("LANGAGE") == "Fr") {
			GameManager.instanceGM.messageManager.SendAnAlertMess ("Mode Madness activé. Prenez garde!", Color.green);

		} else {
			GameManager.instanceGM.messageManager.SendAnAlertMess ("Madness?! Run! You fool...", Color.red);
		}
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
		RpcStartTheGame ();
		GameObject.Find ("MainSun").GetComponent<DayNightCycle> ().speed = -1.2f;
//		gameObject.GetComponent<RectTransform>().localScale = Vector3.zero;
		GameMode.Invoke ();
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
			spawManager = GameObject.Find ("SpawnManager");
			difficultyPanel = GameObject.Find ("NewDiffPan");
		GetComponent<RectTransform> ().localPosition = new Vector3 (0f, 416f, 0f);
		if (isServer) {
			normalBtn.interactable = true;
			hardBtn.interactable = true;
			nightmareBtn.interactable = true;
			madnessBtn.interactable = true;
			startGameBtn.interactable = true;
		} else {
			startGameBtn.gameObject.SetActive (false);
		}
	}
	public void StartGameButtonAvailable()
	{
		
	}
	[ClientRpc]
	public void RpcStartTheGame()
	{
		GameObject.Find ("SelectionScreen").SetActive (false);
		GameManager.instanceGM.playerObj.GetComponent<PlayerInitialisationScript> ().selectedHero.Invoke ();
//		SelectedHero.Invoke ();
	}
}
