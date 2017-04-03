﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * 
 * class : CameraController
 * A script to controll the camera
 * 
 * */
public class CameraController : MonoBehaviour
{
	public static CameraController instanceCamera = null;
	// our personnage
	public GameObject target; 

	// key to lock and center the camera on the target
	public KeyCode lockKey = KeyCode.L;

	// key to center back the camera on the target
	public KeyCode centerBackKey = KeyCode.Space;

	// selectedPlayer
	// true : camera lock in the perso
	// false : camera free from the perso
    public bool selectedPlayer = true; 

	//speed move of the camera when move with mouse
    public int speed = 5;

	// detection zone of the mouse in the border
    public int zoneDetectionMouse = 100;

	// initial distance player / camera
	[SerializeField] Vector3 offset = new Vector3(3.7f,4.8f,0.2f);


	// initial y, allow to block the y axis
	private float yRef;

	// y difference use to move the y value
	private float yvalueDiff;

	private bool isReady;

	int layer_mask;

	Camera cameraCible;

	//on s'assure en Awake que le script est bien unique. sinon on détruit le nouvel arrivant.
	void Awake(){
		if (instanceCamera == null) {
			instanceCamera = this;
		} else if (instanceCamera != this) 
		{
			Destroy (gameObject);
		}
	}

	public void Initialize()
    {
        cameraCible = GetComponent<Camera>();
        yRef = gameObject.transform.position.y;
		isReady = true;
		layer_mask = LayerMask.GetMask ("Ground"); // ground layer 10
    }

    void Update()
    {
		if (!isReady) 
		{
			return;
		}
		if (Input.GetKeyUp (lockKey))
			selectedPlayer = !selectedPlayer;
		if (!Input.GetKey (centerBackKey) && !selectedPlayer) {
			UtilsScreenMovement.moveScreenWithMouse (cameraCible, zoneDetectionMouse, speed, layer_mask);
		}
    }

    void LateUpdate()
    {
		if (!isReady) 
		{
			return;
		}
		if (selectedPlayer || Input.GetKey (centerBackKey)) {
			CenterBackCameraOnTarget ();
		

			// allow to block y axis
			gameObject.transform.position = new Vector3 () {
				x = gameObject.transform.position.x,
				y = yRef,
				z = gameObject.transform.position.z
			};
		} 
    }


	public void CenterBackCameraOnTarget()
	{
		if (target != null) {
			transform.position = target.transform.position + offset;
		}
	}

    public void MoveCameraTo(Vector3 vect)
    {
        if (vect != null)
        {
            transform.position = vect + offset;
            gameObject.transform.position = new Vector3()
            {
                x = gameObject.transform.position.x,
                y = yRef,
                z = gameObject.transform.position.z
            };
        }
    }

    public void LockUnlockCamera()
	{
		selectedPlayer = !selectedPlayer;

	}

}



