using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

public class MinionsPathFindingScript : NetworkBehaviour 
{
	[SyncVar]public bool isTeam1;
	public NavMeshAgent agent;
	public Transform target; // si ya pas de target; alors il va mettre son point de départ comme target...
//	public float stopTime = 2f;
	// Use this for initialization
	void Start () 
	{
		agent = GetComponent<NavMeshAgent> ();
		if (isTeam1) 
		{
			target = GameObject.Find ("EndPointForMobs").transform;
		} else 
		{
			target = GameObject.Find ("EndPointForMobsTeam2").transform;

		}		
		GoToEndGame ();
	}
	
	public void GoToEndGame()
	{
		StartCoroutine (GoToEndGameRoutine());
	}

	IEnumerator GoToEndGameRoutine()
	{

			yield return new WaitForEndOfFrame ();
		if (agent.isOnNavMesh) 
		{
			agent.SetDestination (target.position);
			agent.Resume ();
		}
	}
}
