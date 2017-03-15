using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.Networking;

[NetworkSettings(channel = 0, sendInterval = 0.3f)]
public class GenericLifeScript : NetworkBehaviour {

	// ce script sert a gerer la vie de l'objet auquel il est attacher. en cas de mort; l'objet est détruit sauf si c'est un joueur : dans ce cas faut écrire le code pour le moment c'est pas préciser...
	public int xpGiven = 50;
	public int goldGiven = 5;

	public RectTransform lifeBar;
	public RectTransform lifeBarMain; // lifebar de l'interface player.
	public Text playerHPTxt;
	[SyncVar]public int maxHp = 1000;
	[SyncVar(hook="RescaleTheLifeBarIG")]public int currentHp = 800;
	public int regenHp;
	public int levelUpBonusHP = 10;

	public GameObject respawnPoint; // placer ici un transform qui correspond a l'endroit ou doit respawn l'objet.

	public int armorScore = 1;
	public Text armorDisplay;
	[Range(0,100)]public float dodge; //chance d'esquiver entre 0 et 100

	public float respawnTime = 5f;
	public Text respawnTxt;
	public bool isDead;
	private float lastTic;
	public float timeBetweenTic = 1f;
	public GameObject guyAttackingMe;
	void Start () 
	{
		lastTic = 0f;
		if (isLocalPlayer) 
		{
			respawnPoint = GameObject.Find ("PlayerRespawnPoint");
			respawnTxt = GameObject.Find ("RespawnText").GetComponent<Text> ();
			armorDisplay = GameObject.Find ("ArmorLog").GetComponent<Text> ();
			armorDisplay.text = armorScore.ToString();
			lifeBarMain = GameObject.Find ("PlayerLifeBarMain").GetComponent<RectTransform> ();
			playerHPTxt = GameObject.Find ("PlayerHPTxt").GetComponent<Text> ();
			playerHPTxt.text = currentHp.ToString () + " / " + maxHp.ToString ();
		}
	}

	void Update () {
		if (isDead || currentHp == maxHp) 
		{
			lifeBar.GetComponentInParent<Canvas> ().enabled = false;

			return;
		}

		if (!isServer) 
		{
			return;
		}



			if (Time.time > lastTic) 
			{
			
				lastTic = Time.time + timeBetweenTic;
				RegenYourHP ();
			}


			if (currentHp > maxHp) 
			{
				currentHp = maxHp;
			lifeBar.GetComponentInParent<Canvas> ().enabled = false;
			if (isLocalPlayer) 
			{
				playerHPTxt.text = currentHp.ToString () + " / " + maxHp.ToString ();
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
				MakeHimDie ();
			}
		}


	public void LooseHealth(int dmg, bool trueDmg, GameObject attacker)
	{	
		if (isServer) 
		{
			
		
		if (attacker != guyAttackingMe || guyAttackingMe == null) 
		{
			guyAttackingMe = attacker;
		}
		if (attacker != GetComponent<AutoAttackScript> ().target && gameObject.layer ==9) //une chance sur 2 de chancer de cible si la personne qui t'attaque n'est pas celle que tu attaques.
		{
			if (Random.Range (0, 2) != 0) //2 est exclusif car c'est un int.
			{
				GetComponent<AutoAttackScript> ().target = attacker;
			}
		}
//		if (gameObject.layer == 8 && GetComponent<AutoAttackScript>().target == null) 
//		{
//			GetComponentInChildren<PlayerEnnemyDetectionScript> ().autoTargetting = true;
//
//		}
			if (currentHp > 0) 
			{
				if (trueDmg) 
				{
					currentHp -= dmg;
					return;
				}
				float y = Random.Range (0, 100);
				if (y > dodge) 
				{
					if (armorScore > 0) 
					{
						float multiplicatorArmor = (float)100f / (100f + armorScore);
						currentHp -= (int)Mathf.Abs (dmg * multiplicatorArmor);
						return;
					} else 
					{
						currentHp -= dmg;
					}
				}
			}
		
		}

			StartCoroutine (HitAnimation ());
			RescaleTheLifeBarIG (currentHp);
			lifeBar.GetComponentInParent<Canvas> ().enabled = true;

		

	}
	public void RescaleTheLifeBarIG(int life)
	{
		currentHp = life;
		float x = (float) currentHp/maxHp;
		if (currentHp != maxHp) 
		{
			lifeBar.GetComponentInParent<Canvas> ().enabled = true;

			lifeBar.localScale = new Vector3 (x, 1f, 1f);
			if (isLocalPlayer) 
			{
				lifeBarMain.localScale = new Vector3 (x, 1f, 1f);
				playerHPTxt.text = currentHp.ToString () + " / " + maxHp.ToString ();

			}
		}
	}
	public void	RegenYourHP ()
	{
		currentHp += regenHp;

		RescaleTheLifeBarIG (currentHp);
	}
	public void MakeHimDie ()
	{
		if (gameObject.layer == 8 && gameObject.tag == "Player") 
		{
			if (isLocalPlayer) 
			{
				//faire ici ce qui se passe pour un joueur qui meurt
			}
			RpcPlayerRespawnProcess ();

			return;
		}
		StartCoroutine (KillTheMob());
	}



	//ce qu'il se passe si un mob meurt...
	IEnumerator KillTheMob()
	{
		if (guyAttackingMe) 
		{
			if(guyAttackingMe.tag == "Player")
			{
//			if (guyAttackingMe == GameManager.instanceGM.playerObj) 
//			{
				guyAttackingMe.GetComponent<PlayerXPScript> ().GetXP (xpGiven);
				guyAttackingMe.GetComponent<PlayerGoldScript> ().GetGold (goldGiven);
				//faire ici ce qui se passe si un mob est tué par un joueur.
//			}
			}
		}
		if (isServer) 
		{
			if (gameObject.tag == "PNJ") 
			{
				GameManager.instanceGM.LooseALife ();
			}
			yield return new WaitForEndOfFrame ();

			NetworkServer.Destroy (gameObject);

		}

//		Destroy (gameObject);
	}


	//ce qu'il se passe si un JOUEUR meurt...
	[ClientRpc]
	public void RpcPlayerRespawnProcess()
	{
		StopAllCoroutines ();
		StartCoroutine (RespawnEnum ());
		if (isLocalPlayer) {
			StartCoroutine (RespawnTimer ());
		}
	}
		IEnumerator RespawnEnum()
	{
		//ajouter par ici une anime de mort un de ces 4...
		if(isLocalPlayer)
		{
			GetComponentInChildren<PlayerEnnemyDetectionScript> ().autoTargetting = false;

		}
		GetComponent<AutoAttackScript> ().StopAllCoroutines ();
		GetComponent<AutoAttackScript> ().enabled = false;
		GetComponentInChildren<SkinnedMeshRenderer> ().enabled = false;
		GetComponent<PlayerClicToMove> ().enabled = false;
		GetComponent<PlayerClicToMove> ().StopAllCoroutines ();
		GetComponent<NavMeshAgent> ().enabled = false;
		GetComponent<CapsuleCollider> ().enabled = false;
		yield return new WaitForEndOfFrame ();
		GetComponentInChildren<SkinnedMeshRenderer> ().enabled = false;

		transform.localPosition = respawnPoint.transform.position;

		yield return new WaitForSeconds (0.8f);

		GetComponentInChildren<SkinnedMeshRenderer> ().enabled = false;
		yield return new WaitForSeconds (respawnTime);
		GetComponent<NavMeshAgent> ().enabled = true;

		GetComponent<NavMeshAgent> ().SetDestination (respawnPoint.transform.localPosition);
		GetComponentInChildren<SkinnedMeshRenderer> ().enabled = true;
		GetComponent<PlayerClicToMove> ().enabled = true;
		GetComponent<CapsuleCollider> ().enabled = true;
		if(isLocalPlayer)
		{
			GetComponentInChildren<PlayerEnnemyDetectionScript> ().autoTargetting = true;

		}
		GetComponent<AutoAttackScript> ().enabled = true;
		currentHp = maxHp;
		isDead = false;
	}

	IEnumerator RespawnTimer()
	{
		respawnTxt.enabled = true;
		int z = (int)respawnTime;
		for (int j = 0; j <z; j++)
		{
			yield return new WaitForEndOfFrame ();
			int k = z - j;
			respawnTxt.text = "Respawning in " + k + " seconds.";
			yield return new WaitForSeconds (1f);
			if (k == 1) 
			{
				respawnTxt.enabled = false;
			}
		}

		
	}
	IEnumerator HitAnimation()
	{
		GetComponentInChildren<SkinnedMeshRenderer> ().enabled = false;
		yield return new WaitForSeconds (0.1f);
		GetComponentInChildren<SkinnedMeshRenderer> ().enabled = true;
	}


	public void LevelUp()
	{
		maxHp += levelUpBonusHP;
		currentHp = maxHp;
		respawnTime += 4f;
		if (isLocalPlayer) 
		{
			lifeBarMain.localScale = new Vector3 (1, 1f, 1f);
			playerHPTxt.text = currentHp.ToString () + " / " + maxHp.ToString ();

		}
	}
}
