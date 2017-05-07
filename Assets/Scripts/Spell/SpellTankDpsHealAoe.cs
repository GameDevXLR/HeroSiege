using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SpellTankDpsHealAoe : NetworkBehaviour {

	//si cette zone rentre en collision avec un ennemi; il le stop (CC)/ le fait arreter d'attaquer.
	// la variable duration détermine le temps de vie du spell.
	//toutes les 0.5f sec, des DamageOverTime(DoT) sont appliqués.
	//le sort peut etre lvl up, augmentant ainsi un peu tout (voir fonction associé dans la classe PlayerCastCCSpell sur le joueur).


	public GameObject caster;
	public List<GameObject> spellTargets;
	public float duration;
	public int spellDamage = 50;
	private float timer;
	private float dotTimer;
	private bool hasHealed;

	void Start()
	{
		timer = Time.time;
		dotTimer = Time.time;
	}
	[ServerCallback]
	void Update()
	{
		if (Time.time > timer + 0.7f && !hasHealed) 
		{
			int nbrOfObj;
			nbrOfObj = spellTargets.Count;
			int x = (int)(nbrOfObj * spellDamage / 10);
			caster.GetComponent<GenericLifeScript> ().currentHp +=(spellDamage / 10)* nbrOfObj ;
			Debug.Log(x.ToString());
			hasHealed = true;

		}
		if (Time.time > timer + duration)
		{
			timer = Time.time; //juste pour m'assurer que ce soit jouer qu'une fois. inutile je crois.
			NetworkServer.Destroy(gameObject);
		}

	}
//	[ServerCallback]
//	public void LateUpdate()
//	{
////		if (Time.time > dotTimer + 0.5f)
////		{
////			dotTimer = dotTimer + 0.5f;
////			spellTargets.Clear();
////		}
//	}
	[ServerCallback]
	void OnTriggerEnter(Collider other)
	{
		if (Time.time < timer + 0.5f) 
		{
			if (other.gameObject.layer == 9) 
			{
				if(!spellTargets.Contains(other.gameObject))
					{
						spellTargets.Add (other.gameObject);
					other.gameObject.GetComponent<GenericLifeScript> ().LooseHealth(spellDamage, true, caster);
				}
			}

		}
	}
}
