using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

public class MinionsPathFindingScript : NetworkBehaviour {
	public NavMeshAgent agent;
	public Transform target;
	public float stopTime = 2f;
	// Use this for initialization
	void Start () {
		agent = GetComponent<NavMeshAgent> ();
		target = GameObject.Find ("EndPointForMobs").transform;
		GoToEndGame ();
	}
	
	public void GoToEndGame()
	{
		StartCoroutine (GoToEndGameRoutine());
	}

	IEnumerator GoToEndGameRoutine()
	{
		yield return new WaitForEndOfFrame ();
		agent.SetDestination (target.position);
		agent.Resume ();
	}
}
