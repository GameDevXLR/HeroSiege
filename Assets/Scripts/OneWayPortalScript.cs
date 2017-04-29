using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AI;
public class OneWayPortalScript : NetworkBehaviour {

	// ce script fait que : 
	//si on entre dans sa zone et qu'il est pas isbeingused, il le devient
	//si le joueur dans la zone sort de la zone, il arrete de l'etre : il faut sortir et rerentrer pour retry.
	//le temps est alors stocker dans timeofactivation.
	//une fois Time.time = timeofactivation + timeofTP : le joueur est tp back a la base, le portal est détruit.
	//juste avant de tp le joueur lance l'effet de particule sur le réseau / il peut encore annuler le TP.
	//il est autodétruit si time.time > timeofLife + timeofCreation(+durée de l'effet particule TP+ petit délai histoire d'etre sur...);
	//il reste tant qu'un joueur l'a pas utilisé
	public AudioClip Teleportation;
	public bool isBeingUsed = true; //je le met en utilisé au début tant que la partie est pas lancée.
	public float timeToTP = 2;
	private float timeOfActivation;
	//	private float timeOfCreation;
	private GameObject childParticuleObj;
	private Vector3 targetOfTP;
	private GameObject targetPlayer;
	private bool isPlayerInTeam1;
	private NetworkInstanceId targetID;
	public ParticleSystem tpEffect;
	public bool isPlayingEffect;
	public string tpdestination;

	[ServerCallback]
	public void OnTriggerEnter(Collider other)
	{
		if (!isBeingUsed) 
		{
			if (other.gameObject.tag == "Player") 
			{
				isBeingUsed = true;
				targetPlayer = other.gameObject;
				TpProcess ();

			}
		}
	}
//	[ServerCallback]
//	public void OnTriggerExit(Collider other)
//	{
//		if (isBeingUsed) 
//		{
//			if (other.gameObject == targetPlayer) 
//			{
//				isBeingUsed = false;
//				targetPlayer = null;
//
//			}
//		}
//	}
	[ServerCallback]
	public void TpProcess ()
	{
		targetID = targetPlayer.GetComponent<NetworkIdentity> ().netId;

		timeOfActivation = Time.time;
		StartCoroutine (TpProcessEnum ());
	}

	[ServerCallback]
	public IEnumerator TpProcessEnum ()
	{
		while (isBeingUsed) 
		{
			if (Time.time > timeOfActivation + (float)timeToTP / 2 && !isPlayingEffect) 
			{
				isPlayingEffect = true;
				RpcPlayEffect ();
				GetComponent<AudioSource> ().PlayOneShot (Teleportation);
			}
			if (Time.time > timeOfActivation + timeToTP) 
			{
				isBeingUsed = false;
				if (GameManager.instanceGM.team1ID.Contains (targetID)) 
				{
					isPlayerInTeam1 = true;
				}
				RpcTpThatPlayer (targetID, isPlayerInTeam1);
				targetPlayer = null;
//				StartCoroutine (destroyThatTP ());
			}
			yield return new WaitForSeconds (0.1f);
		}
		isPlayingEffect = false;
		tpEffect.Stop(true);

	}
	[ClientRpc]
	public void RpcTpThatPlayer(NetworkInstanceId id, bool isTeam1)
	{
		StartCoroutine(TPThatPlayerProcedure(id, isTeam1));
	}

	IEnumerator TPThatPlayerProcedure(NetworkInstanceId id, bool isTeam1)
	{
		GameObject playerToTP;
		playerToTP = ClientScene.FindLocalObject (id) ;
		yield return new WaitForEndOfFrame ();
		playerToTP.GetComponent<NavMeshAgent> ().ResetPath ();
		playerToTP.GetComponent<NavMeshAgent> ().enabled = false;
		playerToTP.transform.localPosition = GameObject.Find(tpdestination).transform.position;
		playerToTP.transform.GetChild (3).GetComponent<ParticleSystem> ().Play (true);	

		yield return new WaitForEndOfFrame ();
		playerToTP.GetComponent<NavMeshAgent> ().enabled = true;

	}

	[ClientRpc]
	public void RpcPlayEffect()
	{
		tpEffect.Play (true);

	}

//	IEnumerator destroyThatTP ()
//	{
//		yield return new WaitForSeconds (1.5f);
//		NetworkServer.Destroy (gameObject);
//	}
}
