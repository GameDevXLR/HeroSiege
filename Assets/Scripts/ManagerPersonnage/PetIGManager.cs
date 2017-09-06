﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PetIGManager : CharacterIGManager {

    public GameObject owner;

    protected override void Start()
    {
        base.Start();

        deadAnimChildMesh = transform.GetChild(3).GetChild(0).gameObject;

    }

    protected override void LooseHeathServer(int dmg, bool trueDmg, GameObject attacker)
    {
       
       base.LooseHeathServer(dmg, trueDmg, attacker);
		if (attacker.layer == Layers.Ennemies && !attacker.GetComponent<EnnemyIGManager>().isDead)
        {
            if (!isTaunt)
            {
                if (attacker != GetComponent<AllyPetAutoAttack>().target)
                {
                    if (Random.Range(0, 10) == 0) 
                    {
                        GetComponent<AllyPetAutoAttack>().SetTheTarget(attacker);
                    }
                }
                
            }
        }
    }

    public override void MakeHimDie()
    {

        StartCoroutine(KillTheMob());
    }


    //ce qu'il se passe si un mob meurt...
    IEnumerator KillTheMob()
    {
 
        //		Anim.SetBool ("isDead", true); pour lancer l'anim mort.
        if (isServer)
        {
            RpcKillTheMob();
            yield return new WaitForSeconds(0.1f);
            NetworkServer.Destroy(gameObject);
        }

    }
    [ClientRpc]
    public void RpcKillTheMob()
    {
        deadAnimChildMesh.GetComponent<Animator>().enabled = true;
        deadAnimChildMesh.GetComponent<Animator>().SetBool("isDead", true);
        deadAnimChildMesh.GetComponent<DeathByTime>().enabled = true;
        deadAnimChildMesh.transform.parent = null;
    }

    //public override void takeDommage(int dmg, bool trueDmg)
    //{
    //    int hpBefore = currentHp;
    //    base.takeDommage(dmg, trueDmg);
    //    gameObject.GetComponent<EventMessageServer>().receiveDegat(gameObject, currentHp - hpBefore);
    //}
}
