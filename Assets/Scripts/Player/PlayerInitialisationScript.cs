using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using HyperLuminalGames;

[NetworkSettings(channel = 0, sendInterval =0.5f)]

public class PlayerInitialisationScript : NetworkBehaviour 
{
	//ce script active/ désactive les autres scripts sur le joueur lorsqu'un joueur se connecte en fonction de
	// si il est le localplayer ou si il est sur le serveur etc...
	//il gere du coup tout ce qui touche q l'initialisation d'un joueur.
	//il se désactive au premier "lateupdate" pour pu faire chier aprés. ATTENTION.
	public SpriteRenderer minimapIcon;
	public Color mainPlayerColor;
	public Color enemyPlayerColor;
	public GameObject difficultyPanel;
	public GameObject heroSelectPanel;
	public GameObject childTankSkin;
	public GameObject childHealSkin;
	public GameObject childDpsSkin;
	public Button selectHeroTank1;
	public Button selectHeroHealer1;
	public Button selectHeroDps1;
	public Sprite tankAvatarImg;
	public Sprite healAvatarImg;
	public Sprite DpsAvatarImg;
	public Sprite tankAvatarImgMini;
	public Sprite healAvatarImgMini;
	public Sprite DpsAvatarImgMini;
	[Header("All ovate sounds:")]
	public AudioClip autoAHeal1;
	public AudioClip autoAHeal2;
	public AudioClip deadHeal;
	[Header("All hunter sounds:")]
	public AudioClip autoAArcher1;
	public AudioClip autoAArcher2;
	public AudioClip deadArcher;

	public PlayerIGManager myPlayerIGManager;
	public GenericManaScript myGeneManaScript;
	public PlayerAutoAttack myAutoAScript;
	public PlayerStatPlus myStatPlusScript;
	[SyncVar(hook = "ChangeMyName")]public string playerNickName;

//	public override void OnStartClient ()
//	{
//		ChangeMyName (playerNickName);
//		base.OnStartClient ();
//	}

	public override void OnStartLocalPlayer ()
	{
		GameManager.instanceGM.playerObj = gameObject;
		GameManager.instanceGM.ID = gameObject.GetComponent<NetworkIdentity> ().netId;
		//		Camera.main.transform.GetChild (0).gameObject.SetActive (false);
		CmdChangeName (PlayerPrefs.GetString ("PlayerNN"));
		difficultyPanel = GameObject.Find ("DifficultyPanel");
		heroSelectPanel = GameObject.Find ("HeroSelectionPanel");
		selectHeroTank1 = heroSelectPanel.transform.Find("ChampionPan").Find ("SelectTank1Btn").GetComponent<Button>();
		selectHeroHealer1 = heroSelectPanel.transform.Find("OvatePan").transform.Find ("SelectHeal1Btn").GetComponent<Button> ();
		selectHeroDps1 = heroSelectPanel.transform.Find("HunterPan").transform.Find ("SelectArcher1Btn").GetComponent<Button> ();
		selectHeroTank1.onClick.AddListener (ListenerSelectHeroTank1);
		selectHeroHealer1.onClick.AddListener (ListenerSelectHeroHeal1);
		selectHeroDps1.onClick.AddListener (ListenerSelectHeroDps1);
		base.OnStartLocalPlayer ();
	}
	// Use this for initialization
	void Start ()
	{
		if (isLocalPlayer) 
		{
			string playerNN;
			playerNN = PlayerPrefs.GetString ("PlayerNN");
//			ChangeMyName (playerNN);
			GetComponent<PlayerLevelUpManager> ().enabled = true;
			minimapIcon.color = mainPlayerColor;
			CameraController.instanceCamera.target = gameObject;
			CameraController.instanceCamera.Initialize ();
			GameObject.Find ("PlayerInterface2.0").GetComponent<Canvas> ().enabled = true;
			GameObject.Find ("PlayerNickNameTxt").GetComponent<Text> ().text = playerNN;
			if (!isServer) 
			{
				difficultyPanel.SetActive (false);
			}
		} else 
		{
			GetComponent<PlayerLevelUpManager> ().enabled = false; //juste pour etre sur
			StartCoroutine (SetProperColor ());
			if (!isServer) {
				StartCoroutine (StartInitName (playerNickName));
			}
		}
		if (isServer) 
		{
//			RpcChangeName ();
			GetComponentInChildren<PlayerEnnemyDetectionScript> ().enabled = true;
			if(isLocalPlayer)
			{
			GameObject.Find ("DifficultyPanel").GetComponent<ChooseDifficultyScript> ().enabled = true;
				if (NetworkManager.singleton.GetComponent<PlayerMenuSettings> ().isItOneLane) 
				{
					GameManager.instanceGM.soloGame = true;
					Destroy( GameObject.Find ("SpawnPointT2"));
				}
			}
		}
	}

	[Command]
	public void CmdChangeName (string nickName)
	{
		playerNickName = nickName;
		//		GetComponent<PlayerManager> ().playerNickname = nickName;
		//		if (!isLocalPlayer) 
		//		{
		//			GetComponent<PlayerManager> ().playerNNTxt.text = nickName;
		//			GetComponent<PlayerManager> ().playerUI.transform.GetChild (0).GetComponent<Text> ().text = nickName;
		//		}
	}

	public void ListenerSelectHeroTank1()
	{
		CmdSelectHeroTank1 ();
	}
	[Command]
	public void CmdSelectHeroTank1()
	{
		RpcHeroTank1Selected ();
	}
	[ClientRpc]
	public void RpcHeroTank1Selected()
	{
		GetComponent<PlayerClicToMove> ().enabled = true;
		GetComponent<PlayerTankCastTauntArea> ().enabled = true;
		GetComponent<PlayerTankCastDpsHealAoe> ().enabled = true;
		GetComponent<PlayerTankCastAvatar> ().enabled = true;
		childTankSkin.SetActive (true);
		myAutoAScript.enabled = true;
		myStatPlusScript.enabled = true;
		GameManager.instanceGM.messageManager.SendAnAlertMess (playerNickName + " will play the Champion.[Tank]", Color.green);

		myAutoAScript.anim = childTankSkin.GetComponentInChildren<Animator> ();
		myPlayerIGManager.deadAnimChildMesh = childTankSkin.transform.GetChild(0).gameObject;
		myPlayerIGManager.deadAnimChildMesh.GetComponent<Animator>().SetBool("stopwalk", true);
		myPlayerIGManager.heroChosen = "Champion";
		GetComponent<PlayerClicToMove> ().anim = childTankSkin.GetComponentInChildren<Animator> ();
		if (isServer) //pour toutes les sync var : ici / s'assurer que les scripts sont bien tous actifs normaleemtn c'est le cas ! 
		{
			myPlayerIGManager.maxHp = 520;
			myPlayerIGManager.currentHp = 520;
			myPlayerIGManager.regenHp = 15;
			myGeneManaScript.maxMp = 150;
			myGeneManaScript.currentMp = 150;
			myGeneManaScript.regenMp = 4;
			myPlayerIGManager.armorScore = 45;
			myAutoAScript.damage = 20;
			myAutoAScript.attackRate = 1;
			myAutoAScript.attackRange = 5;
			myPlayerIGManager.levelUpBonusHP = 75;
			myGeneManaScript.levelUpBonusMP = 12;
			myAutoAScript.levelUpBonusDamage = 3;
			myPlayerIGManager.levelUpBonusArmor = 4;
			myAutoAScript.attackSpeedStat = 0.8f;
			myStatPlusScript.doubleHPBonus = true;

		}
		//faire ici la config du hero tank1 pour tous
		if (isLocalPlayer) //si c'est ton perso et ton choix de perso : 
		{
			GetComponent<PlayerManager> ().avatarImg.sprite = tankAvatarImg;

			heroSelectPanel.GetComponentInParent<Canvas> ().enabled = false;
			ShowYourTip ();
		} else //si c'est le perso d'un autre joueur pour toi : 
		{
			GetComponent<PlayerManager> ().avatarImg.sprite = tankAvatarImgMini;
		}
	}
	public void ListenerSelectHeroHeal1()
	{
		CmdSelectHeroHeal1 ();
	}
	[Command]
	public void CmdSelectHeroHeal1()
	{
		RpcHeroHeal1Selected ();
	}
	[ClientRpc]
	public void RpcHeroHeal1Selected()
	{
		childHealSkin.SetActive (true);
		GetComponent<PlayerClicToMove> ().enabled = true;
		GameManager.instanceGM.messageManager.SendAnAlertMess (playerNickName + " will play the Ovate.[support]", Color.green);
		GetComponent<PlayerCastHealArea> ().enabled = true;
		GetComponent<PlayerHealerCastUlti> ().enabled = true;
		GetComponent<PlayerHealerCastInvokePet> ().enabled = true;
		myAutoAScript.enabled = true;
		myStatPlusScript.enabled = true;
		myAutoAScript.Att1 = autoAHeal1;
		myAutoAScript.Att2 = autoAHeal2;
		myPlayerIGManager.PlayerDeath = deadHeal;
		myPlayerIGManager.heroChosen = "Ovate";
		myAutoAScript.particule = childHealSkin.GetComponentInChildren<ParticleSystem> ();
		myAutoAScript.anim = childHealSkin.GetComponentInChildren<Animator> ();
		myPlayerIGManager.deadAnimChildMesh = childHealSkin.transform.GetChild(0).gameObject;
		myPlayerIGManager.deadAnimChildMesh.GetComponent<Animator>().SetBool("stopwalk", true);
		GetComponent<PlayerClicToMove> ().anim = childHealSkin.GetComponentInChildren<Animator> ();
		if (isServer) //pour toutes les sync var : ici / s'assurer que les scripts sont bien tous actifs normaleemtn c'est le cas ! 
		{
			GetComponentInChildren<PlayerEnnemyDetectionScript> ().gameObject.GetComponent<SphereCollider> ().radius = 1f;

			myPlayerIGManager.maxHp = 300;
			myPlayerIGManager.currentHp = 300;
			myPlayerIGManager.regenHp = 6;
			myGeneManaScript.maxMp = 220;
			myGeneManaScript.currentMp = 220;
			myGeneManaScript.regenMp = 8;
			myAutoAScript.damage = 8;
			myAutoAScript.attackRate = .7f;
			myAutoAScript.attackRange = 10;
			myPlayerIGManager.levelUpBonusHP = 25;
			myGeneManaScript.levelUpBonusMP = 22;
			myPlayerIGManager.armorScore = 10;
			myAutoAScript.levelUpBonusDamage = 2;
			myPlayerIGManager.levelUpBonusArmor = 1;
			myAutoAScript.attackSpeedStat = 1.1f;
			myStatPlusScript.doubleMPBonus = true;

		}
		//faire ici la config du hero tank1 pour tous
		if (isLocalPlayer) //si c'est ton perso et ton choix de perso : 
		{
			heroSelectPanel.GetComponentInParent<Canvas> ().enabled = false;
			ShowYourTip ();
			GetComponent<PlayerManager> ().avatarImg.sprite = healAvatarImg;

		} else //si c'est le perso d'un autre joueur pour toi : 
		{
			GetComponent<PlayerManager> ().avatarImg.sprite = healAvatarImgMini;
		}
	}
	public void ListenerSelectHeroDps1()
	{
		CmdSelectHeroDps1 ();
	}
	[Command]
	public void CmdSelectHeroDps1()
	{
		RpcHeroDps1Selected ();
	}
	[ClientRpc]
	public void RpcHeroDps1Selected()
	{
		childDpsSkin.SetActive (true);
		myAutoAScript.enabled = true;
		myStatPlusScript.enabled = true;

		GetComponent<PlayerClicToMove> ().enabled = true;

		GetComponent<PlayerArcherCastPassiveBoost> ().enabled = true;
		GetComponent<PlayerArcherCastArrowRain> ().enabled = true;
		GetComponent<PlayerArcherCastPoisonTrap> ().enabled = true;
		myAutoAScript.Att1 = autoAArcher1;
		myAutoAScript.Att2 = autoAArcher2;
		myPlayerIGManager.PlayerDeath = deadArcher;
		myAutoAScript.particule = childDpsSkin.GetComponentInChildren<ParticleSystem> ();
		myAutoAScript.anim = childDpsSkin.GetComponentInChildren<Animator> ();
		myPlayerIGManager.deadAnimChildMesh = childDpsSkin.transform.GetChild(0).gameObject;
		myPlayerIGManager.deadAnimChildMesh.GetComponent<Animator>().SetBool("stopwalk", true);
		myPlayerIGManager.heroChosen = "Hunter";
		GameManager.instanceGM.messageManager.SendAnAlertMess (playerNickName + " will play the Hunter.[ADC]", Color.green);

		GetComponent<PlayerClicToMove> ().anim = childDpsSkin.GetComponentInChildren<Animator> ();
		if (isServer) //pour toutes les sync var : ici / s'assurer que les scripts sont bien tous actifs normaleemtn c'est le cas ! 
		{
			GetComponentInChildren<PlayerEnnemyDetectionScript> ().gameObject.GetComponent<SphereCollider> ().radius = 1.8f;
			myPlayerIGManager.maxHp = 440;
			myPlayerIGManager.currentHp = 440;
			myPlayerIGManager.regenHp = 8;
			myGeneManaScript.maxMp = 130;
			myGeneManaScript.currentMp = 130;
			myGeneManaScript.regenMp = 5;
			myAutoAScript.damage = 22;
			myAutoAScript.attackRate = .8f;
			myAutoAScript.attackRange = 15;
			myPlayerIGManager.levelUpBonusHP = 35;
			myGeneManaScript.levelUpBonusMP = 12;

			myPlayerIGManager.armorScore = 15;
			myPlayerIGManager.levelUpBonusArmor = 2;
			myAutoAScript.levelUpBonusDamage = 5;
			myAutoAScript.attackSpeedStat = 1.25f;
			myStatPlusScript.doubleDpsBonus = true;

		}
		//faire ici la config du hero tank1 pour tous
		if (isLocalPlayer) //si c'est ton perso et ton choix de perso : 
		{
			heroSelectPanel.GetComponentInParent<Canvas> ().enabled = false;
			GetComponent<PlayerManager> ().avatarImg.sprite = DpsAvatarImg;

			ShowYourTip ();
		} else //si c'est le perso d'un autre joueur pour toi : 
		{
			GetComponent<PlayerManager> ().avatarImg.sprite = DpsAvatarImgMini;
		}
	}
	public void ChangeMyName (string str)
	{
		StartCoroutine(StartInitName(str));

	}
	IEnumerator StartInitName(string str)
	{
		yield return new WaitForSeconds (0.4f);
		playerNickName = str;
		gameObject.name = playerNickName + netId.ToString();
		GetComponent<PlayerManager> ().playerNickname = playerNickName;
		GetComponent<Location> ().Display_1_Text = playerNickName;
		if (!isLocalPlayer) 
		{
			GetComponent<PlayerManager> ().playerUI.transform.GetChild (0).GetComponent<Text> ().text = str;

		}
	}


//	[ClientRpc]
//	public void RpcChangeName ()
//	{
//		gameObject.name = "Player" + netId.ToString ();
//
//	}
	public override void OnStartServer ()
	{
		StartCoroutine (TellNewPlayerHasJoin ());
		base.OnStartServer ();

	}

	IEnumerator SetProperColor()
	{
		yield return new WaitForSeconds (0.2f); // attendre que la collision est register le joueur en fait...faudra opti.
		if(GameManager.instanceGM.team1ID.Contains(this.netId))
		{
			if(GameManager.instanceGM.isTeam1)
			{
				yield return null;
			}else
			{
				minimapIcon.color = enemyPlayerColor;
			}
		}
		if(GameManager.instanceGM.team2ID.Contains(this.netId))
		{
			if(GameManager.instanceGM.isTeam2)
			{
				yield return null;
			}else
			{
				minimapIcon.color = enemyPlayerColor;
			}
		}
	}
	[ClientRpc]
	public void RpcCallMessage(string mess)
	{
		GameManager.instanceGM.messageManager.SendAnAlertMess (mess, Color.green);

	}

	IEnumerator TellNewPlayerHasJoin()
	{
		yield return new WaitForSeconds (1.5f);
		RpcCallMessage (playerNickName + " has joined the game.");
		if (PlayerPrefs.GetString ("LANGAGE") == "Fr") 
		{
			RpcCallMessage (playerNickName + " a rejoind la partie.");

		}
	}

	void ShowYourTip()
	{
		GameManager.instanceGM.ShowAGameTip ("To move your hero, right clic on the ground. You can attack an enemy by right clicking on it as well. Use 'L' to lock/unlock the camera and 'Spacebar' to center the view on your hero.");
		if (PlayerPrefs.GetString ("LANGAGE") == "Fr") 
		{
			GameManager.instanceGM.ShowAGameTip ("Pour déplacer votre héro, faites un clic droit sur le sol. Vous pouvez attaquer avec le bouton droit de la souris. Utilisez 'L' pour verrouiller/déverrouiller le suivi camera et 'Espace' pour recentrer la vue sur votre héro.");

		}
	}

}
