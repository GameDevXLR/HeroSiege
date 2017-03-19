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
	private AudioSource audioSource; // qui joue le son
	public AudioClip[] playerSounds; //quel sons pour le joueur

	Animator anim; // l'animator qui gere les anim lié a ce script
	public bool stopWalk; //pour l animation : arrete de marcher
	bool charge; // animation et code : charge vers un ennemi / mob
	bool attackAnim; // dois je jouer l'animation d'attaque ? 
	public NavMeshAgent agent; // l'agent qui permet de déplacer l'objet attacher
	public float attackRange; // la portée des auto attaques
	public float attackRate; // le rate d'attaque par seconde
	private float previousAttackTime; // privé : le temps global de la derniere attaque
	[SyncVar(hook = "ActualizeDamage")]public int damage; // combien de dégats brut (hors armure) on fait.
	public Text damageDisplay; // le display de la force d'attaque (joueur only)
	public int levelUpBonusDamage; // (joueur) combien de damage en plus si lvl up 
	public bool isAttacking; //suis je en train d'attaquer ? A sync !!!
	public GameObject target; // qui est ma cible ? a sync ! ! ! 
	public float rotSpeed = 5; // permet de tourner plus vite vers la cible. résoud un bug lié au fait que les objets étaient trop petit.
	private Vector3 targetTempPos; //calcul de position (privé)
	private GameObject targetObj; // l'objet qui t'attaque ! 
	public bool isActualizingPos;

	void Start()
	{

		agent = GetComponent<NavMeshAgent> ();
		anim = GetComponentInChildren<Animator> ();
		audioSource = GetComponent<AudioSource> ();
		if (isLocalPlayer) 
		{
			damageDisplay = GameObject.Find ("DamageLog").GetComponent<Text> ();
			damageDisplay.text = damage.ToString ();
		}

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
					if (agent.remainingDistance <= agent.stoppingDistance) 
				{
						if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f) 
				{
							if (!stopWalk) 
						{
								stopWalk = true;
								anim.SetBool ("stopwalk", stopWalk);
								audioSource.Stop ();
							}
						}
					}
				}
			}
	
	}

	[ClientRpc]
	public void RpcAttackTarget()
	{
		if (isLocalPlayer && !isServer) 
		{
			CmdTellThemMyLocalPos (transform.position);
		}

		charge = false;
		anim.SetBool ("charge", false);
		if (agent.isOnNavMesh ) 
		{
			agent.Stop ();
		}
		isAttacking = true;
		attackAnim = true;
		anim.SetBool ("attack", attackAnim);
		audioSource.clip = playerSounds [0];
		audioSource.Play();
	}
	[Command]
	public void CmdTellThemMyLocalPos(Vector3 pos)
	{
		transform.Translate( pos);
	}

	[ClientRpc]
	public void RpcStopAttacking()
	{
		if (target == null) 
		{
			agent.SetDestination (transform.position);
		}
		isAttacking = false;
		attackAnim = false;
		agent.Resume ();
		audioSource.Stop ();
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
		audioSource.PlayOneShot (playerSounds [1], .6f);
	}

	public void LooseTarget()
	{
		target = null;
		isAttacking = false;
		attackAnim = false;
		if (agent.isOnNavMesh)
		{
			agent.Resume ();
		}
		audioSource.Stop ();
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
		yield return new WaitForSeconds (Random.Range( 0.1f, 0.2f));
		isActualizingPos = false;
	}
}
