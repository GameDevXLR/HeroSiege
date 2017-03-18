using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[NetworkSettings(channel = 2, sendInterval = 0.5f)]

public class ChildrenHandlerForMob : NetworkBehaviour 
{
	private EnemyAutoAttackScript autoAScript;

	void Start()
	{
		if (!isServer) 
		{
			GetComponentInChildren<EnnemiAggroManagerScript> ().enabled = false;
			GetComponentInChildren<SphereCollider> ().enabled = false;
		}
		autoAScript = GetComponent<EnemyAutoAttackScript> ();
	}

	public void SetTheTarget(GameObject Go)
	{
		RpcGetTarget (Go.GetComponent<NetworkIdentity> ().netId);
	}

	public void LooseThatTarget()
	{
		RpcLooseTarget ();
	}

	[ClientRpc]
	public void RpcGetTarget( NetworkInstanceId id)
	{
		autoAScript.AcquireTarget (ClientScene.FindLocalObject(id));

	}
	[ClientRpc]
	public void RpcLooseTarget()
	{
		autoAScript.LooseTarget ();

	}

}
