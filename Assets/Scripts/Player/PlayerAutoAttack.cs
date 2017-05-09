using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;
using UnityEngine.UI;

[NetworkSettings(channel = 0, sendInterval = 0.1f)]
public class PlayerAutoAttack: NetworkBehaviour 
{

	//ce script gere l'auto attack de l'objet auquel il est attacher.
	//subidivision special player.
	//private AudioSource audioSource; // qui joue le son
	//public AudioClip[] playerSounds; //quel sons pour le joueur
	public AudioClip Att1;
	public AudioClip Att2;
	public AudioClip Charge;

	public Animator anim; // l'animator qui gere les anim lié a ce script
	public bool stopWalk; //pour l animation : arrete de marcher
	bool charge; // animation et code : charge vers un ennemi / mob
	bool attackAnim; // dois je jouer l'animation d'attaque ? 
	public NavMeshAgent agent; // l'agent qui permet de déplacer l'objet attacher
	[SyncVar]public float attackRange; // la portée des auto attaques
	[SyncVar]public float attackRate; // le temps entre 2 attack
	[SyncVar(hook = "ActualizeAttSpeed")]public float attackSpeedStat; //le multiplicateur par sec.
	private float previousAttackTime; // privé : le temps global de la derniere attaque
	[SyncVar(hook = "ActualizeDamage")]public int damage; // combien de dégats brut (hors armure) on fait.
	public float attackAnimTime = 1f;
	public Text damageDisplay; // le display de la force d'attaque (joueur only)
	public int levelUpBonusDamage; // (joueur) combien de damage en plus si lvl up 
	public bool isAttacking; //suis je en train d'attaquer ? A sync !!!
	public GameObject target; // qui est ma cible ? a sync ! ! ! 
	public float rotSpeed = 5; // permet de tourner plus vite vers la cible. résoud un bug lié au fait que les objets étaient trop petit.
	private Vector3 targetTempPos; //calcul de position (privé)
	private GameObject targetObj; // l'objet qui t'attaque ! 
	public bool isActualizingPos;
	[SyncVar] public int bonusDamage;
	void Start()
	{

		agent = GetComponent<NavMeshAgent> ();
		anim = GetComponentInChildren<Animator> ();
		if (isLocalPlayer) 
		{
			agent.avoidancePriority = 75;
			damageDisplay = GameObject.Find ("DamageLog").GetComponent<Text> ();
			damageDisplay.text = damage.ToString ();
			attackRate = attackAnimTime / attackSpeedStat;
		}
		anim.SetFloat ("attackSpeed", attackSpeedStat);


	}

	void Update ()
	{
		if (isServer) 
		{
			if (target) 
			{
				if (!isAttacking) 
				{
					if (Vector3.Distance (transform.position, target.transform.position) <= attackRange) {

						RpcAttackTarget ();
					}
				} else 
				{
					if (Time.time > previousAttackTime) 
					{
						previousAttackTime = Time.time + attackRate;
						target.GetComponent<GenericLifeScript> ().LooseHealth (damage, false, gameObject);
					}
					if (Vector3.Distance (transform.position, target.transform.position) > attackRange || target.GetComponent<GenericLifeScript> ().isDead) 
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
				if (Vector3.Distance (targetTempPos, target.transform.position) > 0 && !isActualizingPos) 
				{
					if (Vector3.Distance (transform.position, target.transform.position) > attackRange) 
					{
						StartCoroutine (ActualizeTargetPos ());
					}
				}
			}
		}

		if (!agent.pathPending) 
		{
				if(agent.isOnNavMesh)
			{
					if (agent.remainingDistance <= agent.stoppingDistance + 0.5f) 
				{
						if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f) 
				{
							if (!stopWalk) 
						{
								stopWalk = true;
								anim.SetBool ("stopwalk", stopWalk);
								//audioSource.Stop ();
							}
						}
					}
				}
			}
	
	}

	[ClientRpc]
	public void RpcAttackTarget()
	{
		if (isLocalPlayer && !isServer) {
			CmdTellThemMyLocalPos (transform.position);
		}

		charge = false;
		anim.SetBool ("charge", false);
		if (agent.isOnNavMesh) {
			agent.velocity = Vector3.zero;
			agent.isStopped = true;
		}
		isAttacking = true;
		attackAnim = true;
		anim.SetBool ("attack", attackAnim);
		int AttSound = Random.Range (0, 10);
		if (AttSound > 5) {
			//audioSource.clip = playerSounds [1];
			GetComponent<AudioSource> ().PlayOneShot (Att1);
		}
			else
			//audioSource.clip = playerSounds [0];
			GetComponent<AudioSource> ().PlayOneShot (Att2);


	}
	[Command]
	public void CmdTellThemMyLocalPos(Vector3 pos)
	{
		transform.position = pos;
	}

	[ClientRpc]
	public void RpcStopAttacking()
	{
		if (target == null) {
			if (agent.isActiveAndEnabled) 
			{
				agent.SetDestination (transform.position);
			}
		}
		isAttacking = false;
		attackAnim = false;
		if (agent.isActiveAndEnabled) 
		{
			agent.isStopped = false;
		}
		//audioSource.Stop ();
		anim.SetBool ("attack", attackAnim);
		if (charge) 
		{
			//faire ici l'arret de la charge.
			charge = false;
			anim.SetBool ("charge", charge);
		}
	}

	public void AcquireTarget(GameObject newTarget)
	{
		target = newTarget;
		charge = true;
		anim.SetBool ("charge", charge);
		GetComponent<AudioSource> ().PlayOneShot (Charge);
	}

	public void LooseTarget()
	{
		target = null;
		isAttacking = false;
		attackAnim = false;
		if (agent.isOnNavMesh)
		{
			agent.isStopped = false;
		}
		//audioSource.Stop ();
		anim.SetBool ("attack", attackAnim);
		if (charge) 
		{
			//faire ici l'arret de la charge.
			charge = false;
			anim.SetBool ("charge", false);
		}
	}

	public void LevelUp()
	{
		damage += levelUpBonusDamage;
		if (isLocalPlayer) 
		{
			damageDisplay.text = damage.ToString ();
		}
	}

	public void ActualizeDamage(int dmg)
	{
		damage = dmg;
		if (isLocalPlayer) 
		{
			damageDisplay.text = damage.ToString ();

		}
	}
	IEnumerator ActualizeTargetPos()
	{
		isActualizingPos = true;
		agent.SetDestination (target.transform.position);
		targetTempPos = target.transform.position;
		yield return new WaitForSeconds (Random.Range( 0.10f, 0.20f));
		isActualizingPos = false;
	}
	public void ActualizeAttSpeed(float aS)
	{
		attackSpeedStat = aS;
		attackRate = attackAnimTime / attackSpeedStat;

		anim.SetFloat ("attackSpeed", aS);
	}
}
