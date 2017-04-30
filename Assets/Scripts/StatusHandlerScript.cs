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
	public SpriteRenderer CCTwistImg;
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
		yield return new WaitForSeconds (CCTime);
		CCTwistImg.enabled = false;

		underCC = false;
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
		yield return new WaitForSeconds (rootTime);
		GetComponent<NavMeshAgent> ().isStopped = false;
	}

}
