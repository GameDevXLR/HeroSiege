using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class BossSpawnManager : NetworkBehaviour 
{

	public GameObject bossPrefab;
	public Transform[] bossSpawns;
	[SyncVar(hook = "ActuBossLvlT1")]public int bossLvlT1;
	[SyncVar(hook = "ActuBossLvlT2")]public int bossLvlT2;
	public Text T1LvlDisplay;
	public Text T2LvlDisplay;
	public Transform targetTeam1Destination;
	public Transform targetTeam2Destination;


	// Use this for initialization
	void Start () 
	{
		T1LvlDisplay = GameObject.Find ("BossLvlT1Display").GetComponent<Text> ();
		T2LvlDisplay = GameObject.Find ("BossLvlT2Display").GetComponent<Text> ();

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
	public void ActuBossLvlT1(int lvl)
	{
		bossLvlT1 = lvl;
		T1LvlDisplay.text = lvl.ToString ();
	}
	public void ActuBossLvlT2(int lvl)
	{
		bossLvlT2 = lvl;
		T2LvlDisplay.text = lvl.ToString ();
	}
}
