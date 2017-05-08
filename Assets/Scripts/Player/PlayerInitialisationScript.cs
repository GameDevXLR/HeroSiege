﻿using System.Collections;
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
	public GenericLifeScript myGeneLifeScript;
	public GenericManaScript myGeneManaScript;
	public PlayerAutoAttack myAutoAScript;
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
		selectHeroTank1 = heroSelectPanel.transform.Find ("SelectTank1Btn").GetComponent<Button>();
		selectHeroHealer1 = heroSelectPanel.transform.Find ("SelectHeal1Btn").GetComponent<Button> ();
		selectHeroDps1 = heroSelectPanel.transform.Find ("SelectArcher1Btn").GetComponent<Button> ();
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
		GetComponent<PlayerTankCastTauntArea> ().enabled = true;
		GetComponent<PlayerTankCastDpsHealAoe> ().enabled = true;
		GetComponent<PlayerTankCastAvatar> ().enabled = true;
		childTankSkin.SetActive (true);
		myAutoAScript.anim = childTankSkin.GetComponentInChildren<Animator> ();
		myGeneLifeScript.deadAnimChildMesh = childTankSkin.transform.GetChild(0).gameObject;
		myGeneLifeScript.deadAnimChildMesh.GetComponent<Animator>().SetBool("stopwalk", true);

		GetComponent<PlayerClicToMove> ().anim = childTankSkin.GetComponentInChildren<Animator> ();
		if (isServer) //pour toutes les sync var : ici / s'assurer que les scripts sont bien tous actifs normaleemtn c'est le cas ! 
		{
			myGeneLifeScript.maxHp = 300;
			myGeneLifeScript.currentHp = 300;
			myGeneLifeScript.regenHp = 12;
			myGeneManaScript.maxMp = 120;
			myGeneManaScript.currentMp = 120;
			myGeneManaScript.regenMp = 6;
			myAutoAScript.damage = 20;
			myAutoAScript.attackRate = 1;
			myAutoAScript.attackRange = 4;

		}
		//faire ici la config du hero tank1 pour tous
		if (isLocalPlayer) //si c'est ton perso et ton choix de perso : 
		{
			heroSelectPanel.GetComponentInParent<Canvas> ().enabled = false;
		} else //si c'est le perso d'un autre joueur pour toi : 
		{
			//rien pour le moment ? 
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
		GetComponent<PlayerCastHealArea> ().enabled = true;
		GetComponent<PlayerHealerCastUlti> ().enabled = true;
		GetComponent<PlayerHealerCastInvokePet> ().enabled = true;
		myAutoAScript.anim = childHealSkin.GetComponentInChildren<Animator> ();
		myGeneLifeScript.deadAnimChildMesh = childHealSkin.transform.GetChild(0).gameObject;
		myGeneLifeScript.deadAnimChildMesh.GetComponent<Animator>().SetBool("stopwalk", true);
		GetComponent<PlayerClicToMove> ().anim = childHealSkin.GetComponentInChildren<Animator> ();
		if (isServer) //pour toutes les sync var : ici / s'assurer que les scripts sont bien tous actifs normaleemtn c'est le cas ! 
		{
			myGeneLifeScript.maxHp = 150;
			myGeneLifeScript.currentHp = 150;
			myGeneLifeScript.regenHp = 6;
			myGeneManaScript.maxMp = 230;
			myGeneManaScript.currentMp = 230;
			myGeneManaScript.regenMp = 12;
			myAutoAScript.damage = 8;
			myAutoAScript.attackRate = .5f;
			myAutoAScript.attackRange = 11;

		}
		//faire ici la config du hero tank1 pour tous
		if (isLocalPlayer) //si c'est ton perso et ton choix de perso : 
		{
			heroSelectPanel.GetComponentInParent<Canvas> ().enabled = false;
		} else //si c'est le perso d'un autre joueur pour toi : 
		{
			//rien pour le moment ? 
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
		GetComponent<PlayerCastHealArea> ().enabled = true;
		GetComponent<PlayerArcherCastArrowRain> ().enabled = true;
		GetComponent<PlayerHealerCastInvokePet> ().enabled = true;
		myAutoAScript.anim = childDpsSkin.GetComponentInChildren<Animator> ();
		myGeneLifeScript.deadAnimChildMesh = childDpsSkin.transform.GetChild(0).gameObject;
		myGeneLifeScript.deadAnimChildMesh.GetComponent<Animator>().SetBool("stopwalk", true);

		GetComponent<PlayerClicToMove> ().anim = childDpsSkin.GetComponentInChildren<Animator> ();
		if (isServer) //pour toutes les sync var : ici / s'assurer que les scripts sont bien tous actifs normaleemtn c'est le cas ! 
		{
			myGeneLifeScript.maxHp = 180;
			myGeneLifeScript.currentHp = 180;
			myGeneLifeScript.regenHp = 5;
			myGeneManaScript.maxMp = 130;
			myGeneManaScript.currentMp = 130;
			myGeneManaScript.regenMp = 5;
			myAutoAScript.damage = 25;
			myAutoAScript.attackRate = 1f;
			myAutoAScript.attackRange = 15;

		}
		//faire ici la config du hero tank1 pour tous
		if (isLocalPlayer) //si c'est ton perso et ton choix de perso : 
		{
			heroSelectPanel.GetComponentInParent<Canvas> ().enabled = false;
		} else //si c'est le perso d'un autre joueur pour toi : 
		{
			//rien pour le moment ? 
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

	}

}
