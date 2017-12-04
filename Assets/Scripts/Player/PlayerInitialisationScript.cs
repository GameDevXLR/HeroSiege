using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using HyperLuminalGames;
using UnityEngine.Events;

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
	[Header("Heros skin selection materials and avatars.")]
	public SkinnedMeshRenderer tankSkinMesh;
	public SkinnedMeshRenderer healSkinMesh;
	public SkinnedMeshRenderer dpsSkinMesh;
	private SkinnedMeshRenderer actualSkinMesh;
	public Material tankMatSkin1;
	public Material tankMatSkin2;
	public Material tankMatSkin3;
	public Material healMatSkin1;
	public Material healMatSkin2;
	public Material healMatSkin3;
	public Material dpsMatSkin1;
	public Material dpsMatSkin2;
	public Material dpsMatSkin3;
	public Button selectHeroTank1;
	public Button selectHeroHealer1;
	public Button selectHeroDps1;
	public Sprite tankAvatarImg;
	public Sprite healAvatarImg;
	public Sprite DpsAvatarImg;
	public Sprite tankAvatar2Img;
	public Sprite healAvatar2Img;
	public Sprite DpsAvatar2Img;
	public Sprite tankAvatar3Img;
	public Sprite healAvatar3Img;
	public Sprite DpsAvatar3Img;
	public Sprite tankAvatarImgMini;
	public Sprite healAvatarImgMini;
	public Sprite DpsAvatarImgMini;
	public AudioClip[] tankPainSounds;
	[Header("All ovate sounds:")]
	public AudioClip[] healPainSounds;
	public AudioClip autoAHeal1;
	public AudioClip autoAHeal2;
	public AudioClip deadHeal;
	[Header("All hunter sounds:")]
	public AudioClip[] dpsPainSounds;
	public AudioClip autoAArcher1;
	public AudioClip autoAArcher2;
	public AudioClip deadArcher;
	[Header("HeroSelection stuff.")]
	public UnityEvent selectedHero;
	public PlayerIGManager myPlayerIGManager;
	public GenericManaScript myGeneManaScript;
	public PlayerAutoAttack myAutoAScript;
	public PlayerStatPlus myStatPlusScript;
	[SyncVar(hook = "ChangeMyName")]public string playerNickName;

	public Color selectedHeroColor;
	public Color defaultHeroColor;

//	Image heroArtPlaceholder;
	public Sprite tankArtwork;
	public Sprite ovateArtwork;
	public Sprite archerArtwork;
	public Button lockHeroBtn;
	public GameObject nextHeroBtn;
	public GameObject previousHeroBtn;

	[Header("Gestion de ce qui se passe après le lockHero.")]
	public Sprite cadreHeroSelectedEffect;
	public Sprite mainCadreHeroSelectedEffect;

	//permet de remplacer l'image cible par le artwork approprié de nassima
//	public Sprite tankIllustration;
//	public Sprite healIllustration;
//	public Sprite archerIllustration;
//
//	[SerializeField]private Sprite TmpIllustration;


	//remis récemment, ca avait été enlevé...C'est le bordel pour le sync des noms des joueurs. A optimiser soon. Voir si ca suffit a débug le tout cette fonction. A TEST (novembre2017)
	//Re enlevé le 30 novembre
//	public override void OnStartClient ()
//	{
//		ChangeMyName (playerNickName);
//		base.OnStartClient ();
//	}

	public override void OnStartLocalPlayer ()
	{
		GameManager.instanceGM.playerObj = gameObject;
		GameManager.instanceGM.ID = gameObject.GetComponent<NetworkIdentity> ().netId;
		//	Camera.main.transform.GetChild (0).gameObject.SetActive (false);
		CmdChangeName (PlayerPrefs.GetString ("PlayerNN"));
		difficultyPanel = GameObject.Find ("NewDiffPan");
		if (isServer) 
		{
            //	difficultyPanel.transform.localScale = new Vector3 (0.5f,0.5f,0.5f);
		}
		heroSelectPanel = GameObject.Find ("HeroSelectionPanel");
		nextHeroBtn= heroSelectPanel.transform.parent.Find ("NextHeroBtn").gameObject;
		previousHeroBtn =  heroSelectPanel.transform.parent.Find ("PreviousHeroBtn").gameObject;
		selectHeroTank1 = heroSelectPanel.transform.Find("ChampionPan").GetChild(0).Find ("SelectTank1Btn").GetComponent<Button>();
		selectHeroHealer1 = heroSelectPanel.transform.Find("OvatePan").GetChild(0).Find ("SelectHeal1Btn").GetComponent<Button> ();
		selectHeroDps1 = heroSelectPanel.transform.Find("HunterPan").GetChild(0).Find ("SelectArcher1Btn").GetComponent<Button> ();
		lockHeroBtn = GameObject.Find ("LockHero").GetComponent<Button>();
		selectHeroTank1.onClick.AddListener (ListenerSelectHeroTank1);
		selectHeroHealer1.onClick.AddListener (ListenerSelectHeroHeal1);
		selectHeroDps1.onClick.AddListener (ListenerSelectHeroDps1);
		lockHeroBtn.onClick.AddListener (LockMyHeroSelec);
		base.OnStartLocalPlayer ();
	}
	// Use this for initialization
	void Start ()
	{
		if (isLocalPlayer) 
		{
			
//			heroArtPlaceholder = GameObject.Find ("bg_select").GetComponent<Image> ();
			selectedHero.AddListener (CapsuleSelectTank);
			string playerNN;
			playerNN = PlayerPrefs.GetString ("PlayerNN");
			CmdChangeName(playerNN);
			GetComponent<PlayerLevelUpManager> ().enabled = true;
			minimapIcon.color = mainPlayerColor;
			GameObject.Find ("PlayerInterface2.0").GetComponent<Canvas> ().enabled = true;
			GameObject.Find ("PlayerNickNameTxt").GetComponent<Text> ().text = playerNN;
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
            //	RpcChangeName ();
            NetworkUtils.Instance.addConn(GetComponent<NetworkIdentity>().connectionToClient);
			GetComponentInChildren<PlayerEnnemyDetectionScript> ().enabled = true;
			if(isLocalPlayer)
			{
			    GameObject.Find ("NewDiffPan").GetComponent<ChooseDifficultyScript> ().enabled = true;
				if (ExampleNetworkManager.singleton.GetComponent<PlayerMenuSettings> ().isItOneLane) 
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
		ChangeMyName (nickName);

        NetworkUtils.Instance.listPlayer[nickName] = GetComponent<NetworkIdentity>().connectionToClient;

    }
	public void LockMyHeroSelec()
	{
		CmdSayILocked ();
	}
	[Command]
	public void CmdSayILocked()
	{
		RpcLockPlayer ();
		
	}
	[ClientRpc]
	public void RpcLockPlayer()
	{
		if (isLocalPlayer) 
		{
//			GameObject.Find ("GraphicsBeneathHeroPan").SetActive (false);
//			heroSelectPanel.SetActive (false);
//			lockHeroBtn.enabled = false;
			GameObject go = heroSelectPanel.transform.parent.Find("GraphicsBeneathHeroPan").gameObject;
			go.transform.Find ("CadreSideL").gameObject.SetActive (false);
			go.transform.Find ("CadreSideR").gameObject.SetActive (false);
			go.transform.Find ("CadreHeroSelected").GetComponent<Image>().sprite = cadreHeroSelectedEffect ;
			go.transform.Find ("CadreMain").GetComponent<Image>().sprite = mainCadreHeroSelectedEffect ;

			lockHeroBtn.gameObject.SetActive(false);
			nextHeroBtn.SetActive( false);
			previousHeroBtn.SetActive( false);

			//permet de remplacer l'image cible par le artwork approprié de nassima
//			GameObject.Find ("bg_select").GetComponent<Image> ().sprite = TmpIllustration;
		}
//		GetComponent<PlayerManager> ().playerSelecHeroChosenImg.CrossFadeAlpha(255f,.5f,false) ;
		GetComponent<PlayerManager> ().playerSelecUI.transform.Find ("LockImg").GetComponent<Image> ().enabled = true;


	}

	public void ListenerSelectHeroTank1()
	{
		selectedHero.RemoveAllListeners ();
		selectedHero.AddListener (CapsuleSelectTank);
		//ajouter ici le code pour montrer son choix
		CmdSayIAmTank ();
		lockHeroBtn.interactable = true;
//		lockHeroBtn.transform.GetComponent<Image> ().color = Color.black;
//		CmdSelectHeroTank1 ();
	}
	public void CapsuleSelectTank()
	{
		//permet de remplacer l'image cible par le artwork approprié de nassima
//		TmpIllustration = tankIllustration;
		CmdSelectHeroTank1 ();
		CmdSendColor(PlayerPrefs.GetInt("SKIN_COLOR", 1));
	}
	[Command]
	public void CmdSendColor(int i)
	{
		RpcReceiveSkin (i);


	}
	[ClientRpc]
	public void RpcReceiveSkin(int i)
	{
		if (myPlayerIGManager.heroChosen == "Ovate") 
		{
			actualSkinMesh = healSkinMesh;
			switch (i) {
			case 1:
				actualSkinMesh.material = healMatSkin1;
				if(isLocalPlayer){
					
				GetComponent<PlayerManager> ().avatarImg.sprite = healAvatarImg;
			}
				break;		
			case 2:
				actualSkinMesh.material = healMatSkin2;
				if(isLocalPlayer){
					
				GetComponent<PlayerManager> ().avatarImg.sprite = healAvatar2Img;
			}
				break;
			case 3:
				actualSkinMesh.material = healMatSkin3;
				if(isLocalPlayer){
					
				GetComponent<PlayerManager> ().avatarImg.sprite = healAvatar3Img;
			}
				break;
			default:
				break;
			}
		} else if (myPlayerIGManager.heroChosen == "Hunter") 
		{
			actualSkinMesh = dpsSkinMesh;
			switch (i) 
			{
			case 1:
				actualSkinMesh.material = dpsMatSkin1;
				if (isLocalPlayer) {
					GetComponent<PlayerManager> ().avatarImg.sprite = DpsAvatarImg;
				}
				break;		
			case 2:
				actualSkinMesh.material = dpsMatSkin2;
					if(isLocalPlayer){
						
				GetComponent<PlayerManager> ().avatarImg.sprite = DpsAvatar2Img;
					}
				break;
			case 3:
				actualSkinMesh.material = dpsMatSkin3;
						if(isLocalPlayer){
							
				GetComponent<PlayerManager> ().avatarImg.sprite = DpsAvatar3Img;
						}
				break;
			default:
				break;
			}
		} else 
		{
			actualSkinMesh = tankSkinMesh;
			switch (i) 
			{
			case 1:
				actualSkinMesh.material = tankMatSkin1;
							if(isLocalPlayer){
								
				GetComponent<PlayerManager> ().avatarImg.sprite = tankAvatarImg;
							}
				break;		
			case 2:
				actualSkinMesh.material = tankMatSkin2;
								if(isLocalPlayer){
									
				GetComponent<PlayerManager> ().avatarImg.sprite = tankAvatar2Img;
								}
				break;
			case 3:
				actualSkinMesh.material = tankMatSkin3;
									if(isLocalPlayer){
										
				GetComponent<PlayerManager> ().avatarImg.sprite = tankAvatar3Img;
									}
				break;
			default:
				break;
			}
		}
	}
	[Command]
	public void CmdSelectHeroTank1()
	{
		RpcHeroTank1Selected ();
	}	
	[Command]
	public void CmdSayIAmTank()
	{
		RpcTankSelected ();
	}
	[ClientRpc]
	public void RpcTankSelected()
	{
		if (isLocalPlayer) 
		{
//			heroArtPlaceholder.sprite = tankArtwork;
//			selectHeroDps1.transform.parent.parent.GetComponent<Image> ().color = selectedHeroColor;
//			selectHeroHealer1.transform.parent.parent.GetComponent<Image> ().color = selectedHeroColor;
//			selectHeroTank1.transform.parent.parent.GetComponent<Image> ().color = defaultHeroColor;
		}
		GetComponent<PlayerManager> ().playerSelecHeroChosenImg.sprite = tankAvatarImgMini;

		//ajouter ici les changements lié aux icones des joueurs dans les panneaux de team
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
		myPlayerIGManager.painSound = tankPainSounds;

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
			myAutoAScript.damage = 18;
			myAutoAScript.attackRate = 1;
			myAutoAScript.attackRange = 5;
			myPlayerIGManager.levelUpBonusHP = 76;
			myGeneManaScript.levelUpBonusMP = 18;
			myAutoAScript.levelUpBonusDamage = 5;
			myPlayerIGManager.levelUpBonusArmor = 4;
			myAutoAScript.attackSpeedStat = 0.8f;
			myStatPlusScript.doubleHPBonus = true;

		}
		//faire ici la config du hero tank1 pour tous
		if (isLocalPlayer) //si c'est ton perso et ton choix de perso : 
		{
			GetComponent<PlayerManager> ().avatarImg.sprite = tankAvatarImg;

//			heroSelectPanel.GetComponentInParent<Canvas> ().enabled = false;
			ShowYourTip ();
		} else //si c'est le perso d'un autre joueur pour toi : 
		{
			GetComponent<PlayerManager> ().avatarImg.sprite = tankAvatarImgMini;
		}
	}
	public void ListenerSelectHeroHeal1()
	{
		selectedHero.RemoveAllListeners ();
		selectedHero.AddListener (CapsuleSelectHeal);
		//ajouter ici le code pour montrer son choix
		CmdSayIAmHeal ();
		//rendre le bouton interactif est, je crois, devenu inutile. A vérifier puis supprimer la ligne ci dessous dans toutes les méthodes "listenerSelectHeroXXXXX"
		lockHeroBtn.interactable = true;
//		lockHeroBtn.transform.GetComponent<Image> ().color = Color.black;
	}
	public void CapsuleSelectHeal()
	{
		//permet de remplacer l'image cible par le artwork approprié de nassima
//		TmpIllustration = healIllustration;
		CmdSelectHeroHeal1 ();
		CmdSendColor(PlayerPrefs.GetInt("SKIN_COLOR", 1));

	}
	[Command]
	public void CmdSelectHeroHeal1()
	{
		RpcHeroHeal1Selected ();
	}
	[Command]
	public void CmdSayIAmHeal()
	{
		RpcHealSelected ();
	}
	[ClientRpc]
	public void RpcHealSelected()
	{
		if (isLocalPlayer) 
		{
//			heroArtPlaceholder.sprite = ovateArtwork;

//			selectHeroDps1.transform.parent.parent.GetComponent<Image> ().color = selectedHeroColor;
//			selectHeroHealer1.transform.parent.parent.GetComponent<Image> ().color = defaultHeroColor;
//			selectHeroTank1.transform.parent.parent.GetComponent<Image> ().color = selectedHeroColor;
		}
		GetComponent<PlayerManager> ().playerSelecHeroChosenImg.sprite = healAvatarImgMini;

		//ajouter ici les changements lié aux icones des joueurs dans les panneaux de team
		
	}
	[ClientRpc]
	public void RpcHeroHeal1Selected()
	{
		childHealSkin.SetActive (true);
		GetComponent<PlayerClicToMove> ().enabled = true;
		GameManager.instanceGM.messageManager.SendAnAlertMess (playerNickName + " will play the Ovate.[support]", Color.green);
		GetComponent<PlayerCastHealArea> ().enabled = true;
		GetComponent<PlayerHealerCastUlti> ().enabled = true;
		GetComponent<PlayerCastTowerMage> ().enabled = true;
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
		myPlayerIGManager.painSound = healPainSounds;
		GetComponent<PlayerClicToMove> ().anim = childHealSkin.GetComponentInChildren<Animator> ();
		if (isServer) //pour toutes les sync var : ici / s'assurer que les scripts sont bien tous actifs normaleemtn c'est le cas ! 
		{
			GetComponentInChildren<PlayerEnnemyDetectionScript> ().gameObject.GetComponent<SphereCollider> ().radius = 1f;

			myPlayerIGManager.maxHp = 350;
			myPlayerIGManager.currentHp = 300;
			myPlayerIGManager.regenHp = 6;
			myGeneManaScript.maxMp = 220;
			myGeneManaScript.currentMp = 220;
			myGeneManaScript.regenMp = 8;
			myAutoAScript.damage = 16;
			myAutoAScript.attackRate = .7f;
			myAutoAScript.attackRange = 15;
			myPlayerIGManager.levelUpBonusHP = 43;
			myGeneManaScript.levelUpBonusMP = 42;
			myPlayerIGManager.armorScore = 10;
			myAutoAScript.levelUpBonusDamage = 5;
			myPlayerIGManager.levelUpBonusArmor = 1;
			myAutoAScript.attackSpeedStat = 1.1f;
			myStatPlusScript.doubleMPBonus = true;

		}
		//faire ici la config du hero tank1 pour tous
		if (isLocalPlayer) //si c'est ton perso et ton choix de perso : 
		{
			ShowYourTip ();
			GetComponent<PlayerManager> ().avatarImg.sprite = healAvatarImg;

		} else //si c'est le perso d'un autre joueur pour toi : 
		{
			GetComponent<PlayerManager> ().avatarImg.sprite = healAvatarImgMini;
		}
	}
	public void ListenerSelectHeroDps1()
	{
		selectedHero.RemoveAllListeners ();
		selectedHero.AddListener (CapsuleSelectDps);
		//ajouter ici le code pour montrer son choix
		CmdSayIAmDps ();
		lockHeroBtn.interactable = true;
//		lockHeroBtn.transform.GetComponent<Image> ().color = Color.black;
	}
	public void CapsuleSelectDps()
	{
		//permet de remplacer l'image cible par le artwork approprié de nassima
//		TmpIllustration = archerIllustration;
		CmdSelectHeroDps1 ();
		CmdSendColor(PlayerPrefs.GetInt("SKIN_COLOR", 1));

	}
	[Command]
	public void CmdSelectHeroDps1()
	{
		RpcHeroDps1Selected ();
	}
	[Command]
	public void CmdSayIAmDps()
	{
		RpcDpsSelected ();
	}
	[ClientRpc]
	public void RpcDpsSelected()
	{
		if (isLocalPlayer) 
		{
//			heroArtPlaceholder.sprite = archerArtwork; 

//			selectHeroDps1.transform.parent.parent.GetComponent<Image> ().color = defaultHeroColor;
//			selectHeroHealer1.transform.parent.parent.GetComponent<Image> ().color = selectedHeroColor;
//			selectHeroTank1.transform.parent.parent.GetComponent<Image> ().color = selectedHeroColor;
		}
		GetComponent<PlayerManager> ().playerSelecHeroChosenImg.sprite = DpsAvatarImgMini;

		//ajouter ici les changements lié aux icones des joueurs dans les panneaux de team
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
		myPlayerIGManager.painSound = dpsPainSounds;
		myPlayerIGManager.heroChosen = "Hunter";
		GameManager.instanceGM.messageManager.SendAnAlertMess (playerNickName + " will play the Hunter.[ADC]", Color.green);

		GetComponent<PlayerClicToMove> ().anim = childDpsSkin.GetComponentInChildren<Animator> ();
		if (isServer) //pour toutes les sync var : ici / s'assurer que les scripts sont bien tous actifs normaleemtn c'est le cas ! 
		{
			GetComponentInChildren<PlayerEnnemyDetectionScript> ().gameObject.GetComponent<SphereCollider> ().radius = 1.2f;
			myPlayerIGManager.maxHp = 440;
			myPlayerIGManager.currentHp = 440;
			myPlayerIGManager.regenHp = 8;
			myGeneManaScript.maxMp = 130;
			myGeneManaScript.currentMp = 130;
			myGeneManaScript.regenMp = 5;
			myAutoAScript.damage = 20;
			myAutoAScript.attackRate = .8f;
			myAutoAScript.attackRange = 20;
			myPlayerIGManager.levelUpBonusHP = 54;
			myGeneManaScript.levelUpBonusMP = 22;

			myPlayerIGManager.armorScore = 15;
			myPlayerIGManager.levelUpBonusArmor = 2;
			myAutoAScript.levelUpBonusDamage = 7;
			myAutoAScript.attackSpeedStat = 1.25f;
			myAutoAScript.attackAnimTime = 1.25f;
			myStatPlusScript.doubleDpsBonus = true;

		}
		//faire ici la config du hero tank1 pour tous
		if (isLocalPlayer) //si c'est ton perso et ton choix de perso : 
		{
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
		if (!isServer) {
			playerNickName = str;
		}
		gameObject.name = playerNickName + netId.ToString();
		GetComponent<PlayerManager> ().playerNickname = playerNickName;
		//GetComponent<PlayerManager> ().playerSelecUI.GetComponentInChildren<Text> ().text = playerNickName;
		yield return new WaitForSeconds (3f);
		GetComponent<Location> ().Display_1_Text = playerNickName;
		if (!isLocalPlayer) 
		{
			GetComponent<PlayerManager> ().playerUI.transform.GetChild (0).GetComponent<Text> ().text = str;

		}
	}

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
		if (isLocalPlayer) 
		{
			StartCoroutine (HideTheLoadingScreen ());
		}

	}

	IEnumerator HideTheLoadingScreen()
	{
		GameObject go = GameObject.Find ("LoadingCanvas");
		go.GetComponent<CanvasScaler> ().uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
		float tmp = go.GetComponent<CanvasScaler> ().scaleFactor;
		while (go.GetComponent<CanvasScaler>().scaleFactor>0.02f) 
		{
			go.GetComponent<CanvasScaler>().scaleFactor -=.05f;
			yield return new WaitForEndOfFrame ();
		}
		go.GetComponent<CanvasScaler> ().scaleFactor = tmp;
		go.GetComponent<CanvasScaler> ().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

		go.GetComponent<Canvas> ().enabled = false;
		
	}

	IEnumerator TellNewPlayerHasJoin()
	{
		yield return new WaitForSeconds (3f);
		if (PlayerPrefs.GetString ("LANGAGE") == "Fr") {
			RpcCallMessage (playerNickName + " a rejoind la partie.");

		} else 
		{
			RpcCallMessage (playerNickName + " has joined the game.");
			
		}
	}

	void ShowYourTip()
	{
		if (PlayerPrefs.GetString ("LANGAGE") == "Fr") {
			GameManager.instanceGM.ShowAGameTip ("Pour déplacer votre héros, faites un clic droit sur le sol. Vous pouvez attaquer avec le bouton droit de la souris. Utilisez 'L' pour verrouiller/déverrouiller le suivi camera et 'Espace' pour recentrer la vue sur votre héro.");

		} else {
			GameManager.instanceGM.ShowAGameTip ("To move your hero, right clic on the ground. You can attack an enemy by right clicking on it as well. Use 'L' to lock/unlock the camera and 'Spacebar' to center the view on your hero.");
		}
	}

}
