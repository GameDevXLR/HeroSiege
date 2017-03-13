using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class PlayerInitialisationScript : NetworkBehaviour {
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
//			GetComponent<PlayerXPScript> ().enabled = true;
//			GetComponent<PlayerGoldScript> ().enabled = true;
//			GetComponent<PlayerClicToMove> ().enabled = true;
//			GetComponent<NavMeshAgent> ().enabled = true;
			GetComponentInChildren<PlayerEnnemyDetectionScript> ().enabled = true;
			if (!isServer) 
			{
				difficultyPanel.SetActive(false);
			}
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
