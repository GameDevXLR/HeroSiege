using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;
using UnityEngine.UI;

[NetworkSettings(channel = 0, sendInterval = 0.3f)]
public class EnemyAutoAttackScript : NetworkBehaviour {
	

		//ce script gere l'auto attack de l'objet auquel il est attacher.
		//subidivision special ennemy.
		private AudioSource audioSource; // qui joue le son
		public AudioClip[] enemiSounds; //quel sons pour les mobs. 
		Animator anim; // l'animator qui gere les anim lié a ce script
//		public bool stopWalk; //pour l animation : arrete de marcher
		bool attackAnim; // dois je jouer l'animation d'attaque ? 
		public NavMeshAgent agent; // l'agent qui permet de déplacer l'objet attacher
		public float attackRange; // la portée des auto attaques
		public float attackRate; // le rate d'attaque par seconde
		private float previousAttackTime; // privé : le temps global de la derniere attaque
		public int damage; // combien de dégats brut (hors armure) on fait.
	[SyncVar]public bool isAttacking; //suis je en train d'attaquer ? A sync !!!
	[SyncVar]public GameObject target; // qui est ma cible ? ( sur le serveur..retransmis)
		public float rotSpeed = 5; // permet de tourner plus vite vers la cible. résoud un bug lié au fait que les objets étaient trop petit.
		private Vector3 targetTempPos; //calcul de position (privé)
		private GameObject targetObj; // l'objet qui t'attaque ! 
		private bool isActualizingPos; // suis je déja en train d'actualiser la position de ma cible?
		private ParticleSystem particule;

		void Start()
		{

			agent = GetComponent<NavMeshAgent> ();
			anim = GetComponentInChildren<Animator> ();
			audioSource = GetComponent<AudioSource> ();
		particule = GetComponentInChildren<ParticleSystem> ();


		}

		void Update ()
		{
			if (isServer) 
			{
				if (target) 
				{
					if (!isAttacking) 
					{
					if (Vector3.Distance (transform.position, target.transform.position) <= attackRange) 
						{

						RpcAttackTarget (transform.position);
						}
					} else 
					{
						if (Time.time > previousAttackTime) 
						{
							previousAttackTime = Time.time + attackRate;
							target.GetComponent<GenericLifeScript> ().LooseHealth (damage, false, gameObject);
						}
						if (Vector3.Distance (transform.position, target.transform.position) > attackRange || target == null|| target.GetComponent<GenericLifeScript> ().isDead) 
						{
							RpcStopAttacking ();
						}

					}
				}
				if (target == null && isAttacking) 
				{
					RpcStopAttacking ();
				}
			}
			if (target) 
			{
				Quaternion targetRot = Quaternion.LookRotation (target.transform.position - transform.position);
				float str = Mathf.Min (rotSpeed * Time.deltaTime, 1);
				transform.rotation = Quaternion.Lerp (transform.rotation, targetRot, str);
			if (!isAttacking) 
			{
				if (Vector3.Distance (targetTempPos, target.transform.position) > 0f && !isActualizingPos) 
				{
					if (Vector3.Distance (transform.position, target.transform.position) > attackRange) 
					{
						StartCoroutine (ActualizeTargetPos ());
					}
				}

			}

			}
		}

		[ClientRpc]
	public void RpcAttackTarget(Vector3 pos)
		{
			transform.position = pos;
			agent.Stop ();
			isAttacking = true;
			attackAnim = true;
			agent.enabled = false;
			GetComponent<NavMeshObstacle> ().enabled = true;
			anim.SetBool ("attackEnnemi", attackAnim);
		if (particule != null) {
			particule.Play ();
		}
			audioSource.clip = enemiSounds [0];
			audioSource.Play();
			

		}

		[ClientRpc]
		public void RpcStopAttacking()
		{
		GetComponent<NavMeshObstacle> ().enabled = false;
		agent.enabled = true;

		if (target == null) 
		{
			GetComponent<MinionsPathFindingScript> ().GoToEndGame ();
		}else if (target.GetComponent<GenericLifeScript> ().isDead) 
		{
			target = null;
			GetComponent<MinionsPathFindingScript> ().GoToEndGame ();
		}
			isAttacking = false;
			attackAnim = false;
			agent.Resume ();
			audioSource.Stop ();
			anim.SetBool ("attackEnnemi", attackAnim);
		}

	public void AcquireTarget(NetworkInstanceId id)
		{
		StartCoroutine (AcquireTargetProcess (id));
		//			audioSource.PlayOneShot (enemiSounds [1], .6f); // jouer un son d'aggro
		}
	IEnumerator AcquireTargetProcess(NetworkInstanceId id)
	{
		target = ClientScene.FindLocalObject (id);
		yield return new WaitForSeconds (0.2f);
		if (agent.isOnNavMesh) 
		{
			agent.SetDestination (ClientScene.FindLocalObject (id).transform.position); //obliger de refaire la recherche pour le moment :/ sinon ya pas forcemment le temps de faire la recherche :/
			agent.stoppingDistance = attackRange;

		}
		if (target != null) {
			targetTempPos = target.transform.position;
		}
	}

		public void LooseTarget()
		{
			target = null;
			isAttacking = false;
			attackAnim = false;
			GetComponent<NavMeshObstacle> ().enabled = false;
			agent.enabled = true;
			agent.Resume ();
			audioSource.Stop ();
			anim.SetBool ("attackEnnemi", attackAnim);
			GetComponent<MinionsPathFindingScript> ().GoToEndGame ();
		if (particule != null) {
			particule.Stop ();
		}

		}
	IEnumerator ActualizeTargetPos()
	{

		isActualizingPos = true;
		if (target.GetComponent<GenericLifeScript> ().isDead) 
		{
			GetComponent<MinionsPathFindingScript> ().GoToEndGame ();
		} else 
		{
			agent.SetDestination (target.transform.position);
			targetTempPos = target.transform.position;
		}
		yield return new WaitForSeconds (Random.Range( 0.2f, 0.3f));
		isActualizingPos = false;
	}

}
