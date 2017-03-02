using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerClicToMove : MonoBehaviour {

	public NavMeshAgent agentPlayer;
	public AutoAttackScript attackScript;
	public GameObject target;
	// Use this for initialization
	void Start () {
		agentPlayer = GetComponent<NavMeshAgent> ();
		attackScript = GetComponent<AutoAttackScript> ();
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetMouseButtonDown (1)) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			if (Physics.Raycast (ray, out hit)) {	
				agentPlayer.destination = hit.point;
				
				if (hit.collider.gameObject.layer == 9) {
					target = hit.collider.gameObject;
					agentPlayer.stoppingDistance = 1;
					attackScript.AcquireTarget (target);


				} else {
					target = null;
					agentPlayer.stoppingDistance = 0;
					attackScript.LooseTarget ();
				}
			}
		
		}
		if (target) 
		{
			agentPlayer.SetDestination (target.transform.position);
		}
	}
}

