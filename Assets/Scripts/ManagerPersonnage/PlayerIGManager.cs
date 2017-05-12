﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;
using UnityEngine.UI;

[NetworkSettings(channel = 0, sendInterval = 0.3f)]
public class PlayerIGManager : CharacterIGManager {

    // Audio
    public AudioClip PlayerDeath;
    public AudioClip PlayerSpawn;
    // kill
    public int killCount;
    // life
    public RectTransform lifeBarMain; // lifebar de l'interface player.
    public Text playerHPTxt;
    //level up
    public int levelUpBonusHP = 10;
    [SyncVar] public int levelUpBonusArmor;
    // mort
    public GameObject respawnPoint; // placer ici un transform qui correspond a l'endroit ou doit respawn le joueur.
    public Text respawnTxt;
    [SyncVar(hook = "ActualizePlayerDeaths")] public int playerDeathCount; //nombre de morts du joueur.
    public float respawnTime = 5f;
    public ParticleSystem rezParticule;
    private Image playerDeathDisplay;
    // stat armor/dodge
    public Text armorDisplay;
    public Text dodgeDisplay;
    
    // animation
    public GameObject deadAnimChildEffect;


    // shake de la camera
    public float durationShake = 5;
    public float amountShake = 10;
    [Range(0, 100)]
    public int threshold = 25;
    public bool underThreshold = false;
    
    new void Start()
    {
        lastTic = 0f;
        if (isLocalPlayer)
        {
            respawnTxt = GameObject.Find("RespawnText").GetComponent<Text>();
            armorDisplay = GameObject.Find("ArmorLog").GetComponent<Text>();
            armorDisplay.text = armorScore.ToString();
            dodgeDisplay = GameObject.Find("DodgeLog").GetComponent<Text>();
            ActualizeDodge(dodge);
            lifeBarMain = GameObject.Find("PlayerLifeBarMain").GetComponent<RectTransform>();
            playerHPTxt = GameObject.Find("PlayerHPTxt").GetComponent<Text>();
            playerDeathDisplay = GameObject.Find("PlayerDeadAvatarImg").GetComponent<Image>();
            playerHPTxt.text = currentHp.ToString() + " / " + maxHp.ToString();
        }
        deadAnimChildMesh = transform.GetChild(5).GetChild(0).gameObject;
        deadAnimChildEffect = transform.GetChild(4).gameObject;
    }

    public new void WhenUpdateCurrentSupAtMaxHp()
    {
        currentHp = maxHp;
        lifeBar.GetComponentInParent<Canvas>().enabled = false;
        if (isLocalPlayer)
        {
            playerHPTxt.text = currentHp.ToString() + " / " + maxHp.ToString();
        }
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
            

            if (currentHp > 0)
            {
                if (trueDmg)
                {
                    currentHp -= dmg;
                }
                else
                {
                    float y = Random.Range(0, 100);
                    if (y > dodge)
                    {
                        if (armorScore <= -100)
                        {
                            currentHp -= dmg * 2 ;
                        }
                        else
                        {
                            float multiplicatorArmor = (float)100f / (100f + armorScore);
                            currentHp -= (int)Mathf.Abs(dmg * multiplicatorArmor);
                        }
                    }
                }
                
            }

        }
        RescaleTheLifeBarIG(currentHp);
        lifeBar.GetComponentInParent<Canvas>().enabled = true;
        
    }


    public new void RescaleTheLifeBarIG(int life)
    {
        currentHp = life;
        float x = (float)currentHp / maxHp;
        if (x > 1f)
        {
            x = 1f;
        }


        if (((float)currentHp / maxHp) * 100 <= threshold && !underThreshold && isLocalPlayer)
        {
            underThreshold = true;
            Camera.main.GetComponent<CameraShaker>().ShakeCamera(amountShake, durationShake);
        }

        if (currentHp == maxHp)
        {
            lifeBar.GetComponentInParent<Canvas>().enabled = false;
        }
        else if (currentHp != maxHp && currentHp != 0)
        {
            lifeBar.GetComponentInParent<Canvas>().enabled = true;

            if (currentHp > maxHp)
            {
                currentHp = maxHp;
                lifeBar.GetComponentInParent<Canvas>().enabled = false;
            }
        }
        lifeBar.localScale = new Vector3(x, 1f, 1f);
        if (isLocalPlayer)
        {
            lifeBarMain.localScale = new Vector3(x, 1f, 1f);
            playerHPTxt.text = currentHp.ToString() + " / " + maxHp.ToString();

        }
        else
        {
            GetComponent<PlayerManager>().playerLifeBar.localScale = new Vector3(x, 1f, 1f);
            GetComponent<PlayerManager>().playerLifeTxt.text = currentHp.ToString() + " / " + maxHp.ToString();
        }
    }


    public new void MakeHimDie()
    {
        if (isLocalPlayer)
        {
            //faire ici ce qui se passe pour un joueur qui meurt
        }
        RpcPlayerRespawnProcess();
        
    }



    //ce qu'il se passe si un JOUEUR meurt...
    [ClientRpc]
    public void RpcPlayerRespawnProcess()
    {
        StopAllCoroutines();
        StartCoroutine(RespawnEnum());
        if (isLocalPlayer)
        {
            StartCoroutine(RespawnTimer());
        }
    }

    IEnumerator RespawnEnum()
    {
        //ajouter par ici une anime de mort un de ces 4...
        if (isServer)
        {
            GetComponentInChildren<PlayerEnnemyDetectionScript>().autoTargetting = false;

        }
        deadAnimChildMesh.GetComponent<Animator>().SetBool("isDead", true);
        GetComponent<AudioSource>().PlayOneShot(PlayerDeath);
        playerDeathCount++;
        respawnTime += playerDeathCount * 2;
        GetComponent<GenericManaScript>().manaBar.GetComponentInParent<Canvas>().enabled = false;
        gameObject.layer = 16; //passe en layer Ignore
        GetComponent<PlayerAutoAttack>().StopAllCoroutines();
        GetComponent<PlayerAutoAttack>().target = null;
        GetComponent<PlayerAutoAttack>().enabled = false;
        GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
        deadAnimChildMesh.transform.parent = null;
        GetComponent<PlayerClicToMove>().target = null;
        GetComponent<PlayerClicToMove>().enabled = false;
        GetComponent<PlayerClicToMove>().StopAllCoroutines();
        GetComponent<NavMeshAgent>().SetDestination(respawnPoint.transform.position);
        deadAnimChildEffect.GetComponent<ParticleSystem>().Play(true);
        deadAnimChildEffect.transform.parent = null;
        GetComponent<NavMeshAgent>().enabled = false;
        GetComponent<CapsuleCollider>().enabled = false;
        yield return new WaitForEndOfFrame();
        //		GetComponentInChildren<SkinnedMeshRenderer> ().enabled = false;

        transform.localPosition = respawnPoint.transform.position;

        //		yield return new WaitForSeconds (0.8f);
        //		GetComponentInChildren<SkinnedMeshRenderer> ().enabled = false;
        yield return new WaitForSeconds(respawnTime);
        deadAnimChildMesh.transform.parent = this.transform;
        deadAnimChildMesh.transform.localPosition = new Vector3(0f, -0.7f, 0f); // le mesh est légerement en dessous...allez pigé...a revoir ca !
        deadAnimChildMesh.GetComponent<Animator>().SetBool("isDead", false);

        deadAnimChildEffect.transform.parent = this.transform;
        deadAnimChildEffect.transform.localPosition = Vector3.zero;
        GetComponent<NavMeshAgent>().enabled = true;
        gameObject.layer = 8;
        GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
        GetComponent<PlayerClicToMove>().enabled = true;
        GetComponent<CapsuleCollider>().enabled = true;
        GetComponent<AudioSource>().PlayOneShot(PlayerSpawn);
        if (isServer)
        {
            GetComponentInChildren<PlayerEnnemyDetectionScript>().autoTargetting = true;
            isDead = false;

        }
        rezParticule.Play(true);
        GetComponent<GenericManaScript>().manaBar.GetComponentInParent<Canvas>().enabled = true;
        GetComponent<PlayerAutoAttack>().enabled = true;
        currentHp = maxHp;
        GetComponent<GenericManaScript>().currentMp = GetComponent<GenericManaScript>().maxMp;
    }

    IEnumerator RespawnTimer()
    {
        playerDeathDisplay.enabled = true;
        respawnTxt.enabled = true;
        int z = (int)respawnTime;
        for (int j = 0; j < z; j++)
        {
            yield return new WaitForEndOfFrame();
            int k = z - j;
            respawnTxt.text = k.ToString();
            yield return new WaitForSeconds(1f);
            if (k == 1)
            {
                playerDeathDisplay.enabled = false;
                respawnTxt.enabled = false;
            }
        }
    }


    public void LevelUp()
    {
        maxHp += levelUpBonusHP + ((maxHp - bonusHp) * 5 / 100);
        armorScore += levelUpBonusArmor;
        currentHp += levelUpBonusHP + ((maxHp - bonusHp) * 5 / 100);
        respawnTime += 1f;
        lifeBar.localScale = new Vector3(1, 1f, 1f);

        if (isLocalPlayer)
        {
            lifeBarMain.localScale = new Vector3(1, 1f, 1f);
            playerHPTxt.text = currentHp.ToString() + " / " + maxHp.ToString();

        }
    }
    public new void ActualizeArmor(int armor)
    {
        armorScore = armor;
        if (isLocalPlayer)
        {
            armorDisplay.text = armorScore.ToString();
        }
    }
    public new void ActualizeDodge(float dod)
    {
        dodge = dod;
        if (isLocalPlayer)
        {
            dodgeDisplay.text = dod.ToString();
        }
    }
    public void ActualizePlayerDeaths(int dea)
    {
        playerDeathCount = dea;
        GetComponent<PlayerManager>().playerDeathsTxt.text = dea.ToString();
    }
    public new void ActualizeDeadIcon(bool isHeDead)
    {
        isDead = isHeDead;
        if (!isLocalPlayer)
        {
            GetComponent<PlayerManager>().deadAvatarImg.enabled = isHeDead;
        }
        
    }

}
