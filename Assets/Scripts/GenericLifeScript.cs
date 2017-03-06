using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GenericLifeScript : NetworkBehaviour {

	// ce script sert a gerer la vie de l'objet auquel il est attacher. en cas de mort; l'objet est détruit sauf si c'est un joueur : dans ce cas faut écrire le code pour le moment c'est pas préciser...
	public int xpGiven = 50;
	public int goldGiven = 5;
	public RectTransform lifeBar;
	public int maxHp = 1000;
	[SyncVar(hook="RescaleTheLifeBarIG")]public int currentHp = 800;
	public int regenHp;
	public int levelUpBonusHP = 10;

	public Transform respawnTransform; // placer ici un transform qui correspond a l'endroit ou doit respawn l'objet.

	public int armorScore = 1;
	[Range(0,100)]public float dodge; //chance d'esquiver entre 0 et 100

	public float respawnTime = 5f;
	public Text respawnTxt;
	public bool isDead;
	private float lastTic;
	public float timeBetweenTic = 1f;
	public GameObject guyAttackingMe;
	void Start () {
		lastTic = 0f;
		respawnTransform = GameObject.Find ("PlayerRespawnPoint").transform;
		respawnTxt = GameObject.Find ("RespawnText").GetComponent<Text> ();
	}

	void Update () {

		if (isDead || currentHp == maxHp) 
		{
			lifeBar.GetComponentInParent<Canvas> ().enabled = false;

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
		if (!isServer) 
		{
			return;
		}
		if (attacker != guyAttackingMe || guyAttackingMe == null) 
		{
			guyAttackingMe = attacker;
		}
		if (attacker != GetComponent<AutoAttackScript> ().target && gameObject.layer ==9) //une chance sur 2 de chancer de cible si la personne qui t'attaque n'est pas celle que tu attaques.
		{
			if (Random.Range (0, 2) != 0) //2 est exclusif car c'est un int. et ouais.
			{
				GetComponent<AutoAttackScript> ().target = attacker;
			}
		}
		float y = Random.Range (0, 100);
		if (y > dodge) {
			StartCoroutine (HitAnimation ());
			RescaleTheLifeBarIG (currentHp);
			lifeBar.GetComponentInParent<Canvas> ().enabled = true;
			if (currentHp > 0) {
				if (trueDmg) {
					currentHp -= dmg;
					return;
				} 
				if (armorScore > 0) {
					float multiplicatorArmor = (float)100f / (100f + armorScore);
					currentHp -= (int)Mathf.Abs (dmg * multiplicatorArmor);
					return;
				} else {
					currentHp -= dmg;
				}
			}
		}

	}
	public void RescaleTheLifeBarIG(int life)
	{
		currentHp = life;
		float x = (float) currentHp/maxHp;
		lifeBar.GetComponentInParent<Canvas> ().enabled = true;

		lifeBar.localScale = new Vector3 (x, 1f, 1f);

	}
	public void	RegenYourHP ()
	{
		currentHp += regenHp;

		RescaleTheLifeBarIG (currentHp);
	}
	public void MakeHimDie ()
	{
		if (gameObject.layer == 8 && isLocalPlayer) 
		{
			//faire ici ce qui se passe pour un joueur qui meurt
			PlayerRespawnProcess();
			return;
		}
		StartCoroutine (KillTheMob());
	}

	//ce qu'il se passe si un mob meurt...
	IEnumerator KillTheMob()
	{
		if (guyAttackingMe) 
		{
//			if (guyAttackingMe == GameManager.instanceGM.playerObj) 
//			{
				guyAttackingMe.GetComponent<PlayerXPScript> ().GetXP (xpGiven);
				guyAttackingMe.GetComponent<PlayerGoldScript> ().GetGold (goldGiven);
				//faire ici ce qui se passe si un mob est tué par un joueur.
//			}
		}
		yield return new WaitForEndOfFrame ();
		if (isServer) 
		{
			NetworkServer.Destroy (gameObject);
		}
//		Destroy (gameObject);
	}


	//ce qu'il se passe si un JOUEUR meurt...
	public void PlayerRespawnProcess()
	{
		StartCoroutine (RespawnEnum ());
		StartCoroutine (RespawnTimer ());
	}
		IEnumerator RespawnEnum()
	{
		//ajouter par ici une anime de mort un de ces 4...
		GetComponentInChildren<PlayerEnnemyDetectionScript> ().autoTargetting = false;
		GetComponent<AutoAttackScript> ().enabled = false;
		GetComponentInChildren<SkinnedMeshRenderer> ().enabled = false;
		GetComponent<PlayerClicToMove> ().enabled = false;
		GetComponent<NavMeshAgent> ().SetDestination (transform.position);
		GetComponent<CapsuleCollider> ().enabled = false;
		yield return new WaitForSeconds (0.8f);
		GetComponentInChildren<SkinnedMeshRenderer> ().enabled = false;
		yield return new WaitForSeconds (respawnTime);
		GetComponent<NavMeshAgent> ().SetDestination (respawnTransform.position);
		GetComponentInChildren<SkinnedMeshRenderer> ().enabled = true;
		GetComponent<PlayerClicToMove> ().enabled = true;
		GetComponent<CapsuleCollider> ().enabled = true;
		GetComponentInChildren<PlayerEnnemyDetectionScript> ().autoTargetting = true;
		GetComponent<AutoAttackScript> ().enabled = true;

		gameObject.transform.position = respawnTransform.position;
		gameObject.transform.rotation = respawnTransform.rotation;
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
	}
}
