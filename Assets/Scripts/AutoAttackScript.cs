using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AutoAttackScript : MonoBehaviour {

	//ce script gere l'auto attack de l'objet auquel il est attacher.
	Animator anim;
	public bool stopWalk; //pour l animation
	bool charge;
	bool attackAnim;
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
		anim = GetComponentInChildren<Animator> ();
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
		if (target == null ) 
		{					
			LooseTarget ();
			
			if (isAttacking) {
				if (gameObject.layer == 8) {
					agent.SetDestination (transform.position);
				}
				StopAttacking ();
			}
		}
		// si il est arreter
		if (!agent.pathPending) 
		{
			if (agent.remainingDistance <= agent.stoppingDistance) 
			{
				if(!agent.hasPath || agent.velocity.sqrMagnitude == 0f){
				if (gameObject.layer == 8) {		
					// si c'est le joueur
					if (!stopWalk) {
						//ecrire ici ton animation de joueur idle.
						stopWalk = true;
						anim.SetBool ("stopwalk", stopWalk);
						}}
				}
			}
		}
	}
	public void AttackTheTarget()
	{
			agent.Stop ();

		isAttacking = true;
		attackAnim = true;
		if (gameObject.layer == 8) {
			anim.SetBool ("attack", attackAnim);
		}
		if(gameObject.layer == 9){
		anim.SetBool ("attackEnnemi", attackAnim);
		}}
	public void StopAttacking()
	{
		isAttacking = false;
		attackAnim = false;
		if (gameObject.layer == 8) {
			anim.SetBool ("attack", attackAnim);
		}
		if (gameObject.layer == 9) {
			anim.SetBool ("attackEnnemi", attackAnim);
		}
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
		if (gameObject.layer == 8) 
		{
			//faire ici ton animation de charge.
			charge = true;
			anim.SetBool ("charge", charge);
		}
	}
	public void LooseTarget()
	{
		target = null;
		if (gameObject.layer == 9) 
		{
			GetComponent<MinionsPathFindingScript> ().GoToEndGame ();
		}
		if (gameObject.layer == 8 && charge) 
		{
			//faire ici l'arret de la charge.
			charge = false;
			anim.SetBool ("charge", charge);
		}
	}
}
