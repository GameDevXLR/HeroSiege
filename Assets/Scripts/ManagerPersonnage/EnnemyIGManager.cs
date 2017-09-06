using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;
using UnityEngine.UI;
using cakeslice;

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

	//my enemies
	public List<GameObject> myEnemies;
	public cakeslice.Outline outlinemob;
	[Header("Enemies abilities")]
	public bool isSlowingOnAutoA;
	public bool isCCOnAutoA;
	public bool isDrainingManaOnAutoA;
	public bool isCastingAoeCC;
	public bool isAbleToResurect;
	public bool IsAbleToIgnoreAggro;
	[SyncVar]public bool isAnInvisible;


    new void Start()
    {
        base.Start();
        deadAnimChildMesh = transform.GetChild(2).GetChild(0).gameObject;
		if (isAnInvisible) 
		{
			transform.Find ("MiniMapIcon").GetComponent<SpriteRenderer> ().enabled = false;

			deadAnimChildMesh.GetComponentInChildren<SkinnedMeshRenderer> ().enabled = false;
		}
        if (isServer) 
		{
			nbrOfPlayersT1 = GameManager.instanceGM.team1ID.Count;
			nbrOfPlayersT2 = GameManager.instanceGM.team2ID.Count;

        }
    }

	public override void LooseHealth (int dmg, bool trueDmg, GameObject attacker)
	{
		base.LooseHealth (dmg, trueDmg, attacker);
		if (!myEnemies.Contains (attacker)) 
		{
			myEnemies.Add (attacker);
		}
	}

    protected override void LooseHeathServer(int dmg, bool trueDmg, GameObject attacker)
    {

		if (attacker != guyAttackingMe || guyAttackingMe == null)
		{
			if (Vector3.Distance (transform.position, attacker.transform.position) < GetComponent<EnemyAutoAttackScript> ().attackRange*3) {
				guyAttackingMe = attacker;
			}
		}

		if (currentHp > 0)
		{
			takeDommage(dmg, trueDmg);
		}
        if ((attacker.tag == "Player" && !attacker.GetComponent<PlayerIGManager>().isDead)
            ||(attacker.tag != "Player" && !attacker.GetComponent<EnnemyIGManager>().isDead))
        {
            if (!isTaunt)
            {
                if (attacker != GetComponent<EnemyAutoAttackScript>().target)
                {
                    if (Random.Range(0, 3) != 0)  //3 est exclusif car c'est un int.
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

    public override void takeDommage(int dmg, bool trueDmg)
    {
        int hpBefore = currentHp;
        base.takeDommage(dmg, trueDmg);
        guyAttackingMe.GetComponent<EventMessageServer>().receiveDegat(gameObject, guyAttackingMe,currentHp - hpBefore);
    }

    /// <summary>
    ///  Cette fonction n'est appellé que sur le serveur
    /// </summary>
    public override void MakeHimDie()
    {
		if (isAbleToResurect) 
		{
			currentHp = maxHp;
			RpcMakeHimRez ();
			isDead = false;
			isAbleToResurect = false;
			return;
		}
        StartCoroutine(KillTheMob());
    }


    /// <summary>
    /// ce qu'il se passe si un mob meurt...
    /// Fonction qui ne doit s'effectuer que sur le serveur
    /// </summary>
    IEnumerator KillTheMob()
    {
        if (guyAttackingMe)
        {
            if (guyAttackingMe.tag == "Player")
            {
                guyAttackingMe.GetComponent<PlayerXPScript>().GetXP(xpGiven/10);
                //guyAttackingMe.GetComponent<PlayerGoldScript>().GetGold(goldGiven/10);
				ShareXPWithTheTeam (guyAttackingMe.GetComponent<PlayerXPScript> ().isTeam1, xpGiven);
                GameManager.instanceGM.playerObj.GetComponent<PlayerGoldScript>().receiveGoldFromEnnemy(this,guyAttackingMe);
            }
        }
        else
        {
            GameManager.instanceGM.playerObj.GetComponent<PlayerGoldScript>().receiveGoldFromEnnemy(this);
        }
        //		Anim.SetBool ("isDead", true); pour lancer l'anim mort.
        if (isServer)
        {
			int y = goldGiven;
			if (myEnemies.Count > 0) 
			{
				 y = goldGiven / myEnemies.Count;
			}
			for (int i = 0; i < myEnemies.Count; i++) 
			{
				myEnemies [i].GetComponent<PlayerGoldScript> ().GetGold (y);
				if (i == myEnemies.Count - 1) 
				{
					myEnemies.Clear ();
				}
			}

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
		GetComponent<StatusHandlerScript> ().CCTwistImg.enabled = false;
		GetComponent<StatusHandlerScript> ().SlowImg.enabled = false;
        //goldDisplay.text = goldGiven.ToString();
        //goldCanvas.GetComponent<Animator>().enabled = true;
        //goldCanvas.GetComponent<Canvas>().enabled = true;
        //goldCanvas.GetComponent<DeathByTime>().enabled = true;
        //		goldCanvas.GetComponent<RectTransform> ().SetParent (null, false);
		if (deadAnimChildMesh) {
			
			deadAnimChildMesh.GetComponent<Animator> ().enabled = true;
			deadAnimChildMesh.GetComponent<Animator> ().SetBool ("isDead", true);
			deadAnimChildMesh.GetComponent<DeathByTime> ().enabled = true;

			deadAnimChildMesh.transform.parent = null;
		}
    }



    [ClientRpc]
    public void RpcKillTheJungleMob()
    {
		GetComponent<StatusHandlerScript> ().CCTwistImg.enabled = false;
		GetComponent<StatusHandlerScript> ().SlowImg.enabled = false;

        GetComponent<NavMeshObstacle>().enabled = false;
        GetComponent<NavMeshAgent>().enabled = false;
        

        //goldDisplay.text = goldGiven.ToString();
        //goldCanvas.GetComponent<Animator>().enabled = true;
        //goldCanvas.GetComponent<Canvas>().enabled = true;
        //goldCanvas.GetComponent<InactivateAnimatorCanvas>().inactiveWithTime();


        //  goldCanvas.GetComponent<RectTransform> ().SetParent (null, false);
		if (deadAnimChildMesh) {
			deadAnimChildMesh.GetComponent<Animator> ().enabled = true;
			deadAnimChildMesh.GetComponent<Animator> ().SetBool ("isDead", true);
			deadAnimChildMesh.GetComponent<InactivateByTime> ().InactivateWithlifeTime ();
		}
		GetComponent<EnemyAutoAttackScript>().target = null;
        GetComponent<NavMeshAgent>().acceleration = 0;
        GetComponent<NavMeshAgent>().velocity = Vector3.zero;
		StartCoroutine (MoveOutOfTheWay ());
        GetComponent<InactivateByTime>().InactivateWithlifeTime();
        guyAttackingMe = null;
    }

	IEnumerator MoveOutOfTheWay()
	{
		yield return new WaitForSeconds (2f);
		transform.position = Vector3.zero;

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

	[ClientRpc]
	public void RpcMakeHimRez()
	{
		StartCoroutine(rezProcedure());
	}

	IEnumerator rezProcedure()
	{
		transform.Find ("RezParticle").GetComponent<ParticleSystem> ().Play ();
		yield return new WaitForSeconds (2f);
		transform.Find ("RezParticle").GetComponent<ParticleSystem> ().Stop();

	}
}
