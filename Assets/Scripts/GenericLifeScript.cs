using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GenericLifeScript : MonoBehaviour {

	// ce script sert a gerer la vie de l'objet auquel il est attacher. en cas de mort; l'objet est détruit sauf si c'est un joueur : dans ce cas faut écrire le code pour le moment c'est pas préciser...

	public int maxHp = 1000;
	public int currentHp = 800;
	public int regenHp;
	public Transform respawnTransform; // placer ici un transform qui correspond a l'endroit ou doit respawn l'objet.

	public int armorScore = 1;


	public bool isDead;
	private float lastTic;
	public float timeBetweenTic = 1f;

	void Start () {
		lastTic = 0f;
	}

	void Update () {

		if (isDead == false) {

			if (Time.time > lastTic) {
				lastTic = Time.time + timeBetweenTic;
				RegenYourHP ();
			}


			if (currentHp > maxHp) {
				currentHp = maxHp;
			}

			if (currentHp < 0) {
				currentHp = 0;
			}
			if (currentHp == 0) {
				isDead = true;
				MakeHimDie ();
			}
		}
	}

	public void LooseHealth(int dmg, bool trueDmg)
	{		StartCoroutine(HitAnimation());
		
		if (currentHp > 0) {
			if (trueDmg) {
				currentHp -= dmg;
				return;
			} 
			if (armorScore > 0) {
				float multiplicatorArmor = (float) 100f / (100f + armorScore);
				currentHp -= (int)Mathf.Abs (dmg * multiplicatorArmor);
				return;
			}else
			{
				currentHp -= dmg;
			}
		}
	}
	public void	RegenYourHP ()
	{
		currentHp += regenHp;
	}
	public void MakeHimDie ()
	{
		if (gameObject.layer == 8) 
		{
			//faire ici ce qui se passe pour un joueur qui meurt
			PlayerRespawnProcess();
			return;
		}
		Destroy (gameObject); // sinon ca détruit l'objet tout simplement.
	}

	public void PlayerRespawnProcess(){
		StartCoroutine (RespawnEnum ());
	}
		IEnumerator RespawnEnum()
	{
		GetComponentInChildren<PlayerEnnemyDetectionScript> ().autoTargetting = false;
		GetComponent<AutoAttackScript> ().enabled = false;
		GetComponentInChildren<SkinnedMeshRenderer> ().enabled = false;
		GetComponent<PlayerClicToMove> ().enabled = false;
		GetComponent<NavMeshAgent> ().SetDestination (transform.position);
		GetComponent<CapsuleCollider> ().enabled = false;
		yield return new WaitForSeconds (0.8f);
		GetComponent<Renderer> ().enabled = false;
		Debug.Log ("5sec before respawn");
		yield return new WaitForSeconds (4.2f);
		GetComponent<NavMeshAgent> ().SetDestination (respawnTransform.position);
		GetComponentInChildren<SkinnedMeshRenderer> ().enabled = true;
		GetComponent<PlayerClicToMove> ().enabled = true;
		GetComponent<CapsuleCollider> ().enabled = true;
		GetComponentInChildren<PlayerEnnemyDetectionScript> ().autoTargetting = true;
		GetComponent<AutoAttackScript> ().enabled = true;

		gameObject.transform.position = respawnTransform.position;
		gameObject.transform.rotation = respawnTransform.rotation;
		currentHp = maxHp;
		isDead = false;
		}
	IEnumerator HitAnimation()
	{
		GetComponentInChildren<SkinnedMeshRenderer> ().enabled = false;
		yield return new WaitForSeconds (0.1f);
		GetComponentInChildren<SkinnedMeshRenderer> ().enabled = true;
		yield return new WaitForSeconds (0.1f);
		GetComponentInChildren<SkinnedMeshRenderer> ().enabled = false;
		yield return new WaitForSeconds (0.1f);
		GetComponentInChildren<SkinnedMeshRenderer> ().enabled = true;
		yield return new WaitForSeconds (0.1f);
		GetComponentInChildren<SkinnedMeshRenderer> ().enabled = false;
		yield return new WaitForSeconds (0.1f);
		GetComponentInChildren<SkinnedMeshRenderer> ().enabled = true;
	}
}
