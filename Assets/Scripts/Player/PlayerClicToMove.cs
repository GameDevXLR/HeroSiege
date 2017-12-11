using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerClicToMove : NetworkBehaviour {


	//ce script est gaté. Ca c'est dit.
	// il gere le déplacement du joueur.
	public Animator anim;
	//private AudioSource audioS;
//	public AudioClip clicSound;
	public AudioClip walkSound;
	public NavMeshAgent agentPlayer;
	public PlayerAutoAttack attackScript;
	private PlayerEnnemyDetectionScript aggroArea;
	public GameObject target;
	public Vector3 targetTmpPos;
	public GameObject cursorTargetter;
	int layer_mask;
    public bool isInMiniMap = false;
    NetworkClient nClient;
	[SyncVar(hook = "ActualizePSpeed")]public float playerSpeed; // c'est pas link au nameshagent.speed attention!!! a corriger a l'occase. voir itemmanager qui fait appel pour le moment lors de l'achat des boots
	public Text speedDisplay;
	//	[SyncVar]public Vector3 startingPos; inutil now c'était pour les pnj...héritage d'un temps révolu.
	// Use this for initialization
	void Start () 
	{
		agentPlayer = GetComponent<NavMeshAgent> ();
		attackScript = GetComponent<PlayerAutoAttack> ();
		anim = GetComponentInChildren<Animator> ();

		if (isLocalPlayer) 
		{
			speedDisplay = GameObject.Find ("SpeedLog").GetComponent<Text> ();
			//audioS = GetComponent<AudioSource> ();
			layer_mask = LayerMask.GetMask ("Ground", "Ennemies", "UI");
			cursorTargetter = GameObject.Find ("ClickArrowFull");
			nClient = GameObject.Find ("NetworkManagerObj").GetComponent<NetworkManager> ().client;
		}
		if (isServer) 
		{
			aggroArea = GetComponentInChildren<PlayerEnnemyDetectionScript> ();

		}
		agentPlayer.speed = playerSpeed;
//		if (gameObject.tag == "PNJ" && startingPos == Vector3.zero) 
//		{
//			startingPos = gameObject.transform.position;
//		}
	}
	
	// Update is called once per frame
	void Update () {
       
		if ( isLocalPlayer) 
		{
            if (Input.GetMouseButtonUp(1) && !isInMiniMap)
            {
                bool next = true;
				if(GetComponent<PlayerAutoAttack>().particule)
				{
					GetComponent<PlayerAutoAttack> ().particule.gameObject.SetActive (false);
					GetComponent<PlayerAutoAttack> ().particule.gameObject.SetActive (true);;
				}


    //			audioS.PlayOneShot (clicSound, .6f);

			
			    Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
                int distance = 1;
                RaycastHit[] hits = Physics.RaycastAll(ray, 2000f, layer_mask); ;

                while (hits.Length > 0 && distance < 10 && hits[0].point.y >= CameraController.instanceCamera.yMinCamera) 
                {
                    distance++;
                    Vector3 origin = ray.origin + (ray.direction * distance);
                    ray = new Ray(origin, ray.direction);
                    hits = Physics.RaycastAll(ray, 2000f, layer_mask);
                }
                int i = 0;

                while(i < hits.Length && next)
                {
                    RaycastHit hit = hits[i++];
                    if (GameManager.instanceGM.gameObject.GetComponent<MouseManager> ().selectedObj) 
				    {
					    GameManager.instanceGM.gameObject.GetComponent<MouseManager> ().selectedObj.eraseRenderer = true;
					    GameManager.instanceGM.gameObject.GetComponent<MouseManager> ().selectedObj = null;
				    }
				     if(hit.collider.gameObject.layer == Layers.Ground )
				    {
					    StartCoroutine (MoveFirst ((float)nClient.GetRTT(), hit.point));
					    cursorTargetter.transform.position = hit.point;
    //					cursorTargetter.transform.position = new Vector3 (hit.point.x, 0.2f, hit.point.z);
					    cursorTargetter.GetComponent<Animator> ().Play ("ClickArrowAnim");
					    CmdSendNewDestination (hit.point);
					    CancelInvoke ();
    //					cursorTargetter.GetComponent<Animator> ().
					    Invoke ("StopThePosTargeter", 1.5f);
                        //					cursorTargetter.GetComponent<Animator> ().Play("ClickArrowAnim");
                        next = false;
				    }

			    }
            }

        }
		if (target)  
		{
			if(Vector3.Distance(targetTmpPos, target.transform.localPosition)>1f && !GetComponent<PlayerAutoAttack>().holdPosition)
			{
				targetTmpPos = target.transform.localPosition;
				agentPlayer.SetDestination (targetTmpPos);
			} 
		}


	}

    public void movePlayer(RaycastHit hit)
    {
        
        if (hit.collider.gameObject.layer == Layers.Ennemies)
        {
            //					aggroArea.autoTargetting = true;
            cursorTargetter.transform.position = Vector3.zero;
            target = hit.collider.gameObject;
            CmdSendNewTarget(target.GetComponent<NetworkIdentity>().netId);
			return;

        }
        else
        {
            StartCoroutine(MoveFirst((float)nClient.GetRTT(), hit.point));
            cursorTargetter.transform.position = hit.point;
            cursorTargetter.GetComponent<Animator>().Play("ClickArrowAnim");
            CmdSendNewDestination(hit.point);
            CancelInvoke();
            //					cursorTargetter.GetComponent<Animator> ().
            Invoke("StopThePosTargeter", 1.5f);
            //					cursorTargetter.GetComponent<Animator> ().Play("ClickArrowAnim");
        }
    }

	public void StopThePosTargeter()
	{
		cursorTargetter.transform.position = Vector3.zero;

	}
	IEnumerator MoveFirst(float ping, Vector3 desti)
	{
		yield return new WaitForSeconds (ping/2000); // la moitié donc (1/2) d'un truc en milliseconds (000)  :ca fait X/2000
		//audioS.clip = walkSound;
		//audioS.Play ();
		MovingProcedure (desti);
	}


	[Command]
	public void CmdSendNewDestination(Vector3 dest)
	{
		aggroArea.autoTargetting = false;
		RpcNewDestination (dest);
	}
	[ClientRpc]
	public void RpcNewDestination(Vector3 desti)
	{
		if (isLocalPlayer) 
		{
			return;
		}

		MovingProcedure (desti);
	}
	public void MovingProcedure(Vector3 dest)
	{
        if (agentPlayer.isOnNavMesh)
        {
			agentPlayer.isStopped = false;
            agentPlayer.SetDestination(dest);

        }

        target = null;
        agentPlayer.stoppingDistance = 1;
        attackScript.LooseTarget();
        anim.SetBool("stopwalk", false);
        attackScript.stopWalk = false;
    }

    public void ReceiveNewTarget(GameObject target)
    {
        cursorTargetter.transform.position = Vector3.zero;
        GameManager.instanceGM.gameObject.GetComponent<MouseManager>().selectedObj = target.GetComponent<EnnemyIGManager>().outlinemob;
        target.GetComponent<EnnemyIGManager>().outlinemob.eraseRenderer = false;
        CmdSendNewTarget(target.GetComponent<NetworkIdentity>().netId);
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
		agentPlayer.stoppingDistance = attackScript.attackRange;

		target = ClientScene.FindLocalObject (targetID);
//		agentPlayer.stoppingDistance = 1;
		attackScript.AcquireTarget (target);
		attackScript.holdPosition = false;
		anim.SetBool ("stopwalk", false);
		attackScript.stopWalk = false;
	}

	public void SetThatTargetFromAggro(NetworkInstanceId targetid)
	{
		if (GameManager.instanceGM.gameObject.GetComponent<MouseManager> ().selectedObj) 
		{
			GameManager.instanceGM.gameObject.GetComponent<MouseManager> ().selectedObj.eraseRenderer = true;
			GameManager.instanceGM.gameObject.GetComponent<MouseManager> ().selectedObj = null;
		}
		GameObject goTarg = ClientScene.FindLocalObject (targetid);
		GameManager.instanceGM.gameObject.GetComponent<MouseManager> ().selectedObj = goTarg.GetComponent<EnnemyIGManager> ().outlinemob;
		goTarg.GetComponent<EnnemyIGManager> ().outlinemob.eraseRenderer = false;
			SetTargetOnServer (targetid);

	}


	public void ActualizePSpeed(float sp)
	{
		playerSpeed = sp;
		agentPlayer.speed = playerSpeed;
		anim.SetFloat ("moveSpeed", playerSpeed);
		int speedTmp = (int)(sp * 100);
		if (isLocalPlayer) 
		{
			speedDisplay.text = speedTmp.ToString ();
		}
	}


    public void ReceiveStopMoving()
    {
        
        CmdStopMoving();
        anim.SetBool("stopwalk", true);
        attackScript.stopWalk = true;
        attackScript.LooseTarget();
        agentPlayer.isStopped = true;
        agentPlayer.velocity = Vector3.zero;
    }

    [Command]
    public void CmdStopMoving()
    {
        aggroArea.autoTargetting = false;
        RpcStopMoving();

    }

    [ClientRpc]
    public void RpcStopMoving()
    {
        if (isLocalPlayer)
        {
            return;
        }
        anim.SetBool("stopwalk", true);
        attackScript.stopWalk = true;
        attackScript.LooseTarget();
        agentPlayer.isStopped = true;
        agentPlayer.velocity = Vector3.zero;
    }

    public void ReceiveActiveAutoAttack( bool isAutoAAttack)
    {

        CmdActiveAutoAttack(isAutoAAttack);
    }

    [Command]
    public void CmdActiveAutoAttack(bool isAutoAAttack)
    {
        aggroArea.autoTargetting = isAutoAAttack;

    }

}

