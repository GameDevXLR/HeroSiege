﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

public class MinionsPathFindingScript : NetworkBehaviour 
{
	bool walkAnim;
	public bool isBoss;
	[SyncVar]public int originalCampNbr; //permet de savoir quel inib a fait spawn ce mob. Chez tous le monde (client et serveur)
//	private Animator anim;
	[SyncVar] public bool isTeam1;
	public NavMeshAgent agent;
//	[SyncVar] public Transform target;
	public Transform target;
	private Vector3 finalDest;
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

		switch (originalCampNbr) 
		{
		//jung mob: ne pas mettre de chiffre, donc zero.
		case 0:
			break;
		//Team1
		case 1:
			agent.SetPath (GameManager.instanceGM.camp1.path);
			break;
		case 2:
			agent.SetPath (GameManager.instanceGM.camp2.path);
			break;
		case 3:
			agent.SetPath (GameManager.instanceGM.camp3.path);

			break;
			//team2
		case 10:
			agent.SetPath (GameManager.instanceGM.camp1B.path);
			break;
		case 20:
			agent.SetPath (GameManager.instanceGM.camp2B.path);
			break;
		case 30:
			agent.SetPath (GameManager.instanceGM.camp3B.path);

			break;
		default:
			Debug.Log (originalCampNbr);
			break;
		}
		if(isBoss){
			GoToEndGame ();
		}
		finalDest = agent.pathEndPosition;
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

			yield return new WaitForSeconds (0.1f);
		if (agent.isOnNavMesh) 
		{
			agent.isStopped = false;
			switch (originalCampNbr) 
			{
			//jung mob: ne pas mettre de chiffre, donc zero.
			case 0:
		agent.SetDestination (target.position);

				break;
				//Team1
			case 1:
				agent.SetPath (GameManager.instanceGM.camp1.path);
				break;
			case 2:
				agent.SetPath (GameManager.instanceGM.camp2.path);
				break;
			case 3:
				agent.SetPath (GameManager.instanceGM.camp3.path);

				break;
				//team2
			case 10:
				agent.SetPath (GameManager.instanceGM.camp1B.path);
				break;
			case 20:
				agent.SetPath (GameManager.instanceGM.camp2B.path);
				break;
			case 30:
				agent.SetPath (GameManager.instanceGM.camp3B.path);

				break;
			default:
				Debug.Log (originalCampNbr);
				break;
			}
			if (agent.pathStatus == NavMeshPathStatus.PathInvalid) 
			{
				agent.SetDestination (finalDest);
			}
//			if (target == null) 
//			{
//				target = transform;
//
//			}
//			agent.SetDestination (target.position);
		}
	}
}
