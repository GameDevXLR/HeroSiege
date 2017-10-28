using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnnemiAggroManagerScript : MonoBehaviour 
{

	//ce script gere l'aggro : la zone d'aggro; le switch d'aggro; et tout ce qui va avec.
	// il doit etre placer sur un enfant de l'objet qui doit détecter l'aggro (le mob).
	// il doit contenir obligatoirement un collider Trigger ! qui correspondra donc a la zone d'aggro du mob.
	//Si un mob n'a plus de cible, il va directement en chercher une autre.

	Animator anim;

//	private NavMeshAgent agentParent;
	private EnemyAutoAttackScript autoAScript;
	// Use this for initialization
	void Awake () 
	{
//		agentParent =GetComponentInParent<NavMeshAgent> ();
		autoAScript = GetComponentInParent<EnemyAutoAttackScript> ();

	}

	void OnTriggerStay(Collider other)
	{
		if (!autoAScript) 
		{
			return;
		}

		if (!GetComponentInParent<EnnemyIGManager>().isDead && (autoAScript.target == null || autoAScript.target.layer != 8)) 
		{
			if (other.gameObject.layer == 8 || other.gameObject.layer == 9) {
				if (transform.parent.gameObject.layer == 8 && other.gameObject.layer == 8) {
					//en gros si t'es un pet et que tu target un joueur ben oubli...
					return;
				}
				if (other.gameObject.layer != Layers.UI) {
					GetComponentInParent<EnemyAutoAttackScript> ().SetTheTarget (other.gameObject);
				} else 
				{
					Debug.Log ("code pansement XD : peut provoquer des problemes");
				}
			}
		}
	}
		void OnTriggerExit(Collider other)
	{
		if (autoAScript.target != null) 
		{
			if (other.gameObject == autoAScript.target) 
			{
				if (!GetComponentInParent<EnemyAutoAttackScript> ().isUnderCC && !GetComponentInParent<EnnemyIGManager> ().isTaunt) 
				{
					GetComponentInParent<EnemyAutoAttackScript> ().LooseTarget ();
				}
			}
		}
	}


		
}

