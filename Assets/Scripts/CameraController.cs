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
    public bool selectedPlayer = false; 

	//speed move of the camera when move with mouse
    public int speed = 5;

	// detection zone of the mouse in the border
    public int zoneDetectionMouse = 100;

	// initial distance player / camera
	[SerializeField] Vector3 offset = new Vector3(3.7f,4.8f,0.2f);


	// initial y, allow to block the y axis
	private float yvalue;

	// y difference use to move the y value
	private float yvalueDiff;

	private bool isReady;

	int layer_mask;

	Camera cameraCible;

	// limite map
	public Vector3 boundaries = new Vector3(10,10,10);


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
        yvalue = gameObject.transform.position.y;
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

			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay (new Vector3 (gameObject.transform.position.x, 0, gameObject.transform.position.z));
			if (Physics.Raycast (ray, out hit, 50f, layer_mask)) {	
				yvalueDiff = hit.point.y - target.transform.position.y;
			}

			UtilsScreenMovement.moveScreenWithMouse (cameraCible, zoneDetectionMouse, speed, boundaries, yvalueDiff, layer_mask);
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
				y = yvalue,
				z = gameObject.transform.position.z
			};
		} else {
			
			Vector3 destination = new Vector3 () {
				x = gameObject.transform.position.x,
				y = yvalue + yvalueDiff ,
				z = gameObject.transform.position.z
			};
					
			gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, destination, Time.deltaTime);

		}
    }


	public void CenterBackCameraOnTarget(){
		transform.position = target.transform.position + offset;
	}


}



