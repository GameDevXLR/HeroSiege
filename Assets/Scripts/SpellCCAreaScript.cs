using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

	public class SpellCCAreaScript : NetworkBehaviour 
	{
	//si cette zone rentre en collision avec un ennemi; il le stop (CC)/ le fait arreter d'attaquer.
		// la variable duration détermine le temps de vie du spell.
		//toutes les 0.5f sec, des DamageOverTime(DoT) sont appliqués.
		//le sort peut etre lvl up, augmentant ainsi un peu tout (voir fonction associé dans la classe PlayerCastCCSpell sur le joueur).


		public GameObject caster;
		public float duration;
		public int spellDamage = 50;
		private float timer;
		private float dotTimer;

	void Start()
		{
			timer = Time.time;
			dotTimer = Time.time;
		}
	[Server]
	void Update()
	{
		if (Time.time > timer + duration) 
		{
			timer = Time.time; //juste pour m'assurer que ce soit jouer qu'une fois. inutile je crois.
			Network.Destroy (gameObject);
		}
			
	}
	[Server]
	void OnTriggerEnter(Collider other)
	{
	if (other.gameObject.layer == 9) 
		{
			other.gameObject.GetComponent<StatusHandlerScript>().MakeHimCC(duration);			
		}

	}	
	[Server]
	void OnTriggerStay (Collider other)
	{
		if (other.gameObject.layer == 9) 
		{
			if (Time.time > dotTimer + 0.5f) 
			{
				other.gameObject.GetComponent<GenericLifeScript> ().LooseHealth ((int)spellDamage / 5, true, caster);	
				dotTimer = dotTimer + 0.5f;
			}
		}
	}
}
