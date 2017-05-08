using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class SpellArcherArrowRain : NetworkBehaviour {

	//si cette zone rentre en collision avec un ennemi; il le stop (CC)/ le fait arreter d'attaquer.
	// la variable duration détermine le temps de vie du spell.
	//toutes les 0.5f sec, des DamageOverTime(DoT) sont appliqués.
	//le sort peut etre lvl up, augmentant ainsi un peu tout (voir fonction associé dans la classe PlayerCastCCSpell sur le joueur).


	public GameObject caster;
	public List<GameObject> spellTargets;
	public float duration;
	public float tmpDur; // temps restant sur le sort. pratique pour que les CC durent pas aussi longtemps si le mec entre dans la zone tard.
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
			timer = Time.time; //juste pour m'assurer que ce soit jouer qu'une fois. inutile je crois.
			NetworkServer.Destroy(gameObject);
		}

	}
	[ServerCallback]
	public void LateUpdate()
	{
		if (Time.time > dotTimer + 0.3f)
		{
			dotTimer = dotTimer + 0.3f;
			spellTargets.Clear();
		}
	}
	[ServerCallback]
	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == 9)
		{
			tmpDur = (timer + duration) - Time.time;
			other.gameObject.GetComponent<StatusHandlerScript> ().MakeHimSlow (tmpDur);
		}

	}
	[ServerCallback]
	void OnTriggerStay(Collider other)
	{
		if (!spellTargets.Contains(other.gameObject))
		{
			if (other.gameObject.layer == 9)
			{

				spellTargets.Add(other.gameObject);
				other.gameObject.GetComponent<GenericLifeScript>().LooseHealth((int)spellDamage / 10, true, caster);
			}

		}
	}
}
