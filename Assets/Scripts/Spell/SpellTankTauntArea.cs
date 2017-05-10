﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class SpellTankTauntArea : NetworkBehaviour {

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

	void Start()
	{
		timer = Time.time;
		dotTimer = Time.time;
	}
	[ServerCallback]
	void Update()
	{
		if (Time.time > timer + duration)
		{
            foreach (GameObject item in spellTargets)
            {
                item.GetComponent<EnemyAutoAttackScript>().damage += spellDamage;
            }
            

            timer = Time.time; //juste pour m'assurer que ce soit jouer qu'une fois. inutile je crois.
			NetworkServer.Destroy(gameObject);
		}

	}
	[ServerCallback]
	public void LateUpdate()
	{
		if (Time.time > dotTimer + 0.5f)
		{
			dotTimer = dotTimer + 0.5f;
			spellTargets.Clear();
		}
	}
	[ServerCallback]
	void OnTriggerEnter(Collider other)
	{
		if (Time.time < timer + 1) {
			if (other.gameObject.layer == 9 && !spellTargets.Contains(other.gameObject)) {
				other.gameObject.GetComponent<GenericLifeScript> ().GotTauntByFor (caster, duration);
                other.gameObject.GetComponent<EnemyAutoAttackScript>().damage -= (spellDamage* other.gameObject.GetComponent<EnemyAutoAttackScript>().damage) / 100;
                spellTargets.Add(other.gameObject);
                    }

		}
	}

}
