using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[NetworkSettings(channel = 0, sendInterval = 0.1f)]

public class ChildrenHandlerForMob : NetworkBehaviour 
{
	private EnemyAutoAttackScript autoAScript;
	[SyncVar(hook= "SyncEnemy")] public NetworkInstanceId enemyID;
	public GameObject targetEnemy;
	void Start()
	{
		if (isServer) 
		{
			GetComponentInChildren<EnnemiAggroManagerScript> ().enabled = true;
			GetComponentInChildren<SphereCollider> ().enabled = true;
		}
		autoAScript = GetComponent<EnemyAutoAttackScript> ();
	}

	public void SetTheTarget(GameObject Go)
	{
		if (isServer) 
		{
			enemyID = Go.GetComponent<NetworkIdentity> ().netId;
//			RpcGetTarget ();
		}
	}

	public void LooseThatTarget()
	{
		RpcLooseTarget ();
	}

	[ClientRpc]
	public void RpcGetTarget( )
	{
		autoAScript.AcquireTarget (enemyID);

	}
	[ClientRpc]
	public void RpcLooseTarget()
	{
		autoAScript.LooseTarget ();

	}

	public void SyncEnemy(NetworkInstanceId id)
	{
		enemyID = id;
		autoAScript.AcquireTarget (enemyID);
	}

}
