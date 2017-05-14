using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SpellArcherPoisonTrap : NetworkBehaviour 
{
	
	public GameObject caster;
	public float duration = 10f;
	public float notTrigeredTimer = 30f;
	public int spellDamage = 50;
	[SyncVar]public float exploRadius = 2;
	public bool IsTriggered; // le piege a t il déja exploser ? 
	private float timer;
	private float dotTimer;
	public List<GameObject> spellTargets;
	SphereCollider thisCollider;
	public GameObject childObj;
	public GameObject areaBeforeExplo;

	void Start()
	{
		timer = Time.time;
		dotTimer = Time.time;
		thisCollider = GetComponent<SphereCollider> ();
	}

	void Update()
	{
		if (isServer)
		{
			if (Time.time > timer + notTrigeredTimer)
			{
				timer = Time.time; //juste pour m'assurer que ce soit jouer qu'une fois. inutile je crois.
				NetworkServer.Destroy(gameObject);
			}
		}
	}
	[ServerCallback]
	public void FixedUpdate()
	{
		if (Time.time > dotTimer + 1f)
		{
			dotTimer = dotTimer + 1f;
			spellTargets.Clear();
		}
	}

	[ServerCallback]
	void OnTriggerStay(Collider other)
	{

		if (isServer)
		{
			if (!IsTriggered) 
			{
				if (other.gameObject.layer == 9) 
				{
					IsTriggered = true;
					ActivateTheTrap ();
					notTrigeredTimer = duration; //faire que le temps avant destruction devienne la durée du zone.
				} else 
				{
					return;
				}
			}

			if (!spellTargets.Contains(other.gameObject))
			{
				if (other.gameObject.layer == 9 || other.gameObject.layer == 8)
				{

					other.gameObject.GetComponent<GenericLifeScript>().LooseHealth((int)spellDamage, true, caster);
					spellTargets.Add(other.gameObject);
				}
			}
		}
	}

	public void ActivateTheTrap()
	{
		
		RpcActivateTheTrap();
	}
	[ClientRpc]
	public void RpcActivateTheTrap()
	{
		thisCollider = GetComponent<SphereCollider> ();

		thisCollider.radius = exploRadius;//augmenter la zone du poison
		childObj.SetActive (true); // activer l'effet de particule
		childObj.transform.localScale = new Vector3 (exploRadius,exploRadius,exploRadius);
		areaBeforeExplo.SetActive (false);
		
	}
}
