using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EndOfRoadPortalScript : NetworkBehaviour {
	//portail de fin de route pour les mobs : si ils l'atteignent  : on perd une vie. si le nombre de vie tombe a zero : on a perdu la game.

	public int teamNbr; // détermine a quel équipe il fait perdre des vies. A CONFIGURER IMPERATIVEMENT.
	// Use this for initialization
	void Start () 
	{
		if (!isServer) 
		{
			this.enabled = false;
		}
	}

	public void OnTriggerEnter(Collider other)
	{
		
		if (other.gameObject.layer == 9) { // layer9 is Ennemies.
			other.gameObject.GetComponent<GenericLifeScript> ().guyAttackingMe = null;
			other.gameObject.GetComponent<GenericLifeScript> ().MakeHimDie ();

			if (teamNbr == 1) 
			{
				GameManager.instanceGM.Team1LooseALife ();
			}
			if (teamNbr == 2) 
			{
				GameManager.instanceGM.Team2LooseALife ();

			}
		}
	}
}
