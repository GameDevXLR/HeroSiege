using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class AlwaysMove : NetworkBehaviour {
	[SerializeField] float speed = 5;
	[SyncVar]public GameObject target;
	Vector3 targetpoint;

	void Update () 
	{

		targetpoint = target.transform.position;
		transform.position = Vector3.MoveTowards(transform.position, targetpoint, Time.deltaTime * speed);
	
	}
}
