using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerEnnemyDetectionScript : MonoBehaviour {
	//permet au perso du joueur de cibler automatiquement l'ennemi le plus proche.

	public bool autoTargetting;
	public AutoAttackScript autoAScript;
	public NavMeshAgent playerAgent;
	// Use this for initialization
	void Start () {
		autoAScript = GetComponentInParent<AutoAttackScript> ();
		playerAgent = GetComponentInParent<NavMeshAgent> ();
		autoTargetting = true;
	}
	
	// Update is called once per frame
	void OnTriggerStay (Collider other)
	{
		if (autoTargetting) {
			if (playerAgent.velocity == Vector3.zero) {
				if (other.gameObject.layer == 9) {
					if (autoAScript.target == null) {
						playerAgent.SetDestination (other.transform.position);
						autoAScript.AcquireTarget (other.gameObject);
					}
				}
			}
		}
	}
	void Update()
	{
		if (Input.GetKeyUp (KeyCode.S)) 
		{
			autoTargetting = false;
			playerAgent.SetDestination (GetComponentInParent<Transform> ().position);
			if (autoAScript.isAttacking) 
			{
				autoAScript.LooseTarget ();
			}
		}
	}
}
