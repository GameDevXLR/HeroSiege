using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TpMovement : MonoBehaviour {

	public float RotationSpeed;
	private Quaternion rot;
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		transform.rotation = rot;
		transform.Rotate (Vector3.forward * RotationSpeed * Time.deltaTime);
		rot = transform.rotation;
	}
}
