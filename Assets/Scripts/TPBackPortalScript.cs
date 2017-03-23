using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AI;

public class TPBackPortalScript : NetworkBehaviour 
{
	// ce script fait que : 
	//si on entre dans sa zone et qu'il est pas isbeingused, il le devient
	//si le joueur dans la zone prend des dégats, il arrete de l'etre : il faut sortir et rerentrer pour retry.
	//le temps est alors stocker dans timeofactivation.
	//une fois Time.time = timeofactivation + timeofTP : le joueur est tp back a la base, le portal est détruit.
	//juste avant de tp le joueur il lance l'effet de particule.
	//il est autodétruit si time.time > timeofLife + timeofCreation(+durée de l'effet particule TP);

	public bool isBeingUsed;
	public float timeToTP = 2;
	private float timeOfActivation;
	private float timeOfCreation;
	private GameObject childParticuleObj;
	private Vector3 targetOfTP;
	private GameObject targetPlayer;
	private bool isPlayerInTeam1;
	private NetworkInstanceId targetID;
	public ParticleSystem tpEffect;
	public bool isPlayingEffect;

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
	[ServerCallback]
	public void OnTriggerExit(Collider other)
	{
		if (isBeingUsed) 
		{
			if (other.gameObject == targetPlayer) 
			{
				isBeingUsed = false;
				targetPlayer = null;

			}
		}
	}
	[ServerCallback]
	public void TpProcess ()
	{
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
			}
			if (Time.time > timeOfActivation + timeToTP) 
			{
				isBeingUsed = false;
				targetID = targetPlayer.GetComponent<NetworkIdentity> ().netId;
				if (GameManager.instanceGM.team1ID.Contains (targetID)) 
				{
					isPlayerInTeam1 = true;
				}
				targetPlayer = null;
				RpcTpThatPlayer (targetID, isPlayerInTeam1);
				StartCoroutine (destroyThatTP ());
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
		if (isTeam1) 
		{
			playerToTP.transform.localPosition = GameObject.Find ("PlayerRespawnPointT1").transform.position;
			playerToTP.transform.GetChild (3).GetComponent<ParticleSystem> ().Play (true);	
		} else 
		{
			playerToTP.transform.localPosition = GameObject.Find ("PlayerRespawnPointT2").transform.position;
			playerToTP.transform.GetChild (3).GetComponent<ParticleSystem> ().Play (true);
		}
		yield return new WaitForEndOfFrame ();
		playerToTP.GetComponent<NavMeshAgent> ().enabled = true;

	}

	[ClientRpc]
	public void RpcPlayEffect()
	{
		tpEffect.Play (true);
	}

	IEnumerator destroyThatTP ()
	{
		yield return new WaitForSeconds (1.5f);
		NetworkServer.Destroy (gameObject);
	}
}
