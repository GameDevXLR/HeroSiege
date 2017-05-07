using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SpellHealerUlti : NetworkBehaviour {


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
		if (other.gameObject.layer == 9)
		{
			other.gameObject.GetComponent<StatusHandlerScript>().MakeHimSlow(duration);
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
				other.gameObject.GetComponent<GenericLifeScript>().LooseHealth((int)spellDamage / 5, true, caster);
			}

		}
	}
}
