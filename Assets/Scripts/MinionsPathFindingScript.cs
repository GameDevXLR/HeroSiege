using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

public class MinionsPathFindingScript : NetworkBehaviour 
{
	public NavMeshAgent agent;
	public Transform target; // si ya pas de target; alors il va mettre son point de départ comme target...
//	public float stopTime = 2f;
	// Use this for initialization
	void Start () 
	{
		agent = GetComponent<NavMeshAgent> ();
		if (target == null) 
		{
			target = this.transform;
		}
		GoToEndGame ();
	}
	
	public void GoToEndGame()
	{
		StartCoroutine (GoToEndGameRoutine());
	}

	IEnumerator GoToEndGameRoutine()
	{
		if (agent.isOnNavMesh) 
		{
			yield return new WaitForEndOfFrame ();
			agent.SetDestination (target.position);
			agent.Resume ();
		}
	}
}
