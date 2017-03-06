using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ChildrenHandlerForMob : NetworkBehaviour 
{
	private AutoAttackScript autoAScript;

	void Start()
	{
		if (isServer) 
		{
			GetComponentInChildren<EnnemiAggroManagerScript> ().enabled = true;
		}
		autoAScript = GetComponent<AutoAttackScript> ();
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
