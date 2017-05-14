using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FountainRegenScript : MonoBehaviour {

	// pour que cette fontaine fonctionne : il faut que le script soit mis sur un objet possédant un collider en mode Trigger. Dés qu'un joueur entre dedans il regen.

	[Range (0,100)]public int regenHp = 2;
	[Range (0,100)]public int regenMp = 1;
	bool isReady = true;

	void OnTriggerStay(Collider other)
	{
		if (!isReady) 
		{
			return;
		}
		if (other.gameObject.tag == "Player") //On utilise le tag et plus la layer; comme ca nos pnj peuvent po y regen... a voir si on veut changer ca.
		{
			int tmpHP = (regenHp * other.GetComponent<GenericLifeScript> ().maxHp) / 100;
			if (tmpHP < 3) 
			{
				tmpHP = 2;
			}
			int tmpMP = (regenHp * other.GetComponent<GenericManaScript> ().maxMp) / 100;
			if (tmpMP < 3) 
			{
				tmpMP = 2;
			}
			other.GetComponent<GenericLifeScript> ().currentHp += tmpHP;
			other.GetComponent<GenericManaScript> ().currentMp += tmpMP;
			StartCoroutine (GetReadyProcedure ());
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

	IEnumerator GetReadyProcedure()
	{
		isReady = false;
		yield return new WaitForSeconds (5f);
		isReady = true;
	}
}
