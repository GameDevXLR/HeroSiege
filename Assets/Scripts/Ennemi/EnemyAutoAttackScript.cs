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
		public AudioSource audioSource; // qui joue le son
		public AudioClip ennemiAtt; //quel sons pour les mobs. 
		Animator anim; // l'animator qui gere les anim lié a ce script
//		public bool stopWalk; //pour l animation : arrete de marcher
		bool attackAnim; // dois je jouer l'animation d'attaque ? 
		public NavMeshAgent agent; // l'agent qui permet de déplacer l'objet attacher
		public float attackRange; // la portée des auto attaques
		public float attackRate; // le rate d'attaque par seconde
		private float previousAttackTime; // privé : le temps global de la derniere attaque
		public int damage; // combien de dégats brut (hors armure) on fait.
		[SyncVar]public bool isAttacking; //suis je en train d'attaquer ? A sync !!!
		[SyncVar(hook ="GetTargetFromID" )]public NetworkInstanceId targetID; // la target.
		public GameObject target; // qui est ma cible ? ( sur le serveur..retransmis)
		public float rotSpeed = 5; // permet de tourner plus vite vers la cible. résoud un bug lié au fait que les objets étaient trop petit.
		private Vector3 targetTempPos; //calcul de position (privé)
		private GameObject targetObj; // l'objet qui t'attaque ! 
		private bool isActualizingPos; // suis je déja en train d'actualiser la position de ma cible?
		private ParticleSystem particule;
		public float detectionRange = 20f;
		public Vector3 desiredPos; // ou est ce que le serveur me dit que jdevrais etre. lerp vers ca.
		public bool isUnderCC;

	void Start()
		{

			agent = GetComponent<NavMeshAgent> ();
			anim = GetComponentInChildren<Animator> ();
			audioSource = GetComponent<AudioSource> ();
			particule = GetComponentInChildren<ParticleSystem> ();

			if (isServer) 
			{
				GetComponentInChildren<EnnemiAggroManagerScript> ().enabled = true;
				GetComponentInChildren<SphereCollider> ().enabled = true;
			}

		}

	void Update ()
	{
		if (isUnderCC) {
			return;
		}
		if (isServer) {
			if (target) {
				if (!isAttacking) {
					if (Vector3.Distance (transform.localPosition, target.transform.localPosition) <= attackRange) {
						RpcAttackTarget (transform.position);
					}
				} else {
					if (Time.time > previousAttackTime) {
						previousAttackTime = Time.time + attackRate;
						target.GetComponent<GenericLifeScript> ().LooseHealth (damage, false, gameObject);
					}
					if (Vector3.Distance (transform.localPosition, target.transform.localPosition) > attackRange || target == null || target.GetComponent<GenericLifeScript> ().isDead) {
						RpcStopAttacking ();
					}
				}
			}
			if (target == null && isAttacking) {
				RpcStopAttacking ();
			}
		}
			if (target) {
			Quaternion targetRot = Quaternion.LookRotation (target.transform.position - transform.position);
			float str = Mathf.Min (rotSpeed * Time.deltaTime, 1);
			transform.rotation = Quaternion.Lerp (transform.rotation, targetRot, str);

				if (!isAttacking) {
					if (Vector3.Distance (targetTempPos, target.transform.localPosition) > 0 && !isActualizingPos) {
						if (Vector3.Distance (transform.position, target.transform.position) > attackRange + 0.5f) {
							StartCoroutine (ActualizeTargetPos ());
//						} else 
////					{
//							if (Vector3.Distance (desiredPos, transform.position) > 0.5f && desiredPos != Vector3.zero) {
//								transform.position = Vector3.Lerp (transform.position, desiredPos, 5 * Time.deltaTime);
//							}
						}
					}
				}

			}

		}
	

		[ClientRpc]
	public void RpcAttackTarget(Vector3 pos)
		{
//		desiredPos = pos;

			agent.isStopped = true;

		isAttacking = true;
			attackAnim = true;
			agent.enabled = false;
			GetComponent<NavMeshObstacle> ().enabled = true;
			anim.SetBool ("attackEnnemi", attackAnim);
			audioSource.PlayOneShot(ennemiAtt);
		if (particule != null) {
			particule.Play ();
		}

			

		}

		[ClientRpc]
	public void RpcStopAttacking()
	{
		GetComponent<NavMeshObstacle> ().enabled = false;
		agent.enabled = true;
		isAttacking = false;
		attackAnim = false;
		anim.SetBool ("attackEnnemi", attackAnim);
		if (particule != null) {
			particule.Stop ();
		}
		agent.isStopped = false;
	}

	public void AcquireTarget(NetworkInstanceId id)
		{
		StartCoroutine (AcquireTargetProcess ());
	}
	IEnumerator AcquireTargetProcess()
	{

		yield return new WaitForSeconds(0.1f);
		if (target != null) {
			if (agent.isOnNavMesh) {
				agent.SetDestination (target.transform.localPosition); 
				agent.stoppingDistance = attackRange;
				targetTempPos = target.transform.localPosition;
				yield return null;
			}
		} else 
		{
			GetTargetFromID (targetID);
		}
	}
	public void LooseTarget()
	{
		RpcLooseTarget ();	
		isAttacking = false;
	}
	[ClientRpc]
	public void RpcLooseTarget()
	{
			target = null;
			attackAnim = false;
			GetComponent<NavMeshObstacle> ().enabled = false;
			agent.enabled = true;
		agent.isStopped = false;
			anim.SetBool ("attackEnnemi", attackAnim);
			GetComponent<MinionsPathFindingScript> ().GoToEndGame ();
			if (particule != null) 
			{
				particule.Stop ();
			}

		
	}
	IEnumerator ActualizeTargetPos()
	{

		isActualizingPos = true;
		if (target.GetComponent<GenericLifeScript> ().isDead || Vector3.Distance (transform.localPosition, target.transform.localPosition) > detectionRange)
		{
			target = null;
			GetComponent<MinionsPathFindingScript> ().GoToEndGame ();
		} else 
		{
//			GetComponent<NavMeshObstacle> ().enabled = false;
//			agent.Resume();
			agent.SetDestination (target.transform.position);
			targetTempPos = target.transform.position;
		}
		yield return new WaitForSeconds (Random.Range( 0.20f, 0.30f));
		isActualizingPos = false;
	}

	public void GetTargetFromID(NetworkInstanceId id)
	{
		targetID = id;
		target = ClientScene.FindLocalObject (id);
		targetTempPos = target.transform.position;
		StartCoroutine (AcquireTargetProcess ());

	}
	[Server]
	public void SetTheTarget(GameObject targ)
	{
			targetID = targ.GetComponent<NetworkIdentity> ().netId;
//		RpcActualizeAttackerPosition (transform.position);

	}
//	[ClientRpc]
//	public void RpcActualizeAttackerPosition(Vector3 pos)
//	{
//		if (!isServer) {
//			desiredPos = pos;
//		}
//	}
	public void GetCC(float dur)
	{
		RpcGetCCForTimer (dur);
	}
	[ClientRpc]
	public void RpcGetCCForTimer(float dura)
	{
		StartCoroutine(UnderCCProcedure(dura));
	}

	IEnumerator UnderCCProcedure(float durat)
	{
		isUnderCC = true;
		if (isAttacking) 
		{
			if (particule != null) 
			{
				particule.Stop ();
			}
			anim.SetBool ("attackEnnemi", false);
		}
		if (agent.isActiveAndEnabled) 
		{
			agent.isStopped = true;
		}
		anim.enabled = false;
		yield return new WaitForSeconds (durat);
		if (agent.isActiveAndEnabled) 
		{
			agent.isStopped = false;
		}
		if (isAttacking) 
		{
			if (particule != null) 
			{
				particule.Play ();
			}
			anim.SetBool ("attackEnnemi", true);
		}
		anim.enabled = true;

		isUnderCC = false;
	}
		
}
