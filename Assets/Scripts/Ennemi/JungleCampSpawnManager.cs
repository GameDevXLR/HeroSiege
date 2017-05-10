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
    [SyncVar] public int CampLvl = 0; // number of days...
	public int scaleFactor = 50;
    public bool alreadySpawn;


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
        
        jungCampMinion.Clear();
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
        reActivateMob(minion, tr);
        minion = Instantiate (minionPrefab, tr.position, tr.rotation) as GameObject;
		minion.GetComponent<GenericLifeScript> ().maxHp += CampLvl * scaleFactor* GameManager.instanceGM.gameDifficulty;
		minion.GetComponent<GenericLifeScript> ().currentHp = minion.GetComponent<GenericLifeScript>().maxHp;
		minion.GetComponent<GenericLifeScript> ().goldGiven += scaleFactor* CampLvl * 20 / 100;
        minion.GetComponent<EnemyAutoAttackScript>().damage += scaleFactor * CampLvl * 30 / 100;
		minion.GetComponent<GenericLifeScript> ().xpGiven = (CampLvl * scaleFactor*5)-scaleFactor;
          
		minion.GetComponent<MinionsPathFindingScript> ().target = tr;
	}

    [ClientRpc]
    public void RpcUpdateBoss(NetworkInstanceId id, int index)
    {
        CampLvl++;

        GameObject minionboss = ClientScene.FindLocalObject(id);

        Transform tr = bossPos[index];
        reActivateMob(minionboss, tr);
        minionboss.GetComponent<GenericLifeScript>().maxHp += CampLvl * scaleFactor * GameManager.instanceGM.gameDifficulty;
        minionboss.GetComponent<GenericLifeScript>().currentHp = minionboss.GetComponent<GenericLifeScript>().maxHp;
        minionboss.GetComponent<GenericLifeScript>().goldGiven += scaleFactor * CampLvl * 50 / 100;
        minionboss.transform.GetChild(3).transform.localScale = new Vector3(2f, 2f, 2f);
        minionboss.GetComponent<MinionsPathFindingScript>().target = tr;
        minionboss.GetComponent<EnemyAutoAttackScript>().damage += scaleFactor * CampLvl * 60 / 100;
        minionboss.GetComponent<GenericLifeScript>().xpGiven = (CampLvl * scaleFactor * 10) - scaleFactor;

    }

    public void reActivateMob(GameObject mob, Transform transformMob)
    {
        mob.SetActive(true);
        
        if (mob.GetComponent<GenericLifeScript>().mobDeadAnimChildMesh)
        {
            mob.GetComponent<GenericLifeScript>().mobDeadAnimChildMesh.SetActive(true);
            if (mob.GetComponent<GenericLifeScript>().isDead)
            {
                mob.GetComponent<GenericLifeScript>().mobDeadAnimChildMesh.GetComponent<Animator>().SetBool("isDead", false);
                mob.GetComponent<GenericLifeScript>().isDead = false;
            }
        }

        mob.GetComponent<NavMeshAgent>().enabled = true;
        if (mob.GetComponent<NavMeshAgent>().isActiveAndEnabled)
        {
            mob.GetComponent<NavMeshAgent>().SetDestination(transformMob.position);
        }
    }

}
