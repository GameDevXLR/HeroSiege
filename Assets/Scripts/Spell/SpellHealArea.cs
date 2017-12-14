using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SpellHealArea : NetworkBehaviour {
	public GameObject caster;
	public float duration = 2.5f;
	public int healAmount = 50;

	private float timer;
	private float dotTimer;
	public List<GameObject> spellTargets;

	void Start()
	{
		timer = Time.time;
		dotTimer = Time.time;
	}

	void Update()
	{
		if (isServer)
		{
			if (Time.time > timer + duration)
			{
				timer = Time.time; //juste pour m'assurer que ce soit jouer qu'une fois. inutile je crois.
				NetworkServer.Destroy(gameObject);
			}
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
	void OnTriggerStay(Collider other)
	{
			if (!spellTargets.Contains(other.gameObject))
			{
				if (other.gameObject.layer == 8)
				{

					spellTargets.Add(other.gameObject);
                    if (other.gameObject.tag == "Player")
                        other.gameObject.GetComponent<PlayerIGManager>().LooseHealth((int)-healAmount, true, gameObject);
//                    else
//                        other.gameObject.GetComponent<PetIGManager>().LooseHealth((int)-healAmount, true, gameObject);
				}
			}

	}
}
