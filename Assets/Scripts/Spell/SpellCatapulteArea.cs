using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SpellCatapulteArea : NetworkBehaviour {



	public GameObject caster;
	public List<GameObject> spellTargets;
	public float duration; // doit etre plus court que destroyTimer. Combien de temps les 
	// objets entrant subissent les damages ? 
	public int spellDamage = 200;
	private int damageFactor = 1;
	private float timer;
	private float dotTimer;
	private float destroyTimer = 5f; //combien de temps l'objet reste au total? 

	void Start()
	{
		timer = Time.time;
		dotTimer = Time.time;
	}
	[ServerCallback]
	void Update()
	{
		if (Time.time > timer + destroyTimer)
		{
			timer = Time.time; //juste pour m'assurer que ce soit jouer qu'une fois. inutile je crois.
			NetworkServer.Destroy(gameObject);
		}

	}

	[ServerCallback]
	void OnTriggerEnter(Collider other)
	{
		if (Time.time < Time.time + duration) 
		{
			if (!spellTargets.Contains(other.gameObject))
			{
				if (other.gameObject.layer == 9 || other.gameObject.layer == 8)
				{

					int tmpDmg;
					damageFactor = GameManager.instanceGM.Days;
					tmpDmg = damageFactor * spellDamage;
					
					spellTargets.Add(other.gameObject);
					other.gameObject.GetComponent<GenericLifeScript>().LooseHealth((int)spellDamage, true, caster);
				}
				
			}
			
		}

	}

}
