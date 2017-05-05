using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SpellCatapulteArea : NetworkBehaviour {



	public GameObject caster; // doit etre completer lors du lancement du sort/de la config du sort sur playerCastXXX
	public List<GameObject> spellTargets;
	bool isDealing;
	public float timeBeforeImpact = 2f; // doit etre plus petit que duration. Cb de temps avant impact ? 
	public float duration; // doit etre plus court que destroyTimer. Combien de temps les 
	// objets entrant subissent les damages ? 
	public int spellDamage = 200;
	private int damageFactor = 1;
	private float timer;
	private float destroyTimer = 8f; //combien de temps l'objet reste au total? temps de l'anim complete en gros.

	void Start()
	{
		timer = Time.time;
	}
	[ServerCallback]
	void Update()
	{
		if (Time.time > timer + timeBeforeImpact && !isDealing) 
		{
			isDealing = true;
			transform.GetChild (0).gameObject.SetActive (false);
			transform.GetChild (1).gameObject.SetActive (true);

			return;
		}
		if (Time.time > timer + destroyTimer)
		{
			timer = Time.time; //juste pour m'assurer que ce soit jouer qu'une fois. inutile je crois.
			NetworkServer.Destroy(gameObject);
		}

	}

	[ServerCallback]
	void OnTriggerStay(Collider other)
	{
		if (!isDealing) 
		{
			return;
		}
		if (Time.time < timer + duration) 
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
