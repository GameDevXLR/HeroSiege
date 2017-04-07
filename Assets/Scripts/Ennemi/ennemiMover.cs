using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ennemiMover : MonoBehaviour {


	//bouge l'objet vers l'avant en appliquant une force; a condition que la velocité de l'objet ne dépasse pas maxSpeed.
	public float speed;
	public float maxSpeed;
	private Rigidbody rb;

	void Start()
	{
		rb = GetComponent<Rigidbody> ();
	}

	void Update()
	{ 
		if (rb.velocity.magnitude < maxSpeed) {
			rb.AddForce (new Vector3 (5, 0.0f, 0.0f) * speed);
		}
	}
}
