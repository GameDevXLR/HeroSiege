using System.Collections;
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
	// our personnage
    public GameObject player; 

	// selectedPlayer
	// true : camera lock in the perso
	// false : camera free from the perso
    public bool selectedPlayer = false; 

	//speed move of the camera when move with mouse
    public int speed = 5;

	// detection zone of the mouse in the border
    public int zoneDetectionMouse = 100;

	// initial distance player / camera
    private Vector3 offset;

	// initial y, allow to block the y axis
    private float yvalue;

	Camera cameraCible;

    void Start()
    {
        offset = transform.position - player.transform.position;
        cameraCible = GetComponent<Camera>();
        yvalue = gameObject.transform.position.y;
    }

    void Update()
    {
		if (Input.GetKeyDown (KeyCode.L))
			selectedPlayer = !selectedPlayer;
		
		if (!Input.GetKey ("space") && !selectedPlayer) {
			UtilsScreenMovement.moveScreenWithMouse (cameraCible, zoneDetectionMouse, speed);
		}
        
    }

    void LateUpdate()
    {
		if (selectedPlayer || Input.GetKey ("space")) {
			transform.position = player.transform.position + offset;
		}

		// allow to block y axis
        gameObject.transform.position = new Vector3()
        {
            x = gameObject.transform.position.x,
            y = yvalue,
            z = gameObject.transform.position.z
        };
    }

}



