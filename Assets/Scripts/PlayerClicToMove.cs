using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

public class PlayerClicToMove : NetworkBehaviour {

	Animator anim;
	private AudioSource audioS;
//	public AudioClip clicSound;
	public AudioClip walkSound;
	public NavMeshAgent agentPlayer;
	public AutoAttackScript attackScript;
	private PlayerEnnemyDetectionScript aggroArea;
	public GameObject target;
	int layer_mask;

	// Use this for initialization
	void Start () 
	{
		if (isLocalPlayer) 
		{
			audioS = GetComponent<AudioSource> ();
			layer_mask = LayerMask.GetMask ("Ground", "Ennemies");
			aggroArea = GetComponentInChildren<PlayerEnnemyDetectionScript> ();
		}
		agentPlayer = GetComponent<NavMeshAgent> ();
		attackScript = GetComponent<AutoAttackScript> ();
		anim = GetComponentInChildren<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetMouseButtonUp (1) && isLocalPlayer) 
		{
			
//			audioS.PlayOneShot (clicSound, .6f);

			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			if (Physics.Raycast (ray, out hit, 50f, layer_mask)) 
			{	


				if (hit.collider.gameObject.layer == 9) 
				{
					aggroArea.autoTargetting = true;
					target = hit.collider.gameObject;
					CmdSendNewTarget(target.GetComponent<NetworkIdentity> ().netId);

				} else 
				{
					aggroArea.autoTargetting = false;
					CmdSendNewDestination (hit.point);

				}

			}
		
		}
		if (target) 
		{
			agentPlayer.SetDestination (target.transform.position);
		}

	}
	[Command]
	public void CmdSendNewDestination(Vector3 dest)
	{
		agentPlayer.SetDestination (dest);
		RpcNewDestination (dest);
	}
	[ClientRpc]
	public void RpcNewDestination(Vector3 desti)
	{

			agentPlayer.SetDestination (desti);
			target = null;
			agentPlayer.stoppingDistance = 0;
			attackScript.LooseTarget ();
			anim.SetBool ("stopwalk", false);
			attackScript.stopWalk = false;
		audioS.clip = walkSound;
		audioS.Play ();
	}

	[Command]
	public void CmdSendNewTarget(NetworkInstanceId targetID)
	{
		target = ClientScene.FindLocalObject (targetID);

		RpcReceiveNewTarget (targetID);
	}
	[ClientRpc]
	public void RpcReceiveNewTarget(NetworkInstanceId targetID)
	{

		target = ClientScene.FindLocalObject (targetID);
		agentPlayer.stoppingDistance = 1;
		attackScript.AcquireTarget (target);
		anim.SetBool ("stopwalk", false);
		attackScript.stopWalk = false;
	}

}

