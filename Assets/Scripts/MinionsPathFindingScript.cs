using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

public class MinionsPathFindingScript : NetworkBehaviour 
{
	bool walkAnim;
//	private Animator anim;
	[SyncVar] public bool isTeam1;
	public NavMeshAgent agent;
//	[SyncVar] public Transform target;
	[SyncVar]public Transform target;
//	public float stopTime = 2f;
	// Use this for initialization
	void Start () 
	{
//		anim = GetComponentInChildren<Animator> ();
		agent = GetComponent<NavMeshAgent> ();
//		if (isTeam1) 
//		{
//			target = GameObject.Find ("EndPointForMobs").transform;
//		} else 
//		{
//			target = GameObject.Find ("EndPointForMobsTeam2").transform;
//
//		}		
		GoToEndGame ();
	}
	
	public void GoToEndGame()
	{
//		if (!isServer) 
//		{
//			return;
//		}
		StopAllCoroutines ();
		StartCoroutine (GoToEndGameRoutine());
	}

	IEnumerator GoToEndGameRoutine()
	{

			yield return new WaitForSeconds (0.66f);
		if (agent.isOnNavMesh) 
		{
			if (target == null) 
			{
				target = transform;

			}
			agent.SetDestination (target.position);
			agent.isStopped = false;
		}
	}
}
