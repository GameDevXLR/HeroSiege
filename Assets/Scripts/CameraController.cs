using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraController : MonoBehaviour
{
    public GameObject player;
    public bool selectedPlayer = false;
    public int speed = 1;
    public int zoneDetectionMouse = 100;
    private Vector3 offset;
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
		if (Input.GetKey (KeyCode.L))
			selectedPlayer = !selectedPlayer;
		if (!Input.GetKey ("space") && !selectedPlayer) {
			UtilsScreenMovement.moveScreenWithMouse (cameraCible, zoneDetectionMouse, speed);
//			if (Input.mousePosition.x >= 0 && Input.mousePosition.x <= zoneDetectionMouse) {
//				transform.Translate (new Vector3 (-speed * Time.deltaTime, 0, 0));
//			} else if (Input.mousePosition.x <= Screen.width && Input.mousePosition.x >= Screen.width - zoneDetectionMouse) {
//				transform.Translate (new Vector3 (speed * Time.deltaTime, 0, 0));
//			}
//
//			if (Input.mousePosition.y >= 0 && Input.mousePosition.y <= zoneDetectionMouse) {
//				transform.Translate (new Vector3 (0, 0, -speed * Time.deltaTime));
//			} else if (Input.mousePosition.y <= Screen.height && Input.mousePosition.y >= Screen.height - zoneDetectionMouse) {
//				transform.Translate (new Vector3 (0, 0, speed * Time.deltaTime));
//			}
		}
        
    }

    void LateUpdate()
    {
		if (selectedPlayer || Input.GetKey ("space")) {
			transform.position = player.transform.position + offset;
		}
        gameObject.transform.position = new Vector3()
        {
            x = gameObject.transform.position.x,
            y = yvalue,
            z = gameObject.transform.position.z
        };
    }

}



