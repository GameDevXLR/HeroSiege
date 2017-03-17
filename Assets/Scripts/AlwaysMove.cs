using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class AlwaysMove : NetworkBehaviour 
{

	//ce script fait que l'objet auquel il est attaché va toujours bouger vers sa cible a sa vitesse.
	[SerializeField] float speed = 10;
	[SyncVar]public GameObject target;
	Vector3 targetpoint;

	void Update () 
	{

		targetpoint = target.transform.position;
		transform.position = Vector3.MoveTowards(transform.position, targetpoint, Time.deltaTime * speed);
	
	}
}
