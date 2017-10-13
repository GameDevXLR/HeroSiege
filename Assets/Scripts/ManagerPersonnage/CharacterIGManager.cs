using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CharacterIGManager : NetworkBehaviour
{
    //[SyncVar(hook = "RescaleTheLifeBarIG")] public int currentHp = 800;
    //[SyncVar(hook = "ActualizeArmor")] public int armorScore = 1;
    //[SyncVar(hook = "ActualizeDeadIcon")] public bool isDead;
    //[SyncVar(hook = "ActualizeDodge")] [Range(0, 100)] public float dodge;
    // Vie
    public RectTransform lifeBar; // barre de vie IG
	public AudioClip[] painSound;
    [SyncVar] public int maxHp = 1000;
    [SyncVar(hook = "RescaleTheLifeBarIG")] public int currentHp = 800;
    [SyncVar] public int bonusHp;
    [SyncVar] public int regenHp;
    [SyncVar] public int damageReduction;

    // Armure
    [SyncVar(hook = "ActualizeArmor")] public int armorScore = 1;
    [SyncVar] public int bonusArmorScore;

    // Esquive
    [SyncVar(hook = "ActualizeDodge")] [Range(0, 100)] public float dodge;

    // Mort
    [SyncVar(hook = "ActualizeDeadIcon")] public bool isDead;
    public GameObject deadAnimChildMesh; // regroupe mobDeadAnimChildMesh

    // Tic
    protected float lastTic;
    public float timeBetweenTic = 1f;

    // est attaquer par
    public GameObject guyAttackingMe;


    [SyncVar] public bool isTaunt;

    protected virtual void Start()
    {
        lastTic = 0f;

    }

    protected virtual void Update()
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
            WhenUpdateCurrentSupAtMaxHp();
        }
        else
        {
            if (currentHp < 0)
            {
                currentHp = 0;
            }
            if (currentHp == 0)
            {
                isDead = true;
                MakeHimDie();
            }
        }
    }

    public virtual void WhenUpdateCurrentSupAtMaxHp()
    {
        currentHp = maxHp;
        lifeBar.GetComponentInParent<Canvas>().enabled = false;
    }

    public void RegenYourHP()
    {
        currentHp += regenHp;
        RescaleTheLifeBarIG(currentHp);
    }

    public void RescaleTheLifeBarIG(int life)
    {
        TempoRescaleTheLifeBarIG(life);
    }

    public virtual void TempoRescaleTheLifeBarIG(int life)
    {

        currentHp = life;
        float x = (float)currentHp / maxHp;
        if (x > 1f)
        {
            x = 1f;
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
    }

    public virtual void MakeHimDie()
    {
        Debug.Log("Personnage mort");
    }

    public virtual void LooseHealth(int dmg, bool trueDmg, GameObject attacker)
    {
        if (isDead)
        {
            return;
        }
        if (isServer)
        {

            LooseHeathServer(dmg, trueDmg, attacker);
        }
        RescaleTheLifeBarIG(currentHp);
        lifeBar.GetComponentInParent<Canvas>().enabled = true;

    }
    
	protected virtual void LooseHeathServer(int dmg, bool trueDmg, GameObject attacker)
    {
        if (attacker != guyAttackingMe || guyAttackingMe == null)
        {

            guyAttackingMe = attacker;
        }


        if (currentHp > 0)
        {
            takeDommage(dmg, trueDmg, attacker);
        }
    }

    public virtual void takeDommage(int dmg, bool trueDmg, GameObject attacker)
    {
		if (dmg > 1) 
		{
			RpcReactToDamage ();
		}
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
                    currentHp -= dmg * 2;
                }
                else
                {
                    float multiplicatorArmor = (float)100f / (100f + armorScore);
                    int dmgAfterReduct = (dmg / 2) - damageReduction; //on ne peut réduire les damages que jusqu'a 50%
                    if (dmgAfterReduct < 0)
                    {
                        dmgAfterReduct = 0;
                    }
                    dmg = (dmg / 2) + dmgAfterReduct;
                    currentHp -= (int)Mathf.Abs(dmg * multiplicatorArmor);
                }
            }
        }
        if (currentHp < 0)
            currentHp = 0;
    }
	[ClientRpc]
	public void RpcReactToDamage()
	{
		if(painSound.Length>0)
		{
			GetComponent<AudioSource> ().PlayOneShot (painSound [Random.Range (0, painSound.Length)], 0.2f);
			//recu des dégats.
		}
	
	}

    public void ActualizeArmor(int armor)
    {
        ActualizeArmorHook(armor);
    }

    public virtual void ActualizeArmorHook(int armor)
    {
        armorScore = armor;
    }


    public void ActualizeDodge(float dod)
    {
        ActualizeDodgeHook(dod);
    }
    public virtual void ActualizeDodgeHook(float dod)
    {
        dodge = dod;
    }

    public void ActualizeDeadIcon(bool isHeDead)
    {
        ActualizeDeadIconHook(isHeDead);
    }
    public virtual void ActualizeDeadIconHook(bool isHeDead)
    {
        isDead = isHeDead;
    }




    public void GotTauntByFor(GameObject taunter, float timeTaunt)
    {
        GetComponent<EnemyAutoAttackScript>().SetTheTarget(taunter);
        StartCoroutine(GetTaunt(timeTaunt));
    }

    public IEnumerator GetTaunt(float tauntT)
    {
        isTaunt = true;
        yield return new WaitForSeconds(tauntT);
        isTaunt = false;
    }

}
