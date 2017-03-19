using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;


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
	public Text generalTxt;
	public Text gameOverTxt;
	public static GameManager instanceGM = null;
	private GameObject[] ennemies;
	public AlertMessageManager messageManager;
	public GameObject playerObj;
	public List<NetworkInstanceId> team1ID;
	public List<NetworkInstanceId> team2ID;
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

	[SyncVar(hook = "SyncDifficulty")]public int gameDifficulty = 1;

	//on s'assure en Awake que le script est bien unique. sinon on détruit le nouvel arrivant.
	void Awake(){
		if (instanceGM == null) {
			instanceGM = this;
			
		} else if (instanceGM != this) 
		{
//			if(team1ID.Contains(playerObj.GetComponent<NetworkIdentity>().netId)){} //utliser ca pour voir si on est dans la list déja.
			Destroy (gameObject);
		}
		
	}
	void Start()
	{
		generalTxt = GameObject.Find ("GeneralText").GetComponent<Text>();
		difficultyPanel = GameObject.Find ("DifficultyPanel");
		dayNightDisplay = GameObject.Find ("DayNightDisplay").GetComponent<Image> ();
	}
	public void T1SyncLooseLife(int life)
	{
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

//			ennemies = GameObject.FindObjectsOfType<GameObject> ();
//			foreach (GameObject g in ennemies) 
//			{
//				if (g.layer == 9) 
//				{
//					Destroy (g);
//				}
//				if (g.layer == 8) 
//				{
//					g.SetActive (false);
//				}
//			}

			//faire ici ce qui doit se passer si on a pu de vie et que la partie est donc finie.
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

//			ennemies = GameObject.FindObjectsOfType<GameObject> ();
//			foreach (GameObject g in ennemies) 
//			{
//				if (g.layer == 9) 
//				{
//					Destroy (g);
//				}
//				if (g.layer == 8) 
//				{
//					g.SetActive (false);
//				}
//			}

			//faire ici ce qui doit se passer si on a pu de vie et que la partie est donc finie.
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
			gameOverTxt.text = "Victory!!!";
			gameOverTxt.color = Color.green;
		} else 
		{
			gameOverTxt.text = "Deafeat...";

		}
		gameOverTxt.enabled = true;
	}

	public void StopPlayerFromJoining()
	{
		NetworkManager.singleton.matchMaker.SetMatchAttributes(NetworkManager.singleton.matchInfo.networkId, false, NetworkManager.singleton.matchInfo.domain, NetworkLobbyManager.singleton.OnSetMatchAttributes);
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
		NetworkServer.Destroy (GameObject.Find ("StartingBarricade1"));
		NetworkServer.Destroy (GameObject.Find ("StartingBarricade2"));
		NetworkServer.Destroy (GameObject.Find ("PlayerTeamDetector"));
		NetworkServer.Destroy (GameObject.Find ("PlayerTeamDetector2"));
		StopPlayerFromJoining ();
		RpcMessageToAll ();
	}


	public void NightStartingEvents(int nbrOfPlayers)
	{

		if (gameDifficulty == 1 || gameDifficulty == 2) 
		{
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib1.GetComponent<SpawnManager> ().enabled = true;
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib1B.GetComponent<SpawnManager> ().enabled = true;

		}
		if (gameDifficulty == 4) 
		{
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib2.GetComponent<SpawnManager> ().enabled = true;
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib3.GetComponent<SpawnManager> ().enabled = true;
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib2B.GetComponent<SpawnManager> ().enabled = true;
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib3B.GetComponent<SpawnManager> ().enabled = true;

		}
		if (gameDifficulty == 10) 
		{
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib1.GetComponent<SpawnManager> ().enabled = true;
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib2.GetComponent<SpawnManager> ().enabled = true;
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib3.GetComponent<SpawnManager> ().enabled = true;
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
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib1.GetComponent<SpawnManager> ().numberOfMobs *= coPlayers+1;
		}
		if (gameDifficulty == 1 || gameDifficulty == 2) 
		{
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib1.GetComponent<SpawnManager> ().enabled = false;
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib1B.GetComponent<SpawnManager> ().enabled = false;

		}
		if (gameDifficulty == 3) 
		{
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib2.GetComponent<SpawnManager> ().enabled = false;
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib3.GetComponent<SpawnManager> ().enabled = false;
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib2B.GetComponent<SpawnManager> ().enabled = false;
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib3B.GetComponent<SpawnManager> ().enabled = false;

		}
		if (gameDifficulty == 4) 
		{
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib1.GetComponent<SpawnManager> ().enabled = false;
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib2.GetComponent<SpawnManager> ().enabled = false;
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib3.GetComponent<SpawnManager> ().enabled = false;
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
		messageManager.SendAnAlertMess ("The game is starting!", Color.green);
	}
}