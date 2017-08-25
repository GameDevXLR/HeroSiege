using UnityEngine;
using System.Collections;


public class FaceCamera : MonoBehaviour {

	public bool rotationActivated = true;

	void LateUpdate() {

			this.transform.LookAt (Camera.main.transform.position);
		if(rotationActivated){
			this.transform.Rotate (new Vector3 (0, 180, 0));
		}
	}
}
