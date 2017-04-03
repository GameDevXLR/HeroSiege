using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.AI;

[NetworkSettings (channel = 3)]
public class NetMoveSync : NetworkBehaviour
{

	NavMeshAgent agent;
	private Vector3 previousPos;

	[SyncVar]
	Vector3 realPosition;
	[SyncVar]
	Quaternion realRotation;

	private float updateInterval;
	public bool syncRot = false;
	public float timeBtwTic = 0.11f; // maxi disons...utiliser pour les héros. mob a 0.3

	void Start()
	{
		realPosition = transform.position;
		agent = GetComponent<NavMeshAgent>();
	}

	void LateUpdate()
	{
		if (hasAuthority) 
		{

			updateInterval += Time.deltaTime;
			if (updateInterval > timeBtwTic) 
			{ 
	
					updateInterval = 0;
				if (Vector3.Distance (previousPos, transform.position) > 0.2f) 
				{
					previousPos = transform.position;
					CmdSyncPos (transform.position);
				}
				if (syncRot) 
				{
					CmdSyncRot (transform.rotation.y);
				}
			}

		} else {
			if (Vector3.Distance (transform.position, realPosition) > 6f) //si la distance est abuser : juste tp toi.
			{
				transform.position = realPosition;
//				Debug.Log ("bouger de force");
			}
			transform.position = Vector3.Lerp (transform.position, realPosition, 0.1f);
			if (syncRot) 
			{
				transform.rotation = Quaternion.Lerp (transform.rotation, realRotation, 0.1f);
			}
		}
	}

	[Command]
	void CmdSyncPos(Vector3 position)
	{
		realPosition = position;
	}
	[Command]
	void CmdSyncRot(float rotation)
	{
		Quaternion tmpRot = Quaternion.Euler (0f, rotation, 0f);
		realRotation = tmpRot;

	}
	}
