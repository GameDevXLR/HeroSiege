using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndOfRoadPortalScript : MonoBehaviour {
	//portail de fin de route pour les mobs : si ils l'atteignent  : on perd une vie. si le nombre de vie tombe a zero : on a perdu la game.

	// Use this for initialization
	void Start () 
	{
			
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == 9) // layer9 is Ennemies.
		{
			other.gameObject.GetComponent<GenericLifeScript> ().guyAttackingMe = null;
			other.gameObject.GetComponent<GenericLifeScript> ().MakeHimDie();
			GameManager.instanceGM.LooseALife ();
		}
	}
}
