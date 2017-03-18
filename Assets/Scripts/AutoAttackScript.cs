using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;
using UnityEngine.UI;

[NetworkSettings(channel = 0, sendInterval = 0.3f)]
public class AutoAttackScript : NetworkBehaviour 
{

	//ce script gere l'auto attack de l'objet auquel il est attacher.
	// il a besoin d'etre sous divisé (un pour le joueur; un pour les mobs, et un autre pour les tours meme)
	public AudioSource audioSource; // qui joue le son
	public AudioClip[] ennemiSounds; //quels sons pour les ennemis (tous)

	Animator anim; // l'animator qui gere les anim lié a ce script

	bool attackAnim; // dois je jouer l'animation d'attaque ? 
	public NavMeshAgent agent; // l'agent qui permet de déplacer l'objet attacher
	public float attackRange; // la portée des auto attaques
	public float attackRate; // le rate d'attaque par seconde
	private float previousAttackTime; // privé : le temps global de la derniere attaque
	public int damage; // combien de dégats brut (hors armure) on fait.
	public bool isAttacking; //suis je en train d'attaquer ? 
	public GameObject target; // qui est ma cible ? 
	public float rotSpeed = 5; // permet de tourner plus vite vers la cible. résoud un bug lié au fait que les objets étaient trop petit.
	private Vector3 targetTempPos; //calcul de position (privé)
	private bool isActualizingPos; // suis je déja en train d'envoyer ma nouvelle position ? 
	private GameObject targetObj; // l'objet qui t'attaque ! 


	void Start()
	{
		
		agent = GetComponent<NavMeshAgent> ();
		anim = GetComponentInChildren<Animator> ();
		audioSource = GetComponent<AudioSource> ();

	}

	void Update ()
	{
		if (target) 
		{
			if (!isAttacking) 
			{
				if (Vector3.Distance (transform.position, target.transform.position) <= attackRange) {
					AttackTheTarget ();
				} else 
				{

						if (Vector3.Distance (targetTempPos, target.transform.position) > 0 && !isActualizingPos) {
							StartCoroutine (ActualizeTargetPos());
						}

				}
			}

			if (isAttacking) 
			{
				Quaternion targetRot = Quaternion.LookRotation (target.transform.position - transform.position);
				float str = Mathf.Min (rotSpeed * Time.deltaTime, 1);
				transform.rotation = Quaternion.Lerp (transform.rotation, targetRot, str);
				if (Time.time > previousAttackTime) 
				{
					previousAttackTime = Time.time + attackRate;
					target.GetComponent<GenericLifeScript> ().LooseHealth (damage, false, gameObject);
				}
				if (Vector3.Distance (transform.position, target.transform.position) > attackRange || target.GetComponent<GenericLifeScript> ().isDead) 
				{
					StopAttacking ();
				}
			}
		} else 
		{
			LooseTarget ();
			if (isAttacking) 
			{
				StopAttacking ();
			}
		}

	}
	IEnumerator ActualizeTargetPos()
		{
			isActualizingPos = true;
			agent.SetDestination (target.transform.position);
			targetTempPos = target.transform.position;
		yield return new WaitForSeconds (Random.Range( 0.2f, 0.3f));
			isActualizingPos = false;
		}

	public void AttackTheTarget()
	{
		
		agent.Stop ();
		isAttacking = true;
		attackAnim = true;
		agent.enabled = false;
		GetComponent<NavMeshObstacle> ().enabled = true;
		anim.SetBool ("attackEnnemi", attackAnim);
		audioSource.clip = ennemiSounds [0];
		audioSource.Play();
		
	}
	public void StopAttacking()
	{
		isAttacking = false;
		attackAnim = false;
		GetComponent<NavMeshObstacle> ().enabled = false;

		agent.enabled = true;
		audioSource.Stop ();
		anim.SetBool ("attackEnnemi", attackAnim);
		agent.Resume ();
		GetComponent<MinionsPathFindingScript> ().GoToEndGame ();

	}
	public void AcquireTarget(GameObject newTarget)
	{
		target = newTarget;
		if (agent.isOnNavMesh) {
			agent.SetDestination (target.transform.position);
		}
		targetTempPos = target.transform.position;


	}
	public void LooseTarget()
	{
		target = null;
		GetComponent<MinionsPathFindingScript> ().GoToEndGame ();


	}

}
