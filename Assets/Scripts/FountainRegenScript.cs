using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FountainRegenScript : MonoBehaviour {

	// pour que cette fontaine fonctionne : il faut que le script soit mis sur un objet possédant un collider en mode Trigger. Dés qu'un joueur entre dedans il regen.

	public int regenHp = 2;
	public int regenMp = 1;

	void OnTriggerStay(Collider other)
	{
		if (other.gameObject.layer.ToString () == "Player") 
		{
			other.GetComponent<GenericLifeScript> ().currentHp += regenHp;
			other.GetComponent<GenericManaScript> ().currentMp += regenMp;
		}
	}
}
