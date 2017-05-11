﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AllyAggroManager : MonoBehaviour {


	//ce script gere l'aggro : la zone d'aggro; le switch d'aggro; et tout ce qui va avec.
	// il doit etre placer sur un enfant de l'objet qui doit détecter l'aggro (le mob).
	// il doit contenir obligatoirement un collider Trigger ! qui correspondra donc a la zone d'aggro du mob.
	//Si un mob n'a plus de cible, il va directement en chercher une autre.

	Animator anim;

	//	private NavMeshAgent agentParent;
	private AllyPetAutoAttack autoAScript;
	// Use this for initialization
	void Start () 
	{
		//		agentParent =GetComponentInParent<NavMeshAgent> ();
		autoAScript = GetComponentInParent<AllyPetAutoAttack> ();

	}

	void OnTriggerStay(Collider other)
	{

		if (autoAScript.target == null) 
		{
			if (other.gameObject.layer == 9) 
			{
//				if (transform.parent.gameObject.layer == 8 && other.gameObject.layer == 8) 
//				{
//					//en gros si t'es un pet et que tu target un joueur ben oubli...
//					return;
//				}
				GetComponentInParent<AllyPetAutoAttack> ().SetTheTarget(other.gameObject);
			}
		}
	}
	void OnTriggerExit(Collider other)
	{
		if (autoAScript.target != null) 
		{
			if (other.gameObject == autoAScript.target) {
				if (!GetComponentInParent<AllyPetAutoAttack> ().isUnderCC && !GetComponentInParent<GenericLifeScript> ().isTaunt) 
				{
					GetComponentInParent<AllyPetAutoAttack> ().LooseTarget ();
				}
			}
		}
	}



}
