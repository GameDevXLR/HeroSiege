/***
 * 
 * UtilsScreenMovement is a util class which will contain 
 * severall userfull function for the camera movement 
 * 
 * */


using UnityEngine;
using System;
using System.Collections.Generic; 

public static class UtilsScreenMovement
{
	/***
	 * 
	 * move the screen depend of the mouse position 
	 * param Camera => camera target
	 * param zoneDetectionMouse => detection zone in the border
	 * param speed 
	 * 
	 * 
	 * */
	public static void moveScreenWithMouse(Camera cameraCible, int zoneDetectionMouse,  int speed, Vector3 boundaries, float yDiff, int layer_mask)
    {

		float yInitial = cameraCible.transform.position.y;
		// move to the left
        if (Input.mousePosition.x >= 0 && Input.mousePosition.x <= zoneDetectionMouse)
        {
			Vector3 destination = cameraCible.transform.position + cameraCible.transform.TransformDirection (new Vector3 (-speed * Time.deltaTime, 0, 0));

			if (Utils.hadDetectTheLayer ( destination, layer_mask)) 
				cameraCible.transform.position = Vector3.Lerp(cameraCible.transform.position, destination, speed * Time.deltaTime);


        }
		//move to the right
        else if (Input.mousePosition.x <= Screen.width && Input.mousePosition.x >= Screen.width - zoneDetectionMouse)
        {
			Vector3 destination = cameraCible.transform.position + cameraCible.transform.TransformDirection (new Vector3 (speed * Time.deltaTime, 0, 0));

			if (Utils.hadDetectTheLayer ( destination,  layer_mask)) 
				cameraCible.transform.position = Vector3.Lerp(cameraCible.transform.position, destination, speed * Time.deltaTime);
        }

		//move backward
        if (Input.mousePosition.y >= 0 && Input.mousePosition.y <= zoneDetectionMouse)
        {
			Vector3 destination = cameraCible.transform.position + cameraCible.transform.TransformDirection (-Vector3.forward * speed * Time.deltaTime);

			if(Utils.hadDetectTheLayer(destination,  layer_mask)){
				cameraCible.transform.position = Vector3.Lerp(cameraCible.transform.position, destination, speed * Time.deltaTime);

			}

        }
		//move forward
        else if (Input.mousePosition.y <= Screen.height && Input.mousePosition.y >= Screen.height - zoneDetectionMouse)
        {
			Vector3 destination = cameraCible.transform.position + cameraCible.transform.TransformDirection (Vector3.forward * speed * Time.deltaTime);

			if(Utils.hadDetectTheLayer(destination, layer_mask)){

				cameraCible.transform.localPosition = Vector3.Lerp(cameraCible.transform.position, destination, speed * Time.deltaTime);

			}
        }
		cameraCible.transform.position = new Vector3 () {
			x = cameraCible.transform.position.x,
			y = yInitial,
			z = cameraCible.transform.position.z
		};

    }




}
