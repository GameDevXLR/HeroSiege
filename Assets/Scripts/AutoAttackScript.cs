﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AutoAttackScript : MonoBehaviour {

	//ce script gere l'auto attack de l'objet auquel il est attacher.


	public NavMeshAgent agent;
	public float attackRange;
	public float attackRate;
	private float previousAttackTime;
	public int damage;
	public bool isAttacking;
	public GameObject target;

	void Start()
	{
		agent = GetComponent<NavMeshAgent> ();
	}

	void Update ()
	{
		if (Time.time > previousAttackTime) {
			previousAttackTime = Time.time + attackRate;

			if (isAttacking && target) {
				Debug.Log ("toucher");
				target.GetComponent<GenericLifeScript> ().LooseHealth (damage, false);
			}
		
		}
		if (target) {
			if (agent.remainingDistance <= attackRange) {
				AttackTheTarget ();
			}
			if (isAttacking) {
				if (agent.remainingDistance > attackRange) {
					StopAttacking ();
				}
			}
		}
	}
	public void AttackTheTarget()
	{
		agent.Stop ();
		isAttacking = true;
	}
	public void StopAttacking()
	{
		isAttacking = false;
		agent.Resume ();
	}
	public void AcquireTarget(GameObject newTarget)
	{
		target = newTarget;
	}
	public void LooseTarget()
	{
		target = null;
	}
}
