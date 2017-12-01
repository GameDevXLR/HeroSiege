using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockUnlockCameraButton : MonoBehaviour {


	public void LockUnlock()
	{
		Camera.main.transform.GetComponent<CameraController> ().IsLocking ();
	}
}
