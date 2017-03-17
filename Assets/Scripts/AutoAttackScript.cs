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
	public AudioClip[] playerSounds; //quel sons pour le joueur
	public AudioClip[] ennemiSounds; //quels sons pour les ennemis (tous)

	Animator anim; // l'animator qui gere les anim lié a ce script
	public bool stopWalk; //pour l animation : arrete de marcher
	bool charge; // animation et code : charge vers un ennemi / mob
	bool attackAnim; // dois je jouer l'animation d'attaque ? 
	public NavMeshAgent agent; // l'agent qui permet de déplacer l'objet attacher
	public float attackRange; // la portée des auto attaques
	public float attackRate; // le rate d'attaque par seconde
	private float previousAttackTime; // privé : le temps global de la derniere attaque
	public int damage; // combien de dégats brut (hors armure) on fait.
	public Text damageDisplay; // le display de la force d'attaque (joueur only)
	public int levelUpBonusDamage; // (joueur) combien de damage en plus si lvl up 
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
		if (isLocalPlayer) 
		{
			damageDisplay = GameObject.Find ("DamageLog").GetComponent<Text> ();
			damageDisplay.text = damage.ToString ();
		}

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
					if (gameObject.layer == 9) 
					{
						if (Vector3.Distance (targetTempPos, target.transform.position) > 0 && !isActualizingPos) {
							StartCoroutine (ActualizeTargetPos());
						}
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
		} else {
			LooseTarget ();
			if (isAttacking) 
			{
				StopAttacking ();
				if (gameObject.layer == 8) 
				{
					
					agent.SetDestination (transform.position);
					audioSource.Stop ();

				}
			}
		}

		if (gameObject.layer == 8) {
			if (!agent.pathPending) {
				if(agent.isOnNavMesh){
					if (agent.remainingDistance <= agent.stoppingDistance) {
						if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f) {
							if (!stopWalk) {
								stopWalk = true;
								anim.SetBool ("stopwalk", stopWalk);
								audioSource.Stop ();
							}
						}
					}
				}
			}
		}
	}
	IEnumerator ActualizeTargetPos()
		{
			isActualizingPos = true;
			agent.SetDestination (target.transform.position);
			targetTempPos = target.transform.position;
		yield return new WaitForSeconds (Random.Range( 0.5f, 0.7f));
			isActualizingPos = false;
		}

	public void AttackTheTarget()
	{
		
			agent.Stop ();

		isAttacking = true;
		attackAnim = true;
		if (gameObject.layer == 8) 
		{
			anim.SetBool ("attack", attackAnim);
			audioSource.clip = playerSounds [0];
			audioSource.Play();

		}
		if(gameObject.layer == 9 )
		{
			
				agent.enabled = false;
				GetComponent<NavMeshObstacle> ().enabled = true;

		anim.SetBool ("attackEnnemi", attackAnim);
			audioSource.clip = ennemiSounds [0];
			audioSource.Play();
		}
	}
	public void StopAttacking()
	{
		isAttacking = false;
		attackAnim = false;
		if (gameObject.layer == 8) {

				agent.Resume ();
			audioSource.Stop ();
			anim.SetBool ("attack", attackAnim);
		}
		if (gameObject.layer == 9) 
		{
	
				agent.enabled = true;
				GetComponent<NavMeshObstacle> ().enabled = false;
			audioSource.Stop ();
			anim.SetBool ("attackEnnemi", attackAnim);
		}

		if (gameObject.layer == 9) 
		{
			agent.Resume ();
			GetComponent<MinionsPathFindingScript> ().GoToEndGame ();

		}
	}
	public void AcquireTarget(GameObject newTarget)
	{
		target = newTarget;
		if (gameObject.layer == 9 ) 
		{
			agent.SetDestination (target.transform.position);
			targetTempPos = target.transform.position;
		}
		if (gameObject.layer == 8) 
		{
//			GetComponentInChildren<PlayerEnnemyDetectionScript> ().autoTargetting = false;
			charge = true;
			anim.SetBool ("charge", charge);
			audioSource.PlayOneShot (playerSounds [1], .6f);
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
	public void LevelUp()
	{
		damage += levelUpBonusDamage;
		if (isLocalPlayer) 
		{
			damageDisplay.text = damage.ToString ();
		}
	}
}
