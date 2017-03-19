using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

public class PlayerClicToMove : NetworkBehaviour {


	//ce script est gaté. Ca c'est dit.
	// il gere le déplacement du joueur.
	Animator anim;
	private AudioSource audioS;
//	public AudioClip clicSound;
	public AudioClip walkSound;
	public NavMeshAgent agentPlayer;
	public PlayerAutoAttack attackScript;
	private PlayerEnnemyDetectionScript aggroArea;
	public GameObject target;
	public Vector3 targetTmpPos;
	int layer_mask;
//	[SyncVar]public Vector3 startingPos;

	// Use this for initialization
	void Start () 
	{
		if (isLocalPlayer) 
		{
			audioS = GetComponent<AudioSource> ();
			layer_mask = LayerMask.GetMask ("Ground", "Ennemies");
		}
		if (isServer) 
		{
			aggroArea = GetComponentInChildren<PlayerEnnemyDetectionScript> ();

		}
		agentPlayer = GetComponent<NavMeshAgent> ();
		attackScript = GetComponent<PlayerAutoAttack> ();
		anim = GetComponentInChildren<Animator> ();
//		if (gameObject.tag == "PNJ" && startingPos == Vector3.zero) 
//		{
//			startingPos = gameObject.transform.position;
//		}
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetMouseButtonUp (1) && isLocalPlayer) 
		{
			
//			audioS.PlayOneShot (clicSound, .6f);

			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			if (Physics.Raycast (ray, out hit, 80f, layer_mask)) 
			{	
				if (hit.collider.gameObject.layer == Layers.Ennemies) 
				{
//					aggroArea.autoTargetting = true;
					target = hit.collider.gameObject;
					CmdSendNewTarget(target.GetComponent<NetworkIdentity> ().netId);

				} else 
				{
//					
					CmdSendNewDestination (hit.point);
					agentPlayer.SetDestination (hit.point);
				}

			}
		
		}
		if (target)  
		{
			if(Vector3.Distance(agentPlayer.destination, target.transform.position)>1.2f)
			{
				Debug.Log (Vector3.Distance (agentPlayer.destination, target.transform.position));
			targetTmpPos = target.transform.position;
			agentPlayer.SetDestination (targetTmpPos);
			} 
		}


	}
	[Command]
	public void CmdSendNewDestination(Vector3 dest)
	{
		aggroArea.autoTargetting = false;
		agentPlayer.SetDestination (dest);
		RpcNewDestination (dest);
	}
	[ClientRpc]
	public void RpcNewDestination(Vector3 desti)
	{
		agentPlayer.Resume ();
		if (!isLocalPlayer) 
		{
			agentPlayer.SetDestination (desti);

		}
		target = null;
		agentPlayer.stoppingDistance = 0;
		attackScript.LooseTarget ();
		anim.SetBool ("stopwalk", false);
		attackScript.stopWalk = false;
		if (isLocalPlayer) {
			audioS.clip = walkSound;
			audioS.Play ();
		}
	}

	[Command]
	public void CmdSendNewTarget(NetworkInstanceId targetID)
	{
		aggroArea.autoTargetting = true;
		RpcReceiveNewTarget (targetID);
	}
	public void SetTargetOnServer(NetworkInstanceId targetID)
	{
		aggroArea.autoTargetting = true;
		RpcReceiveNewTarget (targetID);
	}

	[ClientRpc]
	public void RpcReceiveNewTarget(NetworkInstanceId targetID)
	{
		agentPlayer.stoppingDistance = 2;

		target = ClientScene.FindLocalObject (targetID);
		agentPlayer.stoppingDistance = 1;
		attackScript.AcquireTarget (target);
		anim.SetBool ("stopwalk", false);
		attackScript.stopWalk = false;
	}

	public void SetThatTargetFromAggro(NetworkInstanceId targetid)
	{
		if (isServer) 
		{

			SetTargetOnServer (targetid);
			return;
		}
	}

}

