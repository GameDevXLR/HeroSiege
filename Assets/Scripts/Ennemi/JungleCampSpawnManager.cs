using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class JungleCampSpawnManager : NetworkBehaviour 
{
	//gere un camp de mob
	//contient une fonction pour détruire les mobs existants sur le point et les remplacer tous les
	//débuts de journées.
	//contient une liste des mobs du camp

	public Transform[] spawnPos;
	public List<GameObject> jungCampMinion;
	public Transform[] bossPos;
	public GameObject minionPrefab;
	public GameObject minionBossPrefab;
	public int CampLvl = 1; // number of days...
	public int scaleFactor = 20;


	// Use this for initialization
	void Start () 
	{
		if (!isServer) 
		{
			this.enabled = false;
		}
		
	}

	public void ResetThisJungCamp()
	{
		foreach (GameObject go in jungCampMinion) 
		{
			if (go != null) {
				NetworkServer.Destroy (go);
			}
		}

		RespawnTheJungCamp ();
	}

	public void RespawnTheJungCamp ()
	{
		CampLvl++;
		jungCampMinion.Clear ();
		foreach (Transform tr in spawnPos) 
		{
			GameObject minion;
			minion = Instantiate (minionPrefab, tr.position, tr.rotation) as GameObject;
			jungCampMinion.Add (minion);
			minion.GetComponent<GenericLifeScript> ().maxHp += CampLvl * scaleFactor;
			minion.GetComponent<GenericLifeScript> ().currentHp += CampLvl * scaleFactor;
			minion.GetComponent<GenericLifeScript> ().goldGiven += scaleFactor;
			if (CampLvl > 2) 
			{
				minion.GetComponent<GenericLifeScript> ().xpGiven += (CampLvl * scaleFactor)-scaleFactor;
			}
			minion.GetComponent<MinionsPathFindingScript> ().target = tr;

			NetworkServer.Spawn (minion);
		}
		foreach (Transform tr2 in bossPos) 
		{
			GameObject minionboss;
			minionboss = Instantiate (minionBossPrefab, tr2.position, tr2.rotation) as GameObject;
			jungCampMinion.Add (minionboss);
			minionboss.GetComponent<GenericLifeScript> ().maxHp += CampLvl * scaleFactor;
			minionboss.GetComponent<GenericLifeScript> ().currentHp += CampLvl * scaleFactor;
			minionboss.GetComponent<MinionsPathFindingScript> ().target = tr2;
			if (CampLvl > 2) 
			{
				minionboss.GetComponent<EnemyAutoAttackScript> ().damage += CampLvl * 5;
				minionboss.GetComponent<GenericLifeScript> ().xpGiven += (CampLvl * scaleFactor)-scaleFactor;
			}
		
			NetworkServer.Spawn (minionboss);
		}
	}

}
