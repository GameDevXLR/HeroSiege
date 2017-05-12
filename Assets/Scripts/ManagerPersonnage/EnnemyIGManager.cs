using System.Collections;
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


    new void Start()
    {
        lastTic = 0f;
        deadAnimChildMesh = transform.GetChild(3).GetChild(0).gameObject;
    }


    public new void LooseHealth(int dmg, bool trueDmg, GameObject attacker)
    {
        if (isDead)
        {
            return;
        }
        if (isServer)
        {


            if (attacker != guyAttackingMe || guyAttackingMe == null)
            {
                guyAttackingMe = attacker;
            }
            if (!attacker.GetComponent<PlayerIGManager>().isDead)
            {
                if (!isTaunt)
                {
                    if (attacker != GetComponent<EnemyAutoAttackScript>().target)
                    {
                        if (Random.Range(0, 4) != 0)
                        { //2 est exclusif car c'est un int.
                            GetComponent<EnemyAutoAttackScript>().SetTheTarget(attacker);
                        }
                    }
                }
            }
            

            if (currentHp > 0)
            {
                if (trueDmg)
                {
                    currentHp -= dmg;
                    if (gameObject.layer == Layers.Ennemies && currentHp <= 0)
                    {
                        if (attacker.tag == "Player")
                        {
                            attacker.GetComponent<PlayerManager>().killCount++;
                        }
                    }
                }
                else
                {
                    float y = Random.Range(0, 100);
                    if (y > dodge)
                    {
                        if (armorScore <= -100)
                        {
                            currentHp -= dmg * 2;
                            if (currentHp <= 0)
                            {
                                if (attacker.tag == "Player")
                                {
                                    attacker.GetComponent<PlayerManager>().killCount++;
                                }
                            }
                        }
                        else
                        {
                            float multiplicatorArmor = (float)100f / (100f + armorScore);
                            currentHp -= (int)Mathf.Abs(dmg * multiplicatorArmor);
                            if (currentHp <= 0)
                            {
                                if (attacker.tag == "Player")
                                {
                                    attacker.GetComponent<PlayerManager>().killCount++;
                                }
                            }
                            
                        }
                    }
                }               
            }
        }
        RescaleTheLifeBarIG(currentHp);
        lifeBar.GetComponentInParent<Canvas>().enabled = true;
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

}
