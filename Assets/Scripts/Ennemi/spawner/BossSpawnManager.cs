using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class BossSpawnManager : NetworkBehaviour 
{

	public GameObject bossPrefab;
	public Transform[] bossSpawns;
    public GameObject[] EffectSpawns;
	[SyncVar(hook = "ActuBossLvlT1")]public int bossLvlT1;
	[SyncVar(hook = "ActuBossLvlT2")]public int bossLvlT2;
	public Text T1LvlDisplay;
	public Text T2LvlDisplay;
	public Transform targetTeam1Destination;
	public Transform targetTeam2Destination;
    private int effectNumber = 4; // nombre d'effect appliqué : 2 pour sologame, 4 sinon


	// Use this for initialization
	void Start () 
	{
		
		T1LvlDisplay = GameObject.Find ("BossLvlT1Display").GetComponent<Text> ();
		T2LvlDisplay = GameObject.Find ("BossLvlT2Display").GetComponent<Text> ();

        if (GetComponent<GameManager>().soloGame)
        {
            effectNumber = 2;
        }

    }

	public void SpawnBosses()
	{
        RpcSpawnEffect();
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
		bossTmpObj.GetComponent<EnnemyIGManager> ().maxHp += (bonusFactor *10*GameManager.instanceGM.gameDifficulty);
		bossTmpObj.GetComponent<EnnemyIGManager> ().currentHp += (bonusFactor *10*GameManager.instanceGM.gameDifficulty);
		bossTmpObj.GetComponent<EnemyAutoAttackScript>().damage += bonusFactor*GameManager.instanceGM.gameDifficulty*3;
		bossTmpObj.GetComponent<EnnemyIGManager> ().goldGiven += bonusFactor * bonusFactor;
		bossTmpObj.GetComponent<EnnemyIGManager> ().xpGiven += bonusFactor * bonusFactor;
		bossTmpObj.GetComponent<EnnemyIGManager> ().isCastingAoeCC = true;
		if (bonusFactor >= 10) 
		{
			bossTmpObj.GetComponent<EnemyAutoAttackScript> ().agent.speed += 5;
		}

		bossTmpObj.GetComponent<MinionsPathFindingScript> ().target = targetDest;
		NetworkServer.Spawn (bossTmpObj);
	}

    [ClientRpc]
    public void RpcSpawnEffect()
    {
        for (int i = 0; i < effectNumber; i++)
        {
            EffectSpawns[i].SetActive(true);
            
        }
        Invoke("unactivateEffect", 3);
    }

    public void unactivateEffect()
    {
        for (int i = 0; i < effectNumber; i++)
        {
            EffectSpawns[i].SetActive(false);
        }
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
