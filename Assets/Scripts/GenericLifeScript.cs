using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.Networking;

[NetworkSettings(channel = 0, sendInterval = 0.3f)]
public class GenericLifeScript : NetworkBehaviour
{

    // ce script sert a gerer la vie de l'objet auquel il est attacher. 
    // il a grand besoin d'etre diviser en un pour mob / un pour joueur / un pour tour ptete meme 1 pour pnj.
    public AudioClip PlayerDeath;
    public AudioClip PlayerSpawn;
    public int xpGiven = 50;
    public int goldGiven = 5;
	public Text goldDisplay;
	public GameObject goldCanvas;
    public int killCount;
    public RectTransform lifeBar; // barre de vie IG

    public RectTransform lifeBarMain; // lifebar de l'interface player.
    public Text playerHPTxt;
    [SyncVar] public int maxHp = 1000;
    [SyncVar(hook = "RescaleTheLifeBarIG")] public int currentHp = 800;
	[SyncVar]public int regenHp;
    public int levelUpBonusHP = 10;

    public GameObject respawnPoint; // placer ici un transform qui correspond a l'endroit ou doit respawn le joueur.
    [SyncVar(hook = "ActualizeArmor")] public int armorScore = 1;
    public Text armorDisplay;
    [SyncVar(hook = "ActualizeDodge")] [Range(0, 100)] public float dodge; //chance d'esquiver entre 0 et 100
    public Text dodgeDisplay;
    public Text respawnTxt;
    [SyncVar(hook = "ActualizeDeadIcon")] public bool isDead;
    [SyncVar(hook = "ActualizePlayerDeaths")] public int playerDeathCount; //nombre de morts du joueur.
    public float respawnTime = 5f;
    private float lastTic;
    public float timeBetweenTic = 1f;
    public GameObject guyAttackingMe;
    public ParticleSystem rezParticule;
//    private Animator Anim;
    public GameObject deadAnimChildEffect;
    public GameObject deadAnimChildMesh;
    public GameObject mobDeadAnimChildMesh;
    private Image playerDeathDisplay;
    public float durationShake = 5;
    public float amountShake = 10;
    [Range(0,100)]
    public int threshold = 25;
    public bool underThreshold = false;

    void Start()
    {
//        Anim = GetComponentInChildren<Animator>();
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
        if (gameObject.layer == Layers.Player)
        {
			if (gameObject.tag == "Player") {
				deadAnimChildMesh = transform.GetChild (5).GetChild (0).gameObject;
				deadAnimChildEffect = transform.GetChild (4).gameObject;
			} else 
			{
				mobDeadAnimChildMesh = transform.GetChild(3).GetChild(0).gameObject;

			}
		}
        if (gameObject.layer == Layers.Ennemies)
        {
			mobDeadAnimChildMesh = transform.GetChild(3).GetChild(0).gameObject;
        }

    }

    void Update()
    {
        if (isDead || currentHp == maxHp)
        {
            lifeBar.GetComponentInParent<Canvas>().enabled = false;
            return;
        }

        if (!isServer)
        {
            return;
        }



        if (Time.time > lastTic)
        {

            lastTic = Time.time + timeBetweenTic;
            RegenYourHP();
        }


        if (currentHp > maxHp)
        {
            currentHp = maxHp;
            lifeBar.GetComponentInParent<Canvas>().enabled = false;
            if (isLocalPlayer)
            {
                playerHPTxt.text = currentHp.ToString() + " / " + maxHp.ToString();
            }
            return;
        }

        if (currentHp < 0)
        {
            currentHp = 0;
        }
        if (currentHp == 0)
        {
            isDead = true;
            MakeHimDie();
        }
       // Debug.Log(((float)currentHp / maxHp) * 100 + "%");
        
        if (underThreshold && ((float)currentHp / maxHp) * 100 > threshold)
        {
            underThreshold = false;
//            Debug.Log( "Update : " + underThreshold);
        }
    }


    public void LooseHealth(int dmg, bool trueDmg, GameObject attacker)
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
			if (gameObject.layer == 9 || gameObject.layer == 8 && gameObject.tag != "Player")
            { //une chance sur 2 de chancer de cible si la personne qui t'attaque n'est pas celle que tu attaques.
                if (!attacker.GetComponent<GenericLifeScript>().isDead)
                {

                    if (attacker != GetComponent<EnemyAutoAttackScript>().target)
                    {
                        if (Random.Range(0, 2) != 0)
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
                        attacker.GetComponent<PlayerManager>().killCount++;
                    }
                    return;
                }
                float y = Random.Range(0, 100);
                if (y > dodge)
                {
					if (armorScore > 0) {
						float multiplicatorArmor = (float)100f / (100f + armorScore);
						currentHp -= (int)Mathf.Abs (dmg * multiplicatorArmor);
						if (gameObject.layer == Layers.Ennemies && currentHp <= 0) {
							if (attacker.tag == "Player") {
								attacker.GetComponent<PlayerManager> ().killCount++;
							}
						}
						return;
					} else {
						currentHp -= dmg;
						if (gameObject.layer == Layers.Ennemies && currentHp <= 0) {
							if (attacker.tag == "Player") {
								attacker.GetComponent<PlayerManager> ().killCount++;
							}
						}
					}
                    
                    

                }
            }

        }

//        StartCoroutine(HitAnimation());
        RescaleTheLifeBarIG(currentHp);
        lifeBar.GetComponentInParent<Canvas>().enabled = true;



    }
    public void RescaleTheLifeBarIG(int life)
    {
        currentHp = life;
        float x = (float)currentHp / maxHp;
        if (x > 1f)
        {
            x = 1f;
        }
       

		if (gameObject.layer == Layers.Player && ((float)currentHp / maxHp) * 100 <= threshold && !underThreshold && isLocalPlayer)
        {
            underThreshold = true;
            Camera.main.GetComponent<CameraShaker>().ShakeCamera(amountShake, durationShake);
        }

        if (currentHp == maxHp)
        {
            lifeBar.GetComponentInParent<Canvas>().enabled = false;
        }
        else
            if (currentHp != maxHp && currentHp != 0)
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
			if (gameObject.layer == Layers.Player && gameObject.tag == "Player")
            {
                GetComponent<PlayerManager>().playerLifeBar.localScale = new Vector3(x, 1f, 1f);
                GetComponent<PlayerManager>().playerLifeTxt.text = currentHp.ToString() + " / " + maxHp.ToString();
            }
        }
    }
    public void RegenYourHP()
    {
        currentHp += regenHp;

        RescaleTheLifeBarIG(currentHp);
    }
    public void MakeHimDie()
    {
        if (gameObject.layer == 8 && gameObject.tag == "Player")
        {
            if (isLocalPlayer)
            {
                //faire ici ce qui se passe pour un joueur qui meurt
            }
            RpcPlayerRespawnProcess();

            return;
        }
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
                //				if (isServer) 
                //				{
                //					NetworkInstanceId id = guyAttackingMe.GetComponent<NetworkIdentity> ().netId;
                ////					GameManager.instanceGM.GiveAKillToHim (id);
                //				}
            }
        }
        //		Anim.SetBool ("isDead", true); pour lancer l'anim mort.
        if (isServer)
        {
            RpcKillTheMob();
            yield return new WaitForSeconds(0.2f);
            NetworkServer.Destroy(gameObject);
            //faire ici la remise dans le pool.

        }

    }
    [ClientRpc]
    public void RpcKillTheMob()
    {
		goldDisplay.text = goldGiven.ToString ();
		goldCanvas.GetComponent<Animator> ().enabled = true;
		goldCanvas.GetComponent<Canvas> ().enabled = true;
		goldCanvas.GetComponent<DeathByTime> ().enabled = true;
//		goldCanvas.GetComponent<RectTransform> ().SetParent (null, false);
        mobDeadAnimChildMesh.GetComponent<Animator>().enabled = true;
        mobDeadAnimChildMesh.GetComponent<Animator>().SetBool("isDead", true);
        mobDeadAnimChildMesh.GetComponent<DeathByTime>().enabled = true;
        mobDeadAnimChildMesh.transform.parent = null;
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
//    IEnumerator HitAnimation()
//    {
//		if (isDead) 
//		{
//			yield return null;
//		}
//        GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
//        yield return new WaitForSeconds(0.05f);
//        if (GetComponentInChildren<SkinnedMeshRenderer>())
//        {
//            GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
//        }
//    }


    public void LevelUp()
    {
        maxHp += levelUpBonusHP;
        currentHp = maxHp;
        respawnTime += 1f;
        lifeBar.localScale = new Vector3(1, 1f, 1f);

        if (isLocalPlayer)
        {
            lifeBarMain.localScale = new Vector3(1, 1f, 1f);
            playerHPTxt.text = currentHp.ToString() + " / " + maxHp.ToString();

        }
    }
    public void ActualizeArmor(int armor)
    {
        armorScore = armor;
        if (isLocalPlayer)
        {
            armorDisplay.text = armorScore.ToString();
        }
    }
    public void ActualizeDodge(float dod)
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
    public void ActualizeDeadIcon(bool isHeDead)
    {
        isDead = isHeDead;
		if (!isLocalPlayer && (gameObject.layer == Layers.Player || gameObject.layer == Layers.IgnoreLayer)) {
			if (gameObject.tag == "Player") {
				GetComponent<PlayerManager> ().deadAvatarImg.enabled = isHeDead;
			}
		}
    }
}
