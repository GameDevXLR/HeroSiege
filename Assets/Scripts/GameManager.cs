using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
using HyperLuminalGames;


public class GameManager : NetworkBehaviour 
{

	// ci-git le game manager ! Oyez Oyez... Il est unique, un seul par utilisateur. il est sync sur le réseau. Il gere tout ce qui est
	// vies des équipes; membres des equipes; infos globales sur qui est le vrai joueur ici...
	// quoi faire en cas de win / loose.
	// clean la scene une fois la partie finie.
	//il gere également les evenements importants genre : event de nuit / jour.
	//il gere aussi le spawn entre guillemets (active / désactive les spawners de mobs)
	// gere pas grand chose lié au réseau parcontre : voir NetworkManagerObj pour ca (dans la hierarchy)
	public GameObject difficultyPanel;
	public AudioClip LooseLifeSound;
	public AudioClip winSound;
	public AudioClip looseSound;
	public Text generalTxt;
	public Text gameOverTxt;
	public Sprite winSprite;
	public Sprite looseSprite;
	public Text team1LivesDisplay;
	public Text team2LivesDisplay;
	public static GameManager instanceGM = null;
	private GameObject[] ennemies;
	public AlertMessageManager messageManager;
	public GameObject playerObj;
	public bool soloGame;
	public List<NetworkInstanceId> team1ID;
//	public int[] playersScoreT1;
	public List<NetworkInstanceId> team2ID;
//	public int[] playersScoreT2;
	public bool isTeam1;
	public bool isTeam2;
	public int coPlayers; //nombre Total! de joueurs connectés. utile que si t'es le serveur pour le moment...
	[SyncVar]public int teamWhoWon;
	[SyncVar(hook = "GameRestarting")]private bool isRestarting;

	public NetworkInstanceId ID;
	[SyncVar(hook = "T1SyncLooseLife")]public int lifeOfTheTeam1 = 15;
	[SyncVar(hook = "T2SyncLooseLife")]public int lifeOfTheTeam2 = 15;
	[SyncVar(hook = "DayNightEvents")]public bool nightTime;
	public int Days = 1;
	public Image dayNightDisplay;
	public Sprite dayIcon;
	public Sprite nightIcon;
	public LocationManager locManager;

	[SyncVar(hook = "SyncDifficulty")]public int gameDifficulty = 1;

	//on s'assure en Awake que le script est bien unique. sinon on détruit le nouvel arrivant.
	void Awake(){
		if (instanceGM == null) 
		{
			instanceGM = this;
			
		} else if (instanceGM != this) 
		{
//			if(team1ID.Contains(playerObj.GetComponent<NetworkIdentity>().netId)){} //utliser ca pour voir si on est dans la list déja.
			Destroy (gameObject);
		}
		
	}
	void Start()
	{
		team1LivesDisplay = GameObject.Find ("LivesDisplayT1").GetComponent<Text> ();
		team2LivesDisplay = GameObject.Find ("LivesDisplayT2").GetComponent<Text> ();
		generalTxt = GameObject.Find ("GeneralText").GetComponent<Text>();
		difficultyPanel = GameObject.Find ("DifficultyPanel");
		dayNightDisplay = GameObject.Find ("DayNightDisplay").GetComponent<Image> ();
		locManager = GameObject.Find ("LocationManager").GetComponent<LocationManager> ();
	}
	public bool IsItSolo()
	{
		coPlayers = NetworkServer.connections.Count;
		if (coPlayers == 1) 
		{
			soloGame = true;
		}
		return soloGame;
	}
	public void T1SyncLooseLife(int life)
	{
		team1LivesDisplay.text = "Blue Team : " + life.ToString ();
		lifeOfTheTeam1 = life;
		if (isTeam1) {
			messageManager.SendAnAlertMess ("We've lost a life. " + lifeOfTheTeam1.ToString () + " lives remaining.", Color.red);
			return;
		} else 
		{
			messageManager.SendAnAlertMess ("The other team have " + lifeOfTheTeam1.ToString () + " lives remaining.", Color.green);

		}

	}

	public void T2SyncLooseLife(int life)
	{
		team2LivesDisplay.text = "Red Team : " + life.ToString ();
		lifeOfTheTeam2 = life;
		if (isTeam2) {
			messageManager.SendAnAlertMess ("We've lost a life. " + lifeOfTheTeam2.ToString () + "  lives remaining.", Color.red);
			return;
		} else 
		{
			messageManager.SendAnAlertMess ("The other team have " + lifeOfTheTeam2.ToString () + " lives remaining.", Color.green);

		}

	}

	public void Team1LooseALife()
	{
		if (isRestarting) 
		{
			return;
		}
		lifeOfTheTeam1 -= 1 ;
		GetComponent<AudioSource> ().PlayOneShot (LooseLifeSound);
		if (lifeOfTheTeam1 <= 0)
		{
			teamWhoWon = 2;
			isRestarting = true;
			StartCoroutine (RestartTheLevel ());
		}
	}

	public void Team2LooseALife()
	{
		if (isRestarting) 
		{
			return;
		}
		lifeOfTheTeam2 -= 1 ;
		GetComponent<AudioSource> ().PlayOneShot (LooseLifeSound);
		if (lifeOfTheTeam2 <= 0)
		{
			teamWhoWon = 1;
			isRestarting = true;

			StartCoroutine (RestartTheLevel ());
		}
	}

	IEnumerator RestartTheLevel()
	{

		yield return new WaitForSeconds (5f);
		if (isServer) 
		{
			NetworkManager.singleton.ServerChangeScene ("scene2");	//utilise onserverloadscene pour dire aux joueurs quoi faire une fois load.	
		}

	}

	public void GameRestarting(bool restarting)
	{
		isRestarting = restarting;

		if (teamWhoWon == 1 && isTeam1 || teamWhoWon == 2 && isTeam2) 
		{
//			gameOverTxt.text = "Victory!!!";
//			gameOverTxt.color = Color.green;
			gameOverTxt.transform.GetComponentInChildren<Image>().sprite = winSprite;
			gameOverTxt.transform.GetComponentInChildren<Image> ().enabled = true;
			GetComponent<AudioSource> ().PlayOneShot (winSound);
		} else 
		{
//			gameOverTxt.text = "Deafeat...";
			gameOverTxt.transform.GetComponentInChildren<Image>().sprite = looseSprite;
			gameOverTxt.transform.GetComponentInChildren<Image> ().enabled = true;

			GetComponent<AudioSource> ().PlayOneShot (looseSound);

		}
		gameOverTxt.enabled = true;
	}

	public void StopPlayerFromJoining()
	{
			NetworkManager.singleton.matchMaker.SetMatchAttributes (NetworkManager.singleton.matchInfo.networkId, false, NetworkManager.singleton.matchInfo.domain, NetworkLobbyManager.singleton.OnSetMatchAttributes);

	}
		
	public void DayNightEvents(bool night)
	{
		nightTime = night;
		dayNightDisplay.sprite = nightIcon;
		if (night) 
		{
			messageManager.SendAnAlertMess ("It's night time. Better be ready.", Color.red);

			if (isServer) 
			{
				if (Days == 1) 
				{
					coPlayers = NetworkServer.connections.Count;
				}
				NightStartingEvents (coPlayers);
				//NightMobs : spawn them here in the code;
			}
		} else 
		{
			Days++;
			dayNightDisplay.sprite = dayIcon;
			messageManager.SendAnAlertMess ("The sun is shining again...It's day " + Days + ".", Color.green);

			if (isServer) 
			{
				
				coPlayers = NetworkServer.connections.Count; // le nombre de joueurs connectés.
				DayStartingEvents (coPlayers);
				Debug.Log ("spawn them here in the code...There is " + coPlayers.ToString() +" players connected.");
			}
		}
	}

	public void SyncDifficulty(int dif)
	{
		
		gameDifficulty = dif;
		switch (gameDifficulty) 
		{
		case 1:
			
			break;
		case 2:

			break;
		case 4:

			break;
		case 10:

			break;
		default :
			Debug.Log ("Dafuck...contact the programer if you see that.");
			return;
		
		}
		if (!isServer) 
		{
			messageManager.SendAnAlertMess ("The host has chosen a game difficulty of " + gameDifficulty + ". Good Luck !", Color.green);
		} else 
		{
			StartTheGameForAll ();
		}
	}

	//détruit ce qui bloque le joueur pour qu'il puisse commencer a avancer.
	public void StartTheGameForAll()
	{
		GameObject.Find ("CastleToCampPortal").GetComponent<OneWayPortalScript> ().isBeingUsed = false;
		GameObject.Find ("CastleToCampPortalT2").GetComponent<OneWayPortalScript> ().isBeingUsed = false;
		NetworkServer.Destroy (GameObject.Find ("StartingBarricade1"));
		NetworkServer.Destroy (GameObject.Find ("StartingBarricade2"));
		NetworkServer.Destroy (GameObject.Find ("PlayerTeamDetector"));
		NetworkServer.Destroy (GameObject.Find ("PlayerTeamDetector2"));
		StopPlayerFromJoining ();
		RpcMessageToAll ();
		ActivateGoldPerSec ();
	}

	[Server]
	public void ActivateGoldPerSec ()
	{
		StartCoroutine (ActivateGoldPerSecExe ());
	}

	IEnumerator ActivateGoldPerSecExe ()
	{
		GameObject[] allPlayers;
		allPlayers = GameObject.FindGameObjectsWithTag ("Player");
		yield return new WaitForSeconds (0.1f);
		for (int i = 0; i < allPlayers.Length; i++) 
		{
			allPlayers [i].GetComponent<PlayerGoldScript> ().isDropping = true;

		}
	}
		
	public void NightStartingEvents(int nbrOfPlayers)
	{

		if (gameDifficulty == 1 || gameDifficulty == 2) 
		{
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib1.GetComponent<SpawnManager> ().enabled = true;
			if (soloGame) 
			{
				return;
			}
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib1B.GetComponent<SpawnManager> ().enabled = true;

		}
		if (gameDifficulty == 4) 
		{
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib2.GetComponent<SpawnManager> ().enabled = true;
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib3.GetComponent<SpawnManager> ().enabled = true;
			if (soloGame) 
			{
				return;
			}
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib2B.GetComponent<SpawnManager> ().enabled = true;
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib3B.GetComponent<SpawnManager> ().enabled = true;

		}
		if (gameDifficulty == 10) 
		{
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib1.GetComponent<SpawnManager> ().enabled = true;
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib2.GetComponent<SpawnManager> ().enabled = true;
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib3.GetComponent<SpawnManager> ().enabled = true;
			if (soloGame) 
			{
				return;
			}
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib1B.GetComponent<SpawnManager> ().enabled = true;
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib2B.GetComponent<SpawnManager> ().enabled = true;
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib3B.GetComponent<SpawnManager> ().enabled = true;
		}
	}

	public void DayStartingEvents(int nbrOfPlayers)
	{
		if (Days == 2) 
		{
			//si c'est le "premier vrai jour" : on multiplie le nombre de mobs par vague par le nombre de joueurs. ca fou direct le bordel. oh yeah.
//			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib1.GetComponent<SpawnManager> ().numberOfMobs *= coPlayers+1;
		}
		if (gameDifficulty == 1 || gameDifficulty == 2) 
		{
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib1.GetComponent<SpawnManager> ().enabled = false;
			if (soloGame) 
			{
				return;
			}
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib1B.GetComponent<SpawnManager> ().enabled = false;

		}
		if (gameDifficulty == 3) 
		{
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib2.GetComponent<SpawnManager> ().enabled = false;
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib3.GetComponent<SpawnManager> ().enabled = false;
			if (soloGame) 
			{
				return;
			}
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib2B.GetComponent<SpawnManager> ().enabled = false;
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib3B.GetComponent<SpawnManager> ().enabled = false;

		}
		if (gameDifficulty == 4) 
		{
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib1.GetComponent<SpawnManager> ().enabled = false;
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib2.GetComponent<SpawnManager> ().enabled = false;
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib3.GetComponent<SpawnManager> ().enabled = false;
			if (soloGame) 
			{
				return;
			}
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib1B.GetComponent<SpawnManager> ().enabled = false;
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib2B.GetComponent<SpawnManager> ().enabled = false;
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib3B.GetComponent<SpawnManager> ().enabled = false;

		}
	}

	public void AddPlayerToTeam(int team, NetworkInstanceId id)
	{
		if (team == 1) 
		{
			if(team1ID.Contains(id))
			{
				return;
			}
			team1ID.Add (id);
		}
		if (team == 2) 
		{
			if(team2ID.Contains(id))
			{
				return;
			}
			team2ID.Add (id);
		}
	}

	[ClientRpc]
	public void RpcMessageToAll()
	{
		GameObject.Find ("GameClock").GetComponent<GameClockMinSec> ().enabled = true;
		messageManager.SendAnAlertMess ("The game is starting!", Color.green);
		ActualizeLocSystem ();
	}

	public void ActualizeLocSystem()
	{
		StartCoroutine (ActualizeTheLocator ());
	}

	IEnumerator ActualizeTheLocator()
	{
		locManager.Realtime_Updates = true;
		yield return new WaitForSeconds (0.1f);
		locManager.Realtime_Updates = false;
	}
}