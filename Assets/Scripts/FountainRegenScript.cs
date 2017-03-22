using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FountainRegenScript : MonoBehaviour {

	// pour que cette fontaine fonctionne : il faut que le script soit mis sur un objet possédant un collider en mode Trigger. Dés qu'un joueur entre dedans il regen.

	public int regenHp = 2;
	public int regenMp = 1;

	void OnTriggerStay(Collider other)
	{
		if (other.gameObject.tag == "Player") //On utilise le tag et plus la layer; comme ca nos pnj peuvent po y regen... a voir si on veut changer ca.
		{
			other.GetComponent<GenericLifeScript> ().currentHp += regenHp;
			other.GetComponent<GenericManaScript> ().currentMp += regenMp;
		}
	}
	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player") //On utilise le tag et plus la layer; comme ca nos pnj peuvent po y regen... a voir si on veut changer ca.
		{
			other.gameObject.transform.GetChild (1).GetComponent<ParticleSystem> ().Play(true);
			other.gameObject.transform.GetChild (2).GetComponent<ParticleSystem> ().Play(true);

		}
	}
	void OnTriggerExit(Collider other)
	{
		if (other.gameObject.tag == "Player") //On utilise le tag et plus la layer; comme ca nos pnj peuvent po y regen... a voir si on veut changer ca.
		{
			other.gameObject.transform.GetChild (1).GetComponent<ParticleSystem> ().Stop(true);
			other.gameObject.transform.GetChild (2).GetComponent<ParticleSystem> ().Stop(true);

		}
	}
}
