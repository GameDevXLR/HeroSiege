using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MinionsPathFindingScript : MonoBehaviour {
	public NavMeshAgent agent;
	public Transform target;
	public float stopTime = 2f;
	// Use this for initialization
	void Start () {
		agent = GetComponent<NavMeshAgent> ();
		target = GameObject.Find ("EndPointForMobs").transform;
		agent.SetDestination (target.position);

	}
	
	// Update is called once per frame
	void Update () {

	}
}
