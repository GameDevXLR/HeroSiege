using System.Collections;
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
    private Transform animParent;


    // shake de la camera
    public float durationShake = 5;
    public float amountShake = 10;
    [Range(0, 100)]
    public int threshold = 25;
    public bool underThreshold = false;

	public string heroChosen;

	public void Awake()
	{
		if (isLocalPlayer) {
			lifeBarMain = GameObject.Find ("PlayerLifeBarMain").GetComponent<RectTransform> ();
		}
	}

    protected override void Start()
    {
        base.Start();
        if (isLocalPlayer)
        {
			lifeBarMain = GameObject.Find("PlayerLifeBarMain").GetComponent<RectTransform>();
            respawnTxt = GameObject.Find("RespawnText").GetComponent<Text>();
            armorDisplay = GameObject.Find("ArmorLog").GetComponent<Text>();
            armorDisplay.text = armorScore.ToString();
            dodgeDisplay = GameObject.Find("DodgeLog").GetComponent<Text>();
            ActualizeDodge(dodge);
            playerHPTxt = GameObject.Find("PlayerHPTxt").GetComponent<Text>();
            playerDeathDisplay = GameObject.Find("PlayerDeadAvatarImg").GetComponent<Image>();
            playerHPTxt.text = currentHp.ToString() + " / " + maxHp.ToString();
        }
       
        deadAnimChildMesh = transform.GetChild(5).GetChild(0).gameObject;
        deadAnimChildEffect = transform.GetChild(4).gameObject;
        
    }

    protected override void Update()
    {
        base.Update();

        if (underThreshold && ((float)currentHp / maxHp) * 100 > threshold)
        {
            underThreshold = false;
        }
    }


    public override void WhenUpdateCurrentSupAtMaxHp()
    {
        base.WhenUpdateCurrentSupAtMaxHp();
        if (isLocalPlayer)
        {
            playerHPTxt.text = currentHp.ToString() + " / " + maxHp.ToString();
        }
    }

    public override void TempoRescaleTheLifeBarIG(int life)
    {

        base.TempoRescaleTheLifeBarIG(life);

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

    public override void takeDommage(int dmg, bool trueDmg, GameObject attacker)
    {
        int hpBefore = currentHp;
        base.takeDommage(dmg, trueDmg, attacker);
        gameObject.GetComponent<EventMessageServer>().receiveDegat(gameObject,currentHp - hpBefore);
    }

    public override void MakeHimDie()
    {
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
            if (GetComponent<PlayerHealerCastInvokePet>())
            { //si t'as un pet 
                if (GetComponent<PlayerHealerCastInvokePet>().actualPet != null)
                {
                    GetComponent<PlayerHealerCastInvokePet>().actualPet.transform.position = Vector3.zero;
                    GetComponent<PlayerHealerCastInvokePet>().actualPet.GetComponent<PetIGManager>().isDead = true;
                    yield return new WaitForSeconds(0.2f);
                    GetComponent<PlayerHealerCastInvokePet>().DestroyThePrevPet(); // détruit le quand tu meurs.
                }
            }

        }
		GetComponent<PlayerStatusHandler> ().CCTwistImg.enabled = false;
		GetComponent<PlayerStatusHandler> ().SlowImg.enabled = false;
        deadAnimChildMesh.GetComponent<Animator>().SetBool("isDead", true);
        GetComponent<AudioSource>().PlayOneShot(PlayerDeath);
        playerDeathCount++;
        respawnTime += playerDeathCount * 2;
        GetComponent<GenericManaScript>().manaBar.GetComponentInParent<Canvas>().enabled = false;
        gameObject.layer = 16; //passe en layer Ignore
        GetComponent<PlayerAutoAttack>().StopAllCoroutines();
        GetComponent<PlayerAutoAttack>().target = null;
		if (GetComponent<PlayerAutoAttack> ().particule) {
			GetComponent<PlayerAutoAttack> ().particule.Stop ();
		}
        GetComponent<PlayerAutoAttack>().enabled = false;
        GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
        animParent = deadAnimChildMesh.transform.parent;

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
		yield return new WaitForSecondsRealtime(respawnTime);
        deadAnimChildMesh.transform.parent = animParent;
        deadAnimChildMesh.transform.localPosition = Vector3.zero;
		if (heroChosen == "Champion") {
			deadAnimChildMesh.transform.localScale = Vector3.one;
		}
        deadAnimChildMesh.GetComponent<Animator>().SetBool("isDead", false);

        deadAnimChildEffect.transform.parent = this.transform;
        deadAnimChildEffect.transform.localPosition = Vector3.zero;
		GetComponent<PlayerClicToMove>().enabled = true;
        GetComponent<NavMeshAgent>().enabled = true;
        gameObject.layer = 8;
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
		GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
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

    public void LevelUp()
    {
        maxHp += levelUpBonusHP + ((maxHp - bonusHp) * 10 / 100);
        armorScore += levelUpBonusArmor;
        currentHp += levelUpBonusHP + ((maxHp - bonusHp) * 10 / 100);
        respawnTime += 1f;
//        lifeBar.localScale = new Vector3(1, 1f, 1f);
//
//        if (isLocalPlayer)
//        {
////            lifeBarMain.localScale = new Vector3(1, 1f, 1f);
////            playerHPTxt.text = currentHp.ToString() + " / " + maxHp.ToString();
//
//        }
    }
    public override  void ActualizeArmorHook(int armor)
    {
        base.ActualizeArmorHook(armor);
        if (isLocalPlayer)
        {
            armorDisplay.text = armorScore.ToString();
        }
    }
    public override void ActualizeDodgeHook(float dod)
    {
    
		base.ActualizeDodgeHook(dod);
        if (isLocalPlayer)
        {
            dodgeDisplay.text = dod.ToString();
        }    
    }
    public void ActualizePlayerDeaths(int dea)
    {
        playerDeathCount = dea;
//        if (isLocalPlayer && playerDeathCount == 1)
//        {
//            GameManager.instanceGM.ShowAGameTip("When you die, you can cast your invoker spells on the enemy team or to help your allies. You can change them in the menu. It's also a good time to spy on the other team.");
//        }
        GetComponent<PlayerManager>().playerDeathsTxt.text = dea.ToString();
    }
    public override void ActualizeDeadIconHook(bool isHeDead)
    {
        isDead = isHeDead;
        if (isLocalPlayer && playerDeathCount == 1 && !isDead)
        {
            GameManager.instanceGM.ShowAGameTip("The time you spent dead depends on the number of time you died before and your hero's level.");
        }

        if (!isLocalPlayer )
        {
                GetComponent<PlayerManager>().deadAvatarImg.enabled = isHeDead;
        }
    }

}
