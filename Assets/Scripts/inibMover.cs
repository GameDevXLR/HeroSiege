using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inibMover : MonoBehaviour {

	public float speed;

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		transform.Rotate( new Vector3 (2, 4, 10) * speed * Time.deltaTime);
	}
}
