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
	private Transform animParent;
    public RectTransform lifeBarMain; // lifebar de l'interface player.
    public Text playerHPTxt;
    [SyncVar] public int maxHp = 1000;
    [SyncVar(hook = "RescaleTheLifeBarIG")] public int currentHp = 800;
	[SyncVar]public int	bonusHp;
	[SyncVar]public int regenHp;
	[SyncVar]public int bonusArmorScore;
	[SyncVar]public int damageReduction;
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
	[SyncVar] public int levelUpBonusArmor;

	[SyncVar]public bool isTaunt;
    public bool isJungleMob;
	private int nbrOfPlayersT1;
	private int nbrOfPlayersT2;
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
			if (isServer) 
			{
				nbrOfPlayersT1 = GameManager.instanceGM.team1ID.Count;
				nbrOfPlayersT2 = GameManager.instanceGM.team2ID.Count;
			}
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
            { //une chance sur 2 de chancer de cible si la personne qui t'attaque n'est pas celle que tu attaques si t'es pas un joueur.
				if (!attacker.GetComponent<GenericLifeScript> ().isDead) 
				{
					if (!isTaunt) 
					{
						if(gameObject.layer == 8 && gameObject.tag != "Player") // si t'es un pet
						{
							if (attacker != GetComponent<AllyPetAutoAttack> ().target) 
							{
								if (Random.Range (0, 10) == 0)  //2 est exclusif car c'est un int.
								{
									GetComponent<AllyPetAutoAttack> ().SetTheTarget (attacker);
								}
							}
						}else
						if (attacker != GetComponent<EnemyAutoAttackScript> ().target) 
						{
							if (Random.Range (0, 2) != 0)  //2 est exclusif car c'est un int.
							{
								GetComponent<EnemyAutoAttackScript> ().SetTheTarget (attacker);
							}
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
						int dmgAfterReduct = (dmg/2) - damageReduction; //on ne peut réduire les damages que jusqu'a 50%
						if (dmgAfterReduct < 0) 
						{
							dmgAfterReduct = 0;
						}
						dmg = (dmg / 2) + dmgAfterReduct;
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
				ShareXPWithTheTeam (guyAttackingMe.GetComponent<PlayerXPScript> ().isTeam1, xpGiven);
                guyAttackingMe.GetComponent<PlayerXPScript>().GetXP(xpGiven/10);
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
            if (isJungleMob)
            {
                isDead = true;
                RpcKillTheJungleMob();
				if (GetComponent<EnemyAutoAttackScript> ().isAttacking) 
				{
					GetComponent<EnemyAutoAttackScript> ().RpcStopAttacking ();
				}

            }
            else
            {
				isDead = true;
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



    [ClientRpc]
    public void RpcKillTheJungleMob()
    {
        goldDisplay.text = goldGiven.ToString();
        goldCanvas.GetComponent<Animator>().enabled = true;
        goldCanvas.GetComponent<Canvas>().enabled = true;
        goldCanvas.GetComponent<InactivateByTime>().InactivateWithlifeTime();
        //  goldCanvas.GetComponent<RectTransform> ().SetParent (null, false);
        mobDeadAnimChildMesh.GetComponent<Animator>().enabled = true;
        mobDeadAnimChildMesh.GetComponent<Animator>().SetBool("isDead", true);
//        mobDeadAnimChildMesh.GetComponent<InactivateByTime>().InactivateWithlifeTime();
        GetComponent<EnemyAutoAttackScript>().target = null;
        GetComponent<NavMeshAgent>().acceleration = 0;
		GetComponent<NavMeshAgent> ().velocity = Vector3.zero;
        guyAttackingMe = null;
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
			if (GetComponent<PlayerHealerCastInvokePet> ()) { //si t'as un pet 
				if (GetComponent<PlayerHealerCastInvokePet> ().actualPet != null) 
				{
					GetComponent<PlayerHealerCastInvokePet> ().actualPet.transform.position = Vector3.zero;
					GetComponent<PlayerHealerCastInvokePet> ().actualPet.GetComponent<GenericLifeScript> ().isDead = true;
					yield return new WaitForSeconds (0.2f);
					GetComponent<PlayerHealerCastInvokePet> ().DestroyThePrevPet(); // détruit le quand tu meurs.
				}
			}

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
		animParent = deadAnimChildMesh.transform.parent;
		GetComponent<PlayerStatusHandler> ().CCTwistImg.enabled = false;
		GetComponent<PlayerStatusHandler> ().SlowImg.enabled = false;

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
		deadAnimChildMesh.transform.parent = animParent;
        deadAnimChildMesh.transform.localPosition = Vector3.zero; 
		deadAnimChildMesh.transform.localScale = Vector3.one;
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
			yield return new WaitForSecondsRealtime(1f);
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
		maxHp += levelUpBonusHP + ((maxHp-bonusHp)*5/100);
		armorScore += levelUpBonusArmor;
		currentHp += levelUpBonusHP+ ((maxHp-bonusHp)*5/100);
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
		if (isLocalPlayer && playerDeathCount == 1) 
		{
			GameManager.instanceGM.ShowAGameTip ("When you die, you can cast your invoker spells on the enemy team or to help your allies. You can change them in the menu. It's also a good time to spy on the other team.");
			if (PlayerPrefs.GetString ("LANGAGE") == "Fr") 
			{
				GameManager.instanceGM.ShowAGameTip ("Quand vous tombez au combat, vous pouvez utiliser vos sorts d'invoqueur sur la team adverse, ou pour aider vos alliés. Vous pouvez les changés dans votre profil. C'est aussi le moment d'espionner l'équipe adverse.");

			}
		}
        GetComponent<PlayerManager>().playerDeathsTxt.text = dea.ToString();
    }
    public void ActualizeDeadIcon(bool isHeDead)
    {
        isDead = isHeDead;
		if (isLocalPlayer && playerDeathCount == 1 && !isDead) 
		{
			GameManager.instanceGM.ShowAGameTip ("The time you spent dead depends on the number of time you died before and your hero's level.");
			if (PlayerPrefs.GetString ("LANGAGE") == "Fr") 
			{
				GameManager.instanceGM.ShowAGameTip ("Le temps que vous passez mort dépend du nombre de fois ou vous êtes mort précédemment et du niveau de votre héros.");

			}
		}

		if (!isLocalPlayer && (gameObject.layer == Layers.Player || gameObject.layer == Layers.IgnoreLayer)) {
			if (gameObject.tag == "Player") {
				GetComponent<PlayerManager> ().deadAvatarImg.enabled = isHeDead;
			}
		}
    }

	public void GotTauntByFor(GameObject taunter, float timeTaunt)
	{
		GetComponent<EnemyAutoAttackScript> ().SetTheTarget (taunter);
		StartCoroutine (GetTaunt (timeTaunt));
	}

	public IEnumerator GetTaunt(float tauntT)
	{
		isTaunt = true;
		yield return new WaitForSeconds (tauntT);
		isTaunt = false;
	}

	public void ShareXPWithTheTeam(bool isT1, int xpToShare)
	{
		if (isT1) 
		{
			foreach (NetworkInstanceId id in GameManager.instanceGM.team1ID) 
			{
				GameObject go =  ClientScene.FindLocalObject (id);
				go.GetComponent<PlayerXPScript> ().GetXP (xpToShare / nbrOfPlayersT1);
			}
		} 
		else 
		{
			foreach (NetworkInstanceId id in GameManager.instanceGM.team2ID) 
			{
				GameObject go =  ClientScene.FindLocalObject (id);
				go.GetComponent<PlayerXPScript> ().GetXP (xpToShare / nbrOfPlayersT2);
			}
		}
	}
}
