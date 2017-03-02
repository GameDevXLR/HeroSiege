using System.Collections;
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
				target.GetComponent<GenericLifeScript> ().LooseHealth (damage, false);
			}
		
		}
		if (target) {
			if (agent.remainingDistance <= attackRange) {
				AttackTheTarget ();
			}
			if (isAttacking) {
				if (agent.remainingDistance > attackRange+0.5f) {
					StopAttacking ();
				}
			}
			if (gameObject.layer == 9) 
			{
				agent.SetDestination (target.transform.position);
			}
			if (target.GetComponent<GenericLifeScript> ().isDead) 
			{
				StopAttacking ();
			}
		}
		if (target == null && isAttacking) 
		{
			if(gameObject.layer == 8){
			agent.SetDestination (transform.position);
			}
				StopAttacking ();
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
		if (gameObject.layer == 9) 
		{
			GetComponent<MinionsPathFindingScript> ().GoToEndGame ();

		}
	}
	public void AcquireTarget(GameObject newTarget)
	{
		target = newTarget;
		if (gameObject.layer == 9) 
		{
			agent.SetDestination (target.transform.position);

		}
	}
	public void LooseTarget()
	{
		target = null;
		if (gameObject.layer == 9) 
		{
			GetComponent<MinionsPathFindingScript> ().GoToEndGame ();
		}
	}
}
