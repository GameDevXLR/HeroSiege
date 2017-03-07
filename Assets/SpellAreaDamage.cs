using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SpellAreaDamage : NetworkBehaviour 
{
	//si cette zone rentre en collision avec un ennemi; il lui inflige ses dégats.
	// la variable duration détermine le temps de vie du spell.


	public GameObject caster;
	public float duration;
	public int spellDamage = 50;
	private float timer;

	void Start()
	{
		timer = Time.time;
	}

	void Update()
	{
		if (isServer) {
			if (Time.time > timer + duration) {
				timer = Time.time; //juste pour m'assurer que ce soit jouer qu'une fois. inutile je crois.
				Network.Destroy (gameObject);
			}
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (isServer) 
		{
			
			if (other.gameObject.layer == 9) 
			{
				other.gameObject.GetComponent<GenericLifeScript> ().LooseHealth (spellDamage, true, caster);			
			}
		}
	}


}
