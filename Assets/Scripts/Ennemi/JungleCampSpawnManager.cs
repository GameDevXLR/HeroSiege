using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;


public class JungleCampSpawnManager : NetworkBehaviour 
{
	//gere un camp de mob
	//contient une fonction pour détruire les mobs existants sur le point et les remplacer tous les
	//débuts de journées.
	//contient une liste des mobs du camp


	public Transform[] spawnPos;
    public List<GameObject> jungCampMinion;
    public List<GameObject> jungCampBoss;
    public Transform[] bossPos;
	public GameObject minionPrefab;
	public GameObject minionBossPrefab;
    public int CampLvl = 0; // number of days...
	public int scaleFactor = 50;
    public bool alreadySpawn;
	public GameObject spawnPartEffects;

	void Start() {
	
		spawnPartEffects = transform.GetChild (0).gameObject;
	}

    public void ResetThisJungCamp()
    {
        //foreach (GameObject go in jungCampMinion) 
        //{
        //	if (go != null) {
        //		NetworkServer.Destroy (go);
        //	}
        //}
        

        if (!alreadySpawn)
        {
            
            spawnJungCamp();
            Invoke("updateTheJungCamp", 0.2f);
        }
        else
        {
            updateTheJungCamp();
        }
    }

    public void spawnJungCamp()
    {
        
        foreach (Transform tr in spawnPos)
        {
            GameObject minion;
            minion = Instantiate(minionPrefab, tr.position, tr.rotation) as GameObject;
            jungCampMinion.Add(minion);

            NetworkServer.Spawn(minion);
        }
        foreach (Transform tr2 in bossPos)
        {
            GameObject minionboss;
            minionboss = Instantiate(minionBossPrefab, tr2.position, tr2.rotation) as GameObject;
            jungCampBoss.Add(minionboss);
            NetworkServer.Spawn(minionboss);
        }
        alreadySpawn = true;

    }

    public void updateTheJungCamp()
    {
		RpcSpawnPartEffect ();
        CampLvl++;
        for (int indexMinion = 0; indexMinion < jungCampMinion.Count ;indexMinion++)
        {
            RpcUpdateMinion(jungCampMinion[indexMinion].GetComponent<NetworkIdentity>().netId, indexMinion);
        }

        for (int indexBoss = 0; indexBoss < jungCampBoss.Count; indexBoss++)
        {
            RpcUpdateBoss(jungCampBoss[indexBoss].GetComponent<NetworkIdentity>().netId, indexBoss);
        }
    }

    [ClientRpc]
    public void RpcUpdateMinion(NetworkInstanceId id, int index)
	{
        GameObject minion = ClientScene.FindLocalObject(id);
        Transform tr = spawnPos[index];
		reActivateMob(ClientScene.FindLocalObject(id), tr, minionPrefab);
        minion.GetComponent<EnnemyIGManager>().maxHp = minionPrefab.GetComponent<EnnemyIGManager>().maxHp;

        minion.GetComponent<EnnemyIGManager>().maxHp = CampLvl * scaleFactor*2 * GameManager.instanceGM.gameDifficulty;
		minion.GetComponent<EnnemyIGManager> ().currentHp = minion.GetComponent<EnnemyIGManager>().maxHp;

        minion.GetComponent<EnnemyIGManager>().goldGiven = minionPrefab.GetComponent<EnnemyIGManager>().goldGiven;
        minion.GetComponent<EnnemyIGManager> ().goldGiven += scaleFactor* CampLvl * 20 / 100;

        minion.GetComponent<EnemyAutoAttackScript>().damage = minionPrefab.GetComponent<EnemyAutoAttackScript>().damage;
        minion.GetComponent<EnemyAutoAttackScript>().damage += scaleFactor * CampLvl * 10 / 100;

		minion.GetComponent<EnnemyIGManager> ().xpGiven = (CampLvl * scaleFactor*5)-scaleFactor;
          
		minion.GetComponent<MinionsPathFindingScript> ().target = tr;
	}

    [ClientRpc]
    public void RpcUpdateBoss(NetworkInstanceId id, int index)
    {
        CampLvl++;

        GameObject minionboss = ClientScene.FindLocalObject(id);

        Transform tr = bossPos[index];
		reActivateMob(ClientScene.FindLocalObject(id), tr, minionBossPrefab);

        minionboss.GetComponent<EnnemyIGManager>().maxHp = minionBossPrefab.GetComponent<EnnemyIGManager>().maxHp;
        minionboss.GetComponent<EnnemyIGManager>().maxHp += CampLvl * scaleFactor *4* GameManager.instanceGM.gameDifficulty;
        minionboss.GetComponent<EnnemyIGManager>().currentHp = minionboss.GetComponent<EnnemyIGManager>().maxHp;

        minionboss.GetComponent<EnnemyIGManager>().goldGiven = minionBossPrefab.GetComponent<EnnemyIGManager>().goldGiven;
        minionboss.GetComponent<EnnemyIGManager>().goldGiven += scaleFactor * CampLvl * 50 / 100;
        minionboss.transform.GetChild(3).transform.localScale = new Vector3(2f, 2f, 2f);
        minionboss.GetComponent<MinionsPathFindingScript>().target = tr;

        minionboss.GetComponent<EnemyAutoAttackScript>().damage = minionBossPrefab.GetComponent<EnemyAutoAttackScript>().damage;
        minionboss.GetComponent<EnemyAutoAttackScript>().damage += scaleFactor * CampLvl * 20 / 100;
        minionboss.GetComponent<EnnemyIGManager>().xpGiven = (CampLvl * scaleFactor * 10) - scaleFactor;

    }

    public void reActivateMob(GameObject mob, Transform transformMob, GameObject prefab)
    {
        mob.SetActive(true);
        


        mob.transform.position = transformMob.position;
		mob.transform.rotation = transformMob.rotation;
		
        if (mob.GetComponent<EnnemyIGManager>().deadAnimChildMesh)
        {
            mob.GetComponent<EnnemyIGManager>().deadAnimChildMesh.SetActive(true);
            if (mob.GetComponent<EnnemyIGManager>().isDead)
            {
                mob.GetComponent<EnnemyIGManager>().deadAnimChildMesh.GetComponent<Animator>().SetBool("isDead", false);
				mob.GetComponent<EnnemyIGManager>().deadAnimChildMesh.GetComponent<Animator>().SetBool("attackEnnemi", false);

                mob.GetComponent<EnnemyIGManager>().isDead = false;
            }
        }

        
        mob.GetComponent<NavMeshAgent>().enabled = true;

        mob.GetComponent<NavMeshAgent>().acceleration = prefab.GetComponent<NavMeshAgent>().acceleration;

        if (mob.GetComponent<NavMeshAgent>().isActiveAndEnabled)
        {
            mob.GetComponent<NavMeshAgent>().SetDestination(transformMob.position);
        }
    }

	[ClientRpc]
	public void RpcSpawnPartEffect()
	{
		spawnPartEffects.SetActive (true);
		Invoke("unactivateEffect", 3);
	}

	public void unactivateEffect()
	{
		spawnPartEffects.SetActive (false);

	}

}
