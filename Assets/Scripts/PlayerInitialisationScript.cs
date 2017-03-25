using System.Collections;
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
	// Use this for initialization
	void Start ()
	{
		if (isLocalPlayer) 
		{
			
			difficultyPanel = GameObject.Find ("DifficultyPanel");
			GetComponent<PlayerLevelUpManager> ().enabled = true;
			minimapIcon.color = mainPlayerColor;
			CameraController.instanceCamera.target = gameObject;
			CameraController.instanceCamera.Initialize ();
			GameObject.Find ("PlayerInterface").GetComponent<Canvas> ().enabled = true;

			if (!isServer) 
			{
				difficultyPanel.SetActive (false);
			}
		} else 
		{
			StartCoroutine (SetProperColor ());
		}
		if (isServer) 
		{
			GetComponentInChildren<PlayerEnnemyDetectionScript> ().enabled = true;
			if(isLocalPlayer)
			{
			GameObject.Find ("DifficultyPanel").GetComponent<ChooseDifficultyScript> ().enabled = true;
			}
		}
		gameObject.name = "Player" + netId.ToString ();
	}

	public override void OnStartLocalPlayer ()
	{
		GameManager.instanceGM.playerObj = gameObject;
		GameManager.instanceGM.ID = gameObject.GetComponent<NetworkIdentity> ().netId;
		Camera.main.transform.GetChild (0).gameObject.SetActive (false);
		base.OnStartLocalPlayer ();
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

	}

	IEnumerator TellNewPlayerHasJoin()
	{
		yield return new WaitForSeconds (0.2f);
		RpcCallMessage ("A new player has joined the game.");

	}

}
