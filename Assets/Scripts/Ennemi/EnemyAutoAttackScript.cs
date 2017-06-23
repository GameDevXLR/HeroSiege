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
		public AudioClip ennemiAtt; //quel sons pour les mobs. 
		Animator anim; // l'animator qui gere les anim lié a ce script
//		public bool stopWalk; //pour l animation : arrete de marcher
		bool attackAnim; // dois je jouer l'animation d'attaque ? 
		bool walkAnim;
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
		public bool isActuAttacking;
		public bool isActuStopAttacking;
	public bool isLoosingTarget;
	public bool hasAttEffect; // il a un effet de particule quand il attaque ? 

	void Start()
		{

			agent = GetComponent<NavMeshAgent> ();
			anim = GetComponentInChildren<Animator> ();
			audioSource = GetComponent<AudioSource> ();
		if (hasAttEffect) 
		{
			particule = transform.Find ("MobAttParticle").GetComponent<ParticleSystem> ();
		}
			if (isServer) 
			{
				GetComponentInChildren<EnnemiAggroManagerScript> ().enabled = true;
				GetComponentInChildren<SphereCollider> ().enabled = true;
			}

		}

	void Update ()
	{
		if (anim) 
		{
			if (anim.GetBool ("attackEnnemi") == false) 
			{
				anim.SetFloat ("realSpeed", Mathf.Clamp(agent.velocity.sqrMagnitude,0.1f,1f));
			}
		}
		if (isUnderCC) 
		{
			return;
		}
		if (target != null && isServer) 
		{
			if (((target.tag == "Player" && target.GetComponent<PlayerIGManager> ().isDead) || target.tag == "Pet" && target.GetComponent<PetIGManager>().isDead) && !isLoosingTarget) 
			{
				isLoosingTarget = true;
				LooseTarget ();
				return;
			}
		}
		if (isServer) 
		{
			if (target) 
			{
				if (!isAttacking) 
				{
					if (!isActuAttacking && Vector3.Distance (transform.localPosition, target.transform.localPosition) <= attackRange) 
					{
						isActuAttacking = true;
						RpcAttackTarget (transform.position);
					}
				} else 
				{
					if (Time.time > previousAttackTime) 
					{
						previousAttackTime = Time.time + attackRate;
						if (target.tag == "Player") 
						{
							target.GetComponent<PlayerIGManager> ().LooseHealth (damage, false, gameObject);
							if (GetComponent<EnnemyIGManager> ().isSlowingOnAutoA) 
							{
								GetComponent<EnemySlowPlayer> ().SlowTheTarget (target, attackRate);
							}
							if (GetComponent<EnnemyIGManager> ().isCastingAoeCC) 
							{
								GetComponent<EnemyCastAoeCC> ().AddACharge ();
							}
							if (GetComponent<EnnemyIGManager> ().isCCOnAutoA) 
							{
								//10% de chance de stun (1s par défaut)
								if (Random.Range (0, 10) > 8) 
								{
									GetComponent<EnemyCCPlayer> ().CCTheTarget (target);
								}
							}
						}
                        else
                            target.GetComponent<PetIGManager>().LooseHealth(damage, false, gameObject);
                    }
					if (!isActuStopAttacking && Vector3.Distance ( transform.localPosition, target.transform.localPosition) > attackRange 
                        || target == null 
                        || (target.tag == "Player" && target.GetComponent<PlayerIGManager> ().isDead ) 
                        || (target.tag != "Player" && target.GetComponent<PetIGManager>().isDead))
                    {
						isActuStopAttacking = true;
						RpcStopAttacking ();
					}
				}
			}
		}
		if (target) 
		{
			//rajouter ici une condition du genre "si la distance est basse" alors tu face le mec; sinon tu face ton chemin...

				if (!isAttacking) 
			{
					if (Vector3.Distance (targetTempPos, target.transform.localPosition) > 0 && !isActualizingPos) 
				{
						if (Vector3.Distance (transform.position, target.transform.position) > attackRange + 0.5f) 
					{
							StartCoroutine (ActualizeTargetPos ());
					}
				}
			}
			if (target && Vector3.Distance (transform.position, target.transform.position) < attackRange) 
			{
				Quaternion targetRot = Quaternion.LookRotation (target.transform.position - transform.position);
				float str = Mathf.Min (rotSpeed * Time.deltaTime, 1);
				transform.rotation = Quaternion.Lerp (transform.rotation, targetRot, str);
			}

		}

	}
	

		[ClientRpc]
	public void RpcAttackTarget(Vector3 pos)
		{
		if (GetComponent<EnnemyIGManager> ().isAnInvisible) 
		{
			GetComponent<EnnemyIGManager>().deadAnimChildMesh.GetComponentInChildren<SkinnedMeshRenderer> ().enabled = true;
			transform.Find ("MiniMapIcon").GetComponent<SpriteRenderer> ().enabled = true;
		}
		agent.velocity = Vector3.zero;
		agent.isStopped = true;
		isAttacking = true;
			attackAnim = true;
			agent.enabled = false;
			GetComponent<NavMeshObstacle> ().enabled = true;
			if (anim != null) {
				anim.SetBool ("attackEnnemi", attackAnim);
			}
			audioSource.PlayOneShot (ennemiAtt);
			if (particule != null) {
				particule.Play ();
			}
		isActuAttacking = false;
		

			

		}

		[ClientRpc]
	public void RpcStopAttacking()
	{
		if (GetComponent<EnnemyIGManager> ().isAnInvisible) 
		{
			GetComponent<EnnemyIGManager>().deadAnimChildMesh.GetComponentInChildren<SkinnedMeshRenderer> ().enabled = false;
			transform.Find ("MiniMapIcon").GetComponent<SpriteRenderer> ().enabled = false;
		}
		GetComponent<NavMeshObstacle> ().enabled = false;
		agent.enabled = true;
		isAttacking = false;
		attackAnim = false;
		if (gameObject.name !="mobJung2(clone)") {
			anim.SetBool ("attackEnnemi", attackAnim);
		}
		if (particule != null) {
			particule.Stop ();
		}

		agent.isStopped = false;
		isActuStopAttacking = false;
	}

	public void AcquireTarget(NetworkInstanceId id)
		{
		StartCoroutine (AcquireTargetProcess ());

	}
	IEnumerator AcquireTargetProcess()
	{
		if (gameObject.name != "Goblin_Ranger_R1") {
			
			if (anim) {
				anim.SetBool ("walk", walkAnim = true);
			}
		}
		yield return new WaitForSeconds(0.1f);
		if (target != null) {
			if (agent.isActiveAndEnabled) {
				agent.SetDestination (target.transform.localPosition); 
				agent.stoppingDistance = attackRange;
				targetTempPos = target.transform.localPosition;
			}
		} else {
			target = GetComponent<MinionsPathFindingScript> ().target.gameObject;
			if (gameObject.layer != 8) 
			{
			StopAllCoroutines ();
				GetTargetFromID (targetID);
			}
				anim.SetBool ("walk", walkAnim = false);
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
		if (GetComponent<EnnemyIGManager> ().isAnInvisible) 
		{
			GetComponent<EnnemyIGManager>().deadAnimChildMesh.GetComponentInChildren<SkinnedMeshRenderer> ().enabled = false;
			transform.Find ("MiniMapIcon").GetComponent<SpriteRenderer> ().enabled = false;

		}
			target = null;
			GetComponent<NavMeshObstacle> ().enabled = false;
			agent.enabled = true;
			agent.isStopped = false;
		if (gameObject.name != "Goblin_Ranger_R1") {
			
			anim.SetBool ("attackEnnemi", attackAnim = false);
			anim.SetBool ("walk", walkAnim = false);
		}
			if (particule != null) 
			{
				particule.Stop ();
			}

		GetComponent<MinionsPathFindingScript> ().GoToEndGame ();
		isLoosingTarget = false;
		
	}
	IEnumerator ActualizeTargetPos()
	{

		isActualizingPos = true;
		if ((target.gameObject.tag == "Player" && target.GetComponent<PlayerIGManager>().isDead) 
            || (target.gameObject.tag == "Pet" && target.GetComponent<PetIGManager> ().isDead) 
            || Vector3.Distance (transform.localPosition, target.transform.localPosition) > detectionRange)
		{
			target = GetComponent<MinionsPathFindingScript> ().target.gameObject;
			if (isServer) {
				SetTheTarget (target);
			}
			agent.SetDestination (target.transform.position);
			yield return new WaitForSeconds (Random.Range( 0.20f, 0.30f));
			isActualizingPos = false;
			
		} else 
		{
			GetComponent<NavMeshObstacle> ().enabled = false;
			agent.enabled = true;
			if (agent.isOnNavMesh) {
				agent.isStopped = false;
				agent.SetDestination (target.transform.position);
			}
			targetTempPos = target.transform.position;
		}
		yield return new WaitForSeconds (Random.Range( 0.20f, 0.30f));
		isActualizingPos = false;
	}

	public void GetTargetFromID(NetworkInstanceId id)
	{
		targetID = id;
		target = ClientScene.FindLocalObject (id);
		if (target != null) {
			targetTempPos = target.transform.position;
		}
		StartCoroutine (AcquireTargetProcess ());

	}
	[Server]
	public void SetTheTarget(GameObject targ)
	{
		if (targ.GetComponent<NetworkIdentity> ()) {
			targetID = targ.GetComponent<NetworkIdentity> ().netId;
		} else 
		{
			Debug.Log ("manque un netid");
		}
	}
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
			agent.velocity = Vector3.zero;
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
