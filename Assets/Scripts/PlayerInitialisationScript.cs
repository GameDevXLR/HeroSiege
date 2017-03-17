using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class PlayerInitialisationScript : NetworkBehaviour 
{
	//ce script active/ désactive les autres scripts sur le joueur lorsqu'un joueur se connecte en fonction de
	// si il est le localplayer ou si il est sur le serveur etc...
	//il gere du coup tout ce qui touche q l'initialisation d'un joueur.
	//il se désactive au premier "lateupdate" pour pu faire chier aprés. ATTENTION.
	public SpriteRenderer minimapIcon;
	public Color mainPlayerColor;
	public GameObject difficultyPanel;
	// Use this for initialization
	void Start ()
	{
		if (isLocalPlayer) 
		{
			difficultyPanel = GameObject.Find ("DifficultyPanel");
			minimapIcon.color = mainPlayerColor;
			CameraController.instanceCamera.target = gameObject;
			CameraController.instanceCamera.Initialize ();
//			GetComponentInChildren<PlayerEnnemyDetectionScript> ().enabled = true;
			if (!isServer) 
			{
				difficultyPanel.SetActive(false);
			}
		}
		if (isServer) 
		{
			GetComponentInChildren<PlayerEnnemyDetectionScript> ().enabled = true;
		}
	}
	
	// Update is called once per frame
	void LateUpdate () 
	{
		this.enabled = false;
	}
	public override void OnStartLocalPlayer ()
	{
		GameManager.instanceGM.playerObj = gameObject;
		GameManager.instanceGM.ID = gameObject.GetComponent<NetworkIdentity> ().netId;
		base.OnStartLocalPlayer ();
	}

//
//	void OnEnable()
//	{
//		SceneManager.sceneLoaded += OnLevelLoadedOrReloaded;
//	}
//	void OnDisable()
//	{
//		SceneManager.sceneLoaded -= OnLevelLoadedOrReloaded;
//
//	}
//
//	void OnLevelLoadedOrReloaded(Scene scene, LoadSceneMode mode)
//	{
//		Debug.Log ("reloaded");
//	}

}
