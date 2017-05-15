﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;
using UnityEngine.UI;

[NetworkSettings(channel = 0, sendInterval = 0.3f)]
public class EnnemyIGManager : CharacterIGManager
{
    // xp
    public int xpGiven = 50;

    // gold
    public int goldGiven = 5;
    public Text goldDisplay;
    public GameObject goldCanvas;

    public bool isJungleMob;

    //Team

    private int nbrOfPlayersT1;
    private int nbrOfPlayersT2;


    new void Start()
    {
        base.Start();
        deadAnimChildMesh = transform.GetChild(3).GetChild(0).gameObject;
        if (isServer) 
		{
			nbrOfPlayersT1 = GameManager.instanceGM.team1ID.Count;
			nbrOfPlayersT2 = GameManager.instanceGM.team2ID.Count;
		}
    }

    protected override void LooseHeathServer(int dmg, bool trueDmg, GameObject attacker)
    {

        base.LooseHeathServer(dmg, trueDmg, attacker);
        if ((attacker.tag == "Player" && !attacker.GetComponent<PlayerIGManager>().isDead)
            ||(attacker.tag != "Player" && !attacker.GetComponent<EnnemyIGManager>().isDead))
        {
            if (!isTaunt)
            {
                if (attacker != GetComponent<EnemyAutoAttackScript>().target)
                {
                    if (Random.Range(0, 2) != 0)  //2 est exclusif car c'est un int.
                    {
                        GetComponent<EnemyAutoAttackScript>().SetTheTarget(attacker);
                    }
                }
            }
        }
        if (currentHp <= 0)
        {
            attacker.GetComponent<PlayerManager>().killCount++;
        }
    }

    public new void MakeHimDie()
    {
       
        StartCoroutine(KillTheMob());
    }


    //ce qu'il se passe si un mob meurt...
    IEnumerator KillTheMob()
    {
        if (guyAttackingMe)
        {
            if (guyAttackingMe.tag == "Player")
            {
                guyAttackingMe.GetComponent<PlayerXPScript>().GetXP(xpGiven);
                guyAttackingMe.GetComponent<PlayerGoldScript>().GetGold(goldGiven);
            }
        }
        //		Anim.SetBool ("isDead", true); pour lancer l'anim mort.
        if (isServer)
        {
            if (isJungleMob)
            {
                isDead = true;
                RpcKillTheJungleMob();

            }
            else
            {
                RpcKillTheMob();
                yield return new WaitForSeconds(0.1f);
                NetworkServer.Destroy(gameObject);
                //faire ici la remise dans le pool.
            }

        }

    }
    [ClientRpc]
    public void RpcKillTheMob()
    {
        goldDisplay.text = goldGiven.ToString();
        goldCanvas.GetComponent<Animator>().enabled = true;
        goldCanvas.GetComponent<Canvas>().enabled = true;
        goldCanvas.GetComponent<DeathByTime>().enabled = true;
        //		goldCanvas.GetComponent<RectTransform> ().SetParent (null, false);
        deadAnimChildMesh.GetComponent<Animator>().enabled = true;
        deadAnimChildMesh.GetComponent<Animator>().SetBool("isDead", true);
        deadAnimChildMesh.GetComponent<DeathByTime>().enabled = true;
        deadAnimChildMesh.transform.parent = null;
    }



    [ClientRpc]
    public void RpcKillTheJungleMob()
    {
        goldDisplay.text = goldGiven.ToString();
        goldCanvas.GetComponent<Animator>().enabled = true;
        goldCanvas.GetComponent<Canvas>().enabled = true;
        goldCanvas.GetComponent<InactivateByTime>().InactivateWithlifeTime();
        //  goldCanvas.GetComponent<RectTransform> ().SetParent (null, false);
        deadAnimChildMesh.GetComponent<Animator>().enabled = true;
        deadAnimChildMesh.GetComponent<Animator>().SetBool("isDead", true);
        deadAnimChildMesh.GetComponent<InactivateByTime>().InactivateWithlifeTime();
        GetComponent<EnemyAutoAttackScript>().target = null;
        GetComponent<NavMeshAgent>().acceleration = 0;
        guyAttackingMe = null;
    }

    public void ShareXPWithTheTeam(bool isT1, int xpToShare)
    {
        if (isT1)
        {
            foreach (NetworkInstanceId id in GameManager.instanceGM.team1ID)
            {
                GameObject go = ClientScene.FindLocalObject(id);
                go.GetComponent<PlayerXPScript>().GetXP(xpToShare / nbrOfPlayersT1);
            }
        }
        else
        {
            foreach (NetworkInstanceId id in GameManager.instanceGM.team2ID)
            {
                GameObject go = ClientScene.FindLocalObject(id);
                go.GetComponent<PlayerXPScript>().GetXP(xpToShare / nbrOfPlayersT2);
            }
        }
    }

}
