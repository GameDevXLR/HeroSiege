using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TowerAggroManager: MonoBehaviour {


	//ce script gere l'aggro : la zone d'aggro; le switch d'aggro; et tout ce qui va avec.
	// il doit etre placer sur un enfant de l'objet qui doit détecter l'aggro (le mob).
	// il doit contenir obligatoirement un collider Trigger ! qui correspondra donc a la zone d'aggro du mob.
	//Si un mob n'a plus de cible, il va directement en chercher une autre.

	Animator anim;

	//	private NavMeshAgent agentParent;
	private TowerPetAutoA autoAScript;
	// Use this for initialization
	void Start () 
	{
		//		agentParent =GetComponentInParent<NavMeshAgent> ();
		autoAScript = GetComponentInParent<TowerPetAutoA> ();

	}

	void OnTriggerStay(Collider other)
	{

		if (autoAScript.target == null) 
		{
			if (other.gameObject.layer == 9) {
				if (!other.GetComponent<EnnemyIGManager> ().isDead) {
					autoAScript.SetTheTarget (other.gameObject);
				}
			}
		}
	}
	void OnTriggerExit(Collider other)
	{
		if (autoAScript.target != null) 
		{
			if (other.gameObject == autoAScript.target) {

					autoAScript.LooseTarget ();

			}
		}
	}



}
