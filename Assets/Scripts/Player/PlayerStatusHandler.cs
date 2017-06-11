using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AI;
public class PlayerStatusHandler : NetworkBehaviour {


	//ce script gere les différents statuts d'un hero: CC  / root / saignement / slow etc...

	public bool underCC;
	public bool underSlow;
	public SpriteRenderer CCTwistImg;
	public SpriteRenderer SlowImg;
	public NavMeshAgent agent;
	[Server]
	public void MakeHimCC(float CCDuration)
	{
		agent = GetComponent<NavMeshAgent> ();
		RpcCCTheObject (CCDuration);
		GetComponent<PlayerAutoAttack> ().GetCC (CCDuration);
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
		yield return new WaitForSecondsRealtime (CCTime);
		CCTwistImg.enabled = false;
		underCC = false;
	}

	public void MakeHimSlow(float dur)
	{
		if (!underSlow) 
		{
			RpcSlowTheObject (dur);
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
		agent.speed -= 2f;
		underSlow = true;
		yield return new WaitForSecondsRealtime (SlowTime);
		agent.speed += 2f;
		SlowImg.enabled = false;
		underSlow = false;
	}


	public void MakeHimRoot(float rootDuration)
	{
		StartCoroutine (RootProcedure(rootDuration));
	}
	IEnumerator RootProcedure (float rootTime)
	{
		agent.isStopped = true;
		GetComponent<Rigidbody> ().velocity = Vector3.zero;
		yield return new WaitForSeconds (rootTime);
		agent.isStopped = false;
	}

}
