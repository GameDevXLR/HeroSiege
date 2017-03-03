using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnnemiAggroManagerScript : MonoBehaviour {

	//ce script gere l'aggro : la zone d'aggro; le switch d'aggro; et tout ce qui va avec.
	// il doit etre placer sur un enfant de l'objet qui doit détecter l'aggro (le mob).
	// il doit contenir obligatoirement un collider Trigger ! qui correspondra donc a la zone d'aggro du mob.
	//Si un mob n'a plus de cible, il va directement en chercher une autre.

	Animator anim;

	private NavMeshAgent agentParent;
	private AutoAttackScript autoAScript;
	// Use this for initialization
	void Start () {
		agentParent =GetComponentInParent<NavMeshAgent> ();
		autoAScript = GetComponentInParent<AutoAttackScript> ();
		
	}
	
	void OnTriggerStay(Collider other){
		if (other.gameObject.layer == 8) {
			if (autoAScript.target == null) {
				autoAScript.AcquireTarget (other.gameObject);
			}
		}
	}
		void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer == 8) {
			autoAScript.LooseTarget ();
		}
	}
		
	}

