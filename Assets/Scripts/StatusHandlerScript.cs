using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.Networking;

public class StatusHandlerScript : NetworkBehaviour 
{

	//ce script gere les différents statuts d'un personnage : CC  / root / saignement / slow etc...

	public bool underCC;
	public bool underSlow;
	public SpriteRenderer CCTwistImg;
	public SpriteRenderer SlowImg;
//	public SkinnedMeshRenderer objRenderer;
//	public Material stunMat;
//	public Material normalMat;
//	private GenericLifeScript lifeScript;
	// ajouter le script d'autoAttack.

	// Use this for initialization
//	void Start () 
//	{
//		lifeScript = GetComponent<GenericLifeScript> ();
//	}
	[Server]
	public void MakeHimCC(float CCDuration)
	{
//		if (underCC) 
//		{
//			return;
//		}
		RpcCCTheObject (CCDuration);
		GetComponent<EnemyAutoAttackScript> ().GetCC (CCDuration);
	}
	[ClientRpc]
	public void RpcCCTheObject(float duration)
	{
		StartCoroutine (CCprocedure (duration));
	}
	IEnumerator CCprocedure ( float CCTime)
	{
		CCTwistImg.enabled = true;

		underCC = true;
//		objRenderer.materials[1] = stunMat;
//		GetComponent<NavMeshAgent> ().Stop ();
//		GetComponent<Rigidbody> ().velocity = Vector3.zero;
		//ajouter la désactivation de l'autoA;
		yield return new WaitForSecondsRealtime (CCTime);
		CCTwistImg.enabled = false;

		underCC = false;
//		objRenderer.materials[1] = normalMat;
		//réactiver l'autoA;
//		GetComponent<NavMeshAgent> ().Resume ();
	}

	public void MakeHimSlow(float dur)
	{
		RpcSlowTheObject (dur);
	}
	[ClientRpc]
	public void RpcSlowTheObject(float duration)
	{
		StartCoroutine (SlowProcedure (duration));
	}
	IEnumerator SlowProcedure ( float SlowTime)
	{
		SlowImg.enabled = true;
		GetComponent<NavMeshAgent> ().speed -= 2f;
		underSlow = true;
		//		objRenderer.materials[1] = stunMat;
		//		GetComponent<NavMeshAgent> ().Stop ();
		//		GetComponent<Rigidbody> ().velocity = Vector3.zero;
		//ajouter la désactivation de l'autoA;
		yield return new WaitForSecondsRealtime (SlowTime);
		GetComponent<NavMeshAgent> ().speed += 2f;

		SlowImg.enabled = false;

		underSlow = false;
		//		objRenderer.materials[1] = normalMat;
		//réactiver l'autoA;
		//		GetComponent<NavMeshAgent> ().Resume ();
	}


	public void MakeHimRoot(float rootDuration)
	{
		StartCoroutine (RootProcedure(rootDuration));
	}
	IEnumerator RootProcedure (float rootTime)
	{
		GetComponent<NavMeshAgent> ().isStopped = true;
		GetComponent<Rigidbody> ().velocity = Vector3.zero;
		yield return new WaitForSecondsRealtime (rootTime);
		GetComponent<NavMeshAgent> ().isStopped = false;
	}

}
