using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerClicToMove : MonoBehaviour {

	Animator anim;
//	bool walk;
	public NavMeshAgent agentPlayer;
	public AutoAttackScript attackScript;
	public GameObject target;
	int layer_mask;
	// Use this for initialization
	void Start () {
		layer_mask = LayerMask.GetMask ("Ground", "Ennemies");
		agentPlayer = GetComponent<NavMeshAgent> ();
		attackScript = GetComponent<AutoAttackScript> ();
		anim = GetComponentInChildren<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetMouseButtonUp (1)) {
			anim.SetBool ("stopwalk", false);

//			walk = false;
			attackScript.stopWalk = false;
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			if (Physics.Raycast (ray, out hit, 50f, layer_mask)) {	

//				Vector3 tempHitPoint = new Vector3 (hit.point.x, 1f, hit.point.z);
				agentPlayer.destination =hit.point;
				GetComponentInChildren<PlayerEnnemyDetectionScript> ().autoTargetting = true;
				
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

