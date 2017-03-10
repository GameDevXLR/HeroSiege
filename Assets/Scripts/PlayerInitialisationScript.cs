using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

public class PlayerInitialisationScript : NetworkBehaviour {
	public SpriteRenderer minimapIcon;
	public Color mainPlayerColor;
	// Use this for initialization
	void Start ()
	{
		if (isLocalPlayer) {
			minimapIcon.color = mainPlayerColor;
			CameraController.instanceCamera.target = gameObject;
			CameraController.instanceCamera.Initialize ();
//			GetComponent<PlayerXPScript> ().enabled = true;
//			GetComponent<PlayerGoldScript> ().enabled = true;
//			GetComponent<PlayerClicToMove> ().enabled = true;
//			GetComponent<NavMeshAgent> ().enabled = true;
			GetComponentInChildren<PlayerEnnemyDetectionScript> ().enabled = true;
		} 
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public override void OnStartLocalPlayer ()
	{
		GameManager.instanceGM.playerObj = gameObject;
		GameManager.instanceGM.ID = gameObject.GetComponent<NetworkIdentity> ().netId;
		base.OnStartLocalPlayer ();
	}
}
