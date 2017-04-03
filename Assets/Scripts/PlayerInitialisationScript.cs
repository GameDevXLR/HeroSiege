﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

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
			GetComponent<PlayerLevelUpManager> ().enabled = false;
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
		yield return new WaitForSeconds (1f);
		RpcCallMessage (playerNickName + " has joined the game.");

	}

}
