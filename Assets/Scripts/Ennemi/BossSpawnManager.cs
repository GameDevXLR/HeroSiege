using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BossSpawnManager : NetworkBehaviour 
{

	public GameObject bossPrefab;
	public Transform[] bossSpawns;
	[SyncVar]public int bossLvlT1;
	[SyncVar]public int bossLvlT2;
	public Transform targetTeam1Destination;
	public Transform targetTeam2Destination;


	// Use this for initialization
	void Start () 
	{
		
	}

	public void SpawnBosses()
	{
		SpawnOneBoss (bossSpawns [0], targetTeam1Destination, bossLvlT1);
		SpawnOneBoss (bossSpawns [1], targetTeam1Destination, bossLvlT2);

		if (GetComponent<GameManager> ().soloGame) 
		{
			return;
		}
		SpawnOneBoss (bossSpawns [2], targetTeam2Destination, bossLvlT1);
		SpawnOneBoss (bossSpawns [3], targetTeam2Destination, bossLvlT2);

	}

	public void SpawnOneBoss(Transform tr, Transform targetDest, int bonusFactor)
	{
		GameObject bossTmpObj;
		bossTmpObj = Instantiate (bossPrefab, tr.position, tr.rotation) as GameObject;
		bossTmpObj.GetComponent<GenericLifeScript> ().maxHp += (bonusFactor * bonusFactor * 10) - bonusFactor;
		bossTmpObj.GetComponent<GenericLifeScript> ().currentHp += (bonusFactor * bonusFactor * 10) - bonusFactor;
		bossTmpObj.GetComponent<EnemyAutoAttackScript>().damage += bonusFactor;
		if (bonusFactor >= 10) 
		{
			bossTmpObj.GetComponent<EnemyAutoAttackScript> ().agent.speed += 5;
		}

		bossTmpObj.GetComponent<MinionsPathFindingScript> ().target = targetDest;
		NetworkServer.Spawn (bossTmpObj);
		
	}
}
