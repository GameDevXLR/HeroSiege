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


    [Server]
	public void MakeHimCC(float CCDuration)
	{
        if (!GetComponent<EnnemyIGManager>().isDead)
        {
            RpcCCTheObject(CCDuration);
            GetComponent<EnemyAutoAttackScript>().GetCC(CCDuration);
        }
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

		//ajouter la désactivation de l'autoA;
		yield return new WaitForSecondsRealtime (CCTime);
		CCTwistImg.enabled = false;

		underCC = false;
	}

    [Server]
    public void MakeHimSlow(float dur)
	{
        if (!GetComponent<EnnemyIGManager>().isDead)
        {
            RpcSlowTheObject(dur);
        }
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

		//ajouter la désactivation de l'autoA;
		yield return new WaitForSecondsRealtime (SlowTime);
		GetComponent<NavMeshAgent> ().speed += 2f;

		SlowImg.enabled = false;

		underSlow = false;
	}

    [Server]
    public void MakeHimRoot(float rootDuration)
	{
        if (!GetComponent<EnnemyIGManager>().isDead)
        {
            StartCoroutine(RootProcedure(rootDuration));
        }
	}
	IEnumerator RootProcedure (float rootTime)
	{
		GetComponent<NavMeshAgent> ().isStopped = true;
		GetComponent<Rigidbody> ().velocity = Vector3.zero;
		yield return new WaitForSecondsRealtime (rootTime);
		GetComponent<NavMeshAgent> ().isStopped = false;
	}

}
