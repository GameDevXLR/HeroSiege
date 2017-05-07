using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PersonnageIGManager : NetworkBehaviour
{
    // Vie
    public RectTransform lifeBar; // barre de vie IG
    [SyncVar] public int maxHp = 1000;
    [SyncVar(hook = "RescaleTheLifeBarIG")] public int currentHp = 800;
    public int regenHp;

    // Armure
    [SyncVar(hook = "ActualizeArmor")] public int armorScore = 1;

    // Esquive
    [SyncVar(hook = "ActualizeDodge")] [Range(0, 100)] public float dodge;

    // Mort
    [SyncVar(hook = "ActualizeDeadIcon")] public bool isDead;
    public GameObject deadAnimChildMesh; // regroupe mobDeadAnimChildMesh

    // Tic
    private float lastTic;
    public float timeBetweenTic = 1f;

    // est attaquer par
    public GameObject guyAttackingMe;

    private void Start()
    {
        lastTic = 0f;
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
       
    }

    public void RegenYourHP()
    {
        currentHp += regenHp;
        RescaleTheLifeBarIG(currentHp);
    }

    public void RescaleTheLifeBarIG(int life)
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

    public void MakeHimDie()
    {
        Debug.Log("Personnage mort");
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
            if (currentHp > 0)
            {
                if (trueDmg)
                {
                    currentHp -= dmg;
                    return;
                }
                float y = Random.Range(0, 100);
                if (y > dodge)
                {
                    if (armorScore > 0)
                    {
                        float multiplicatorArmor = (float)100f / (100f + armorScore);
                        currentHp -= (int)Mathf.Abs(dmg * multiplicatorArmor);
                        return;
                    }
                    else
                    {
                        currentHp -= dmg;
                    }
                }
            }
        }
        RescaleTheLifeBarIG(currentHp);
        lifeBar.GetComponentInParent<Canvas>().enabled = true;
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
}
