using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TpMovement : MonoBehaviour {

	public float RotationSpeed;
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		transform.Rotate (new Vector3 (2, 4, 10) * RotationSpeed * Time.deltaTime);
	}
}
