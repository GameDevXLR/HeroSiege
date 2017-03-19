using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

[NetworkSettings(channel = 1, sendInterval =5f)]

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
			if(GameManager.instanceGM.team1ID.Contains(this.netId))
				{
					if(GameManager.instanceGM.isTeam1)
					{
						return;
					}else
					{
						minimapIcon.color = enemyPlayerColor;
					}
				}
			if(GameManager.instanceGM.team2ID.Contains(this.netId))
			{
				if(GameManager.instanceGM.isTeam2)
				{
					return;
				}else
				{
					minimapIcon.color = enemyPlayerColor;
				}
			}
		}
		if (isServer) 
		{
			GetComponentInChildren<PlayerEnnemyDetectionScript> ().enabled = true;
			if(isLocalPlayer){
			GameObject.Find ("DifficultyPanel").GetComponent<ChooseDifficultyScript> ().enabled = true;
			}
		}
	}

	public override void OnStartLocalPlayer ()
	{
		GameManager.instanceGM.playerObj = gameObject;
		GameManager.instanceGM.ID = gameObject.GetComponent<NetworkIdentity> ().netId;
		base.OnStartLocalPlayer ();
	}
	public override void OnStartServer ()
	{

			GameManager.instanceGM.messageManager.SendAnAlertMess ("A new player has joined the game.", Color.green);
		base.OnStartServer ();
	}


}
