using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ennemiMover : MonoBehaviour {

	public float speed;
	private Rigidbody rb;

	void Start()
	{
		rb = GetComponent<Rigidbody> ();
	}

	void Update()
	{
		rb.AddForce (new Vector3 (5, 0.0f, 0.0f)*speed);
	}
}
