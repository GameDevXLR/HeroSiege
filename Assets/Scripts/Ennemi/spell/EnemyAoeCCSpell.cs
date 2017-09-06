using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EnemyAoeCCSpell : NetworkBehaviour {

	//si un joueur entre dans cette zone, il est CC pour la durée donnée.
	//mettre le duration = temps de l'effet de particule.
	//j'ai fais en sorte que le CC fonctionne qu'une demi seconde dans tous les cas pour pas prendre
	//en compte des joueurs qui entre tard dans la zone.

	public GameObject caster;
	public List<GameObject> spellTargets;
	public float duration;
	public int spellDamage = 50;
	private float timer;

	void Start()
	{
		timer = Time.time;
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
	void OnTriggerEnter(Collider other)
	{
		if (Time.time > timer + 0.5f) 
		{
			return;
		}
		if (!spellTargets.Contains (other.gameObject)) 
		{
			if (other.gameObject.layer == 8) 
			{
				if (other.tag == "Pet") 
				{
					return;
				}
				other.gameObject.GetComponent<PlayerStatusHandler> ().MakeHimCC (duration);
			}
		}

	}

}
