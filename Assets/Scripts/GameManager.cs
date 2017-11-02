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
	public AudioClip[] MusicBackground;
	public float delayTime;
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
	public Text nbrWavesText;
	public static GameManager instanceGM = null;
	private GameObject[] ennemies;
	public MessagesManager messageManager;
	public GameObject playerObj;
	public bool soloGame;
    public List<NetworkInstanceId> team1ID;
    public List<GameObject> team1Players;
    //	public int[] playersScoreT1;
    public List<NetworkInstanceId> team2ID;
    public List<GameObject> team2Players;
    //	public int[] playersScoreT2;
    public bool isTeam1;
	public bool isTeam2;
	public int coPlayers; //nombre Total! de joueurs connectés. utile que si t'es le serveur pour le moment...
    public bool isInTchat = false;
	[SyncVar]public int teamWhoWon;
	[SyncVar(hook = "GameRestarting")]private bool isRestarting;

	public NetworkInstanceId ID;
	[SyncVar(hook = "T1SyncLooseLife")]public int lifeOfTheTeam1 = 50;
	[SyncVar(hook = "T2SyncLooseLife")]public int lifeOfTheTeam2 = 50;
	[SyncVar(hook = "DayNightEvents")]public bool nightTime;
	public int Days = 1;
	public Image dayNightDisplay;
	public Sprite dayIcon;
	public Sprite nightIcon;
	public LocationManager locManager;
	public LightManagerScript lightM;
    public GameObject[] jungCamps;
    public CrystalManager[] crystaux;
    public Canvas tutorialPanel;
	public Text tutoTip;

	[SyncVar(hook = "SyncDifficulty")]public int gameDifficulty = 1;



	[Header("Reputation system.")]

	[SyncVar (hook = "SyncReputation")]public float  actualReputation;
	private float oldReputation;
	public Slider reputSlider;
	public AudioClip looseRep1;
	public AudioClip looseRep2;
	public AudioClip gainRep1;
	public AudioClip gainRep2;

	//référencement des inibiteurs pour que les mobs puissent venir chercher l'info ici (client ou serveur) et ainsi déduire le path.

	[Header("Référencement des inibiteurs")]

	public SpawnManager camp1,camp2,camp3;
	public SpawnManager camp1B, camp2B, camp3B;

	[Header("Systeme de quetes.")]

	//Premiere tentative de création de quete.

	[SyncVar]public bool isQuest1Active;
	public GameObject quest1ID;
	public int quest1Rep = -10;
	public string quest1Desc = "Don't be the first to loose a life!";


	#region ReputationSystem

	public void TurnOnTheRepSystem()
	{
		reputSlider = GameObject.Find ("ReputationSlider").GetComponent<Slider> ();
	}



	public void SyncReputation(float x)
	{
		
		oldReputation = actualReputation;
		actualReputation = x;
		reputSlider.value = actualReputation;
		float y = oldReputation - actualReputation;
		if (isTeam1) 
		{
			if (y == 0) 
			{
				Debug.Log ("this should not happen!");
			}
			if (y > 0) 
			{
				if (y > .2f) 
				{
					GetComponent<AudioSource> ().PlayOneShot (looseRep2);
					return;
				}
				GetComponent<AudioSource> ().PlayOneShot (looseRep1);
			} else 
			{
				if (y < -.2f) 
				{
					GetComponent<AudioSource> ().PlayOneShot (gainRep2);
					return;
				}
				GetComponent<AudioSource> ().PlayOneShot (gainRep1);

			}
			
		} else 
		{
			if (y < 0) 
			{
				GetComponent<AudioSource> ().PlayOneShot (looseRep1);
			} else 
			{
				GetComponent<AudioSource> ().PlayOneShot (gainRep1);

			}
		}
	}

	#endregion
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
		difficultyPanel = GameObject.Find ("NewDiffPan");
		camp1 = difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib1.GetComponent<SpawnManager> ();
		camp2 = difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib2.GetComponent<SpawnManager> ();
		camp3 = difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib3.GetComponent<SpawnManager> ();
		camp1B = difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib1B.GetComponent<SpawnManager> ();
		camp2B = difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib2B.GetComponent<SpawnManager> ();
		camp3B = difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib3B.GetComponent<SpawnManager> ();
	}
	void Start()
	{
		team1LivesDisplay = GameObject.Find ("LivesDisplayT1").GetComponent<Text> ();
		team2LivesDisplay = GameObject.Find ("LivesDisplayT2").GetComponent<Text> ();

		if (isServer) {
			ShowAGameTip ("The number of waves spawn points and the strenght of the enemies depends on the number of players by team and the game difficulty.On Nightmare and above, the number of enemy spawn points will increase.");
			if (PlayerPrefs.GetString ("LANGAGE") == "Fr") 
			{
				ShowAGameTip ("Le nombre de points de départ des vagues et la force des ennemis dépend du nombre de joueur de l'équipe et de la difficulté de la partie. En 'Cauchemar' et plus, le nombre de points de départ est augmenté.");

			}
		}
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
		if (isServer && isQuest1Active) 
		{
			EndQuestOne (true);
		}
		team1LivesDisplay.text = "Blue Team : " + life.ToString ();
		if (PlayerPrefs.GetString ("LANGAGE") == "Fr") 
		{
			team1LivesDisplay.text = "Equipe bleu: " + life.ToString ();
		}
		lifeOfTheTeam1 = life;
		if (life > 10) 
		{
			return;
		}
		if (isTeam1) 
		{
			if (PlayerPrefs.GetString ("LANGAGE") == "Fr") 
			{
				messageManager.SendAnAlertMess ("Nous avons perdu une vie. " + lifeOfTheTeam1.ToString () + " vie(s) restante..", Color.red);

			} else 
			{
				messageManager.SendAnAlertMess ("We've lost a life. " + lifeOfTheTeam1.ToString () + " lives remaining.", Color.red);
			}
			return;
		} else 
		{
			if (PlayerPrefs.GetString ("LANGAGE") == "Fr") 
			{
				messageManager.SendAnAlertMess ("L'équipe adverse a perdu une vie. " + lifeOfTheTeam1.ToString () + " vie(s) restante.", Color.green);

			} else 
			{
				messageManager.SendAnAlertMess ("The other team have " + lifeOfTheTeam1.ToString () + " lives remaining.", Color.green);
			}

		}

	}

	public void T2SyncLooseLife(int life)
	{
		if (isServer && isQuest1Active) 
		{
			EndQuestOne (false);
		}
		team2LivesDisplay.text = "Red Team: " + life.ToString ();
		if (PlayerPrefs.GetString ("LANGAGE") == "Fr") 
		{
			team2LivesDisplay.text = "Equipe rouge: " + life.ToString ();

		}
		lifeOfTheTeam2 = life;
		if (life > 10) 
		{
			return;
		}
		if (isTeam2) 
		{
			if (PlayerPrefs.GetString ("LANGAGE") == "Fr") 
			{
				messageManager.SendAnAlertMess ("Nous avons perdu une vie. " + lifeOfTheTeam2.ToString () + " vie(s) restante..", Color.red);

			} else 
			{
				messageManager.SendAnAlertMess ("We've lost a life. " + lifeOfTheTeam2.ToString () + "  lives remaining.", Color.red);
			}
			return;
		} else 
		{
			if (PlayerPrefs.GetString ("LANGAGE") == "Fr") 
			{
				messageManager.SendAnAlertMess ("L'équipe adverse a perdu une vie. " + lifeOfTheTeam2.ToString () + " vie(s) restante.", Color.green);

			} else 
			{
				messageManager.SendAnAlertMess ("The other team have " + lifeOfTheTeam2.ToString () + " lives remaining.", Color.green);
			}

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
			//penser a corriger ici le bug hyperluminal qui redémarre pas.
			//ptete voir pour détruire tous les objets associés a hyperL avant de restart (tout sur layer player3)
			NetworkManager.singleton.ServerChangeScene ("scene2");	//utilise onserverloadscene pour dire aux joueurs quoi faire une fois load.	
		}

	}

	public void GameRestarting(bool restarting)
	{
		isRestarting = restarting;

		if ( soloGame ||teamWhoWon == 1 && isTeam1 || teamWhoWon == 2 && isTeam2) 
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
//			StartCoroutine (ChangeVolumeCurve ());
			Camera.main.transform.GetComponent<AudioSource> ().clip = MusicBackground[0];
			Camera.main.transform.GetComponent<AudioSource> ().Play();



			if (PlayerPrefs.GetString ("LANGAGE") == "Fr") 
			{
				messageManager.SendAnAlertMess ("La nuit tombe. Préparez-vous!", Color.red);

			} else 
			{
				messageManager.SendAnAlertMess ("It's night time. Better be ready.", Color.red);
			}
			lightM.isSwitchingON = true;

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
//			StartCoroutine (ChangeVolumeCurve ());

			Camera.main.transform.GetComponent<AudioSource> ().clip = MusicBackground[1];
			Camera.main.transform.GetComponent<AudioSource> ().Play();
			Days++;
			if (Days == 2) 
			{
				ShowAGameTip ("During the day, the jungle camps will respawn and the daily bosses will show up. The waves won't spawn before night time.");
				if (PlayerPrefs.GetString ("LANGAGE") == "Fr") 
				{
					ShowAGameTip ("Durant la journée, les camps de la jungle réapparraissent tous les jours, en même temps que les boss. Les vagues redémarrent à la nuit tombée.");

				}
			}
			if (Days == 3) 
			{
				ShowAGameTip ("If you feel strong enough, you could try to capture the outpost in the middle of the map. This will give you access to a shop and will give you more time to come back to the fight if you die.");
				if (PlayerPrefs.GetString ("LANGAGE") == "Fr") 
				{
					ShowAGameTip ("Si vous pensez être assez fort, essayez le capturer l'avant-poste au milieu de la carte. Vous aurez ainsi accés a un magasin et aurez plus de temps pour revenir combattre en cas de mort.");

				}
			}
			playerObj.GetComponent<PlayerCastCatapulte> ().spellDmg = playerObj.GetComponent<PlayerCastCatapulte> ().startDmg* Days;
			playerObj.GetComponent<PlayerCastCatapulte> ().ActualizeCataDmg ();
			GetComponent<BossSpawnManager> ().bossLvlT1 +=2;
			GetComponent<BossSpawnManager> ().bossLvlT2 +=2;
			dayNightDisplay.sprite = dayIcon;
			if (PlayerPrefs.GetString ("LANGAGE") == "Fr") 
			{
				messageManager.SendAnAlertMess ("Le jour se lève...C'est le jour " + Days + ".", Color.green);

			} else 
			{
				messageManager.SendAnAlertMess ("The sun is shining again...It's day " + Days + ".", Color.green);
			}
			lightM.isSwitchingOFF = true;

			if (isServer) 
			{
				coPlayers = NetworkServer.connections.Count; // le nombre de joueurs connectés.
				DayStartingEvents (coPlayers);
				Debug.Log ("spawn them here in the code...There is " + coPlayers.ToString() +" players connected.");
				foreach (GameObject go in jungCamps) 
				{
					go.GetComponent<JungleCampSpawnManager> ().ResetThisJungCamp ();
				}
                foreach (CrystalManager crystal in crystaux)
                {
                    crystal.replenish();
                    crystal.decrease = true;
                }
            }
		}
	}


	public void SyncDifficulty(int dif)
	{
		
		gameDifficulty = dif;
		switch (gameDifficulty) 
		{
		case 1:
			lifeOfTheTeam1 = 99;
			lifeOfTheTeam2 = 99;

			team1LivesDisplay.text = "Blue Team: " + lifeOfTheTeam1.ToString ();
			team2LivesDisplay.text = "Red Team: " + lifeOfTheTeam2.ToString ();
			
			break;
		case 2:
			lifeOfTheTeam1 = 50;
			lifeOfTheTeam2 = 50;

			team1LivesDisplay.text = "Blue Team: " + lifeOfTheTeam1.ToString ();
			team2LivesDisplay.text = "Red Team: " + lifeOfTheTeam2.ToString ();
			break;
		case 3:
			lifeOfTheTeam1 = 25;
			lifeOfTheTeam2 = 25;

			team1LivesDisplay.text = "Blue Team: " + lifeOfTheTeam1.ToString ();
			team2LivesDisplay.text = "Red Team: " + lifeOfTheTeam2.ToString ();
			break;
		case 4:
			lifeOfTheTeam1 = 10;
			lifeOfTheTeam2 = 10;

			team1LivesDisplay.text = "Blue Team: " + lifeOfTheTeam1.ToString ();
			team2LivesDisplay.text = "Red Team: " + lifeOfTheTeam2.ToString ();
			break;
		default :
			Debug.Log ("Dafuck...contact the programer if you see that.");
			return;
		
		}
		if (!isServer) 
		{
			if (PlayerPrefs.GetString ("LANGAGE") == "Fr") 
			{
				messageManager.SendAnAlertMess ("Le serveur a choisi une difficulté de " + gameDifficulty + ". Bonne chance!", Color.green);

			} else 
			{
				messageManager.SendAnAlertMess ("The host has chosen a game difficulty of " + gameDifficulty + ". Good Luck!", Color.green);
			}
		} else 
		{
			StartTheGameForAll ();
			//on garde l'ID de la quete pour pouvoir la finir.
			StartQuestOne();
		}
	}

	public void StartQuestOne()
	{
		if (isQuest1Active) 
		{
			return;
		}
//		quest1ID =  GetComponent<QuestManager> ().CreateQuestPanelItem ("Be the first team to kill 20 minions.", 200);
		quest1ID =  GetComponent<QuestManager> ().CreateQuestPanelItem (quest1Desc, quest1Rep);
		isQuest1Active = true;


	}
	public void EndQuestOne(bool isFirstTeam)
	{
		if (!isQuest1Active) 
		{
			return;
		}
		NetworkServer.Destroy(quest1ID);
		//gestion de la récompense de quete:
		if (isFirstTeam) 
		{
			SyncReputation( actualReputation - (float)((float)quest1Rep / 100f));
		}
		else 
		{

			SyncReputation( actualReputation + (float)((float)quest1Rep / 100f));
		}
		isQuest1Active = false;

	}

	//détruit ce qui bloque le joueur pour qu'il puisse commencer a avancer.
	public void StartTheGameForAll()
	{
//		GameObject.Find ("CastleToCampPortal").GetComponent<OneWayPortalScript> ().isBeingUsed = false;
//		GameObject.Find ("CastleToCampPortalT2").GetComponent<OneWayPortalScript> ().isBeingUsed = false;
//		NetworkServer.Destroy (GameObject.Find ("StartingBarricade1"));
//		NetworkServer.Destroy (GameObject.Find ("StartingBarricade2"));
		RpcInitializeTheGame();
		NetworkServer.Destroy (GameObject.Find ("PlayerTeamDetector"));
		NetworkServer.Destroy (GameObject.Find ("PlayerTeamDetector2"));
		StopPlayerFromJoining ();
		RpcMessageToAll ();
		ActivateGoldPerSec ();
		foreach (GameObject go in jungCamps) 
		{
			go.GetComponent<JungleCampSpawnManager> ().ResetThisJungCamp ();
		}
        foreach(CrystalManager crystal in crystaux)
        {
            crystal.initialize();
        }
	}

	[ClientRpc]
	public void RpcInitializeTheGame()
	{
		generalTxt = GameObject.Find ("GeneralText").GetComponent<Text>();
		dayNightDisplay = GameObject.Find ("DayNightDisplay").GetComponent<Image> ();
		locManager = GameObject.Find ("LocationManager").GetComponent<LocationManager> ();
		lightM = GameObject.Find ("[Lights]").GetComponent<LightManagerScript> ();
		nbrWavesText = GameObject.Find ("WavesCounter").GetComponent<Text> ();
		nbrWavesText.text = "0";
		TurnOnTheRepSystem ();

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
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib1.GetComponent<SpawnManager> ().StartSpawning (Days);
			if (soloGame) 
			{
				return;
			}
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib1B.GetComponent<SpawnManager> ().enabled = true;
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib1B.GetComponent<SpawnManager> ().StartSpawning (Days);
		}
		if (gameDifficulty == 3) 
		{
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib2.GetComponent<SpawnManager> ().enabled = true;
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib3.GetComponent<SpawnManager> ().enabled = true;
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib2.GetComponent<SpawnManager> ().StartSpawning (Days);
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib3.GetComponent<SpawnManager> ().StartSpawning (Days);

			if (soloGame) 
			{
				return;
			}
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib2B.GetComponent<SpawnManager> ().enabled = true;
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib3B.GetComponent<SpawnManager> ().enabled = true;
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib2B.GetComponent<SpawnManager> ().StartSpawning (Days);
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib3B.GetComponent<SpawnManager> ().StartSpawning (Days);

		}
		if (gameDifficulty == 4) 
		{
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib1.GetComponent<SpawnManager> ().enabled = true;
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib2.GetComponent<SpawnManager> ().enabled = true;
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib3.GetComponent<SpawnManager> ().enabled = true;
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib1.GetComponent<SpawnManager> ().StartSpawning (Days);
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib2.GetComponent<SpawnManager> ().StartSpawning (Days);
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib3.GetComponent<SpawnManager> ().StartSpawning (Days);

			if (soloGame) 
			{
				return;
			}
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib1B.GetComponent<SpawnManager> ().enabled = true;
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib2B.GetComponent<SpawnManager> ().enabled = true;
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib3B.GetComponent<SpawnManager> ().enabled = true;
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib1B.GetComponent<SpawnManager> ().StartSpawning (Days);
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib2B.GetComponent<SpawnManager> ().StartSpawning (Days);
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib3B.GetComponent<SpawnManager> ().StartSpawning (Days);

		}
	}

	public void DayStartingEvents(int nbrOfPlayers)
	{
		if (Days == 2) 
		{
			//si c'est le "premier vrai jour" : on multiplie le nombre de mobs par vague par le nombre de joueurs. ca fou direct le bordel. oh yeah.
//			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib1.GetComponent<SpawnManager> ().numberOfMobs *= coPlayers+1;
		}

		GetComponent<BossSpawnManager> ().SpawnBosses ();

		if (gameDifficulty == 1 || gameDifficulty == 2) 
		{
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib1.GetComponent<SpawnManager> ().StopAllCoroutines ();
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib1.GetComponent<SpawnManager> ().enabled = false;
			if (soloGame) 
			{
				return;
			}
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib1B.GetComponent<SpawnManager> ().StopAllCoroutines ();
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib1B.GetComponent<SpawnManager> ().enabled = false;

		}
		if (gameDifficulty == 3) 
		{
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib2.GetComponent<SpawnManager> ().StopAllCoroutines ();
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib3.GetComponent<SpawnManager> ().StopAllCoroutines ();
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib2.GetComponent<SpawnManager> ().enabled = false;
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib3.GetComponent<SpawnManager> ().enabled = false;

			if (soloGame) 
			{
				return;
			}
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib2B.GetComponent<SpawnManager> ().StopAllCoroutines ();
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib3B.GetComponent<SpawnManager> ().StopAllCoroutines ();
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib2B.GetComponent<SpawnManager> ().enabled = false;
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib3B.GetComponent<SpawnManager> ().enabled = false;

		}
		if (gameDifficulty == 4) 
		{
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib1.GetComponent<SpawnManager> ().StopAllCoroutines ();
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib2.GetComponent<SpawnManager> ().StopAllCoroutines ();
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib3.GetComponent<SpawnManager> ().StopAllCoroutines ();
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib1.GetComponent<SpawnManager> ().enabled = false;
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib2.GetComponent<SpawnManager> ().enabled = false;
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib3.GetComponent<SpawnManager> ().enabled = false;

			if (soloGame) 
			{
				return;
			}
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib1B.GetComponent<SpawnManager> ().StopAllCoroutines ();
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib2B.GetComponent<SpawnManager> ().StopAllCoroutines ();
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib3B.GetComponent<SpawnManager> ().StopAllCoroutines ();
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib1B.GetComponent<SpawnManager> ().enabled = false;
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib2B.GetComponent<SpawnManager> ().enabled = false;
			difficultyPanel.GetComponent<ChooseDifficultyScript> ().inib3B.GetComponent<SpawnManager> ().enabled = false;

		}
	}

    public void AddPlayerToTeam(int team, GameObject player)
    {
        if (team == 1)
        {
            if (team1ID.Contains(player.GetComponent<NetworkIdentity>().netId))
            {
                return;
            }
            team1ID.Add(player.GetComponent<NetworkIdentity>().netId);
            team1Players.Add(player);
        }
        if (team == 2)
        {
            if (team2ID.Contains(player.GetComponent<NetworkIdentity>().netId))
            {
                return;
            }
            team2ID.Add(player.GetComponent<NetworkIdentity>().netId);
            team2Players.Add(player);
        }
    }
    

    [ClientRpc]
	public void RpcMessageToAll()
	{
		GameObject.Find ("GameClock").GetComponent<GameClockMinSec> ().enabled = true;
		if (isTeam1) 
		{
			if (PlayerPrefs.GetString ("LANGAGE") == "Fr") 
			{
				messageManager.SendAnAlertMess ("la partie commence, vous avez rejoind l'équipe bleue.", Color.green);			

			} else 
			{
				messageManager.SendAnAlertMess ("The game is starting!You have join the blue team.", Color.green);			
			}
		}
		if (isTeam2) 
		{
			if (PlayerPrefs.GetString ("LANGAGE") == "Fr") 
			{
				messageManager.SendAnAlertMess ("La partie commence, vous avez rejoind l'équipe rouge.", Color.green);			

			} else 
			{
				messageManager.SendAnAlertMess ("The game is starting!You have join the red team.", Color.green);			
			}
		}
//		GameObject.Find ("PlayerSelectionScreen").GetComponent<Canvas> ().enabled = true;
//		GameObject.Find ("DifficultySelectCanvas").GetComponent<Canvas> ().enabled = false;
		ActualizeLocSystem ();
		if (PlayerPrefsX.GetBool ("BEGINNER_GUIDE", true)) 
		{
//			tutorialPanel.enabled = true;
//			tutoTip.text = "Select your hero. This will greatly influence the gameplay. The Ovate is a difficult choice if you are playing solo. The Champion is easy to play and can make a good choice if it's your first game.";
//			if (PlayerPrefs.GetString ("LANGAGE") == "Fr") 
//			{
//				tutoTip.text = "Selectionnez votre héros. Ceci influencera grandement le jeu. L'Ovate est un choix difficile si vous jouez seul. Le Champion est plus accessible si c'est votre première partie.";
//
//			}
		}
	}

	public void ActualizeLocSystem()
	{
		StartCoroutine (ActualizeTheLocator ());
	}

	IEnumerator ActualizeTheLocator()
	{
		locManager.Realtime_Updates = true;
		yield return new WaitForSeconds (0.3f);
		locManager.Realtime_Updates = false;
	}

	public void ShowAGameTip(string tip)
	{
		if (PlayerPrefsX.GetBool ("BEGINNER_GUIDE", true)) 
		{
			tutorialPanel.enabled = true;
			tutoTip.text = tip;
		}
	}
	public void DisconnectThePlayer()
	{
		if (isServer) 
		{
			StopPlayerFromJoining ();
			NetworkManager.singleton.StopHost ();
			return;
		}
		NetworkManager.singleton.StopClient ();
	}
		

//	public IEnumerator ChangeVolumeCurve()
//	{
//		float y = Time.time;
//		bool z = true;
//		while (z) 
//		{
//			yield return new WaitForSecondsRealtime (0.1f);
//			if (Camera.main.GetComponent<AudioSource> ().volume > 0f) {
//				Camera.main.GetComponent<AudioSource> ().volume -= 0.05f;
//			}
//			if (Time.time > y + delayTime) 
//				{
//				
//				Camera.main.GetComponent<AudioSource> ().volume = PlayerPrefs.GetFloat ("MUSIC_VOLUME");
//				z = false;
//				if (nightTime) {
//
//				} else {
//
//				}
//			}
//			yield break;
//		}
//
//	}
	[ServerCallback]
	public void Update()
	{
		if(Input.GetKeyDown(KeyCode.P))
			{
				actualReputation +=(Random.Range(-.21f,.21f));
			if (actualReputation < 0) 
			{
				actualReputation = 0;
			}
			if (actualReputation >1) 
			{
				actualReputation = 1;
			}
			}
	}
}