using UnityEngine;
using System.Collections;


public class FaceCamera : MonoBehaviour {

	public bool rotationActivated = true;
	public Transform cam;
	Vector3 standardPos = new Vector3 (0, 180, 0);

	void Start()
	{
		cam = Camera.main.transform;
		}
	void LateUpdate() {

			this.transform.LookAt (cam.position);
		if(rotationActivated){
			this.transform.Rotate (standardPos);
		}
	}
}
