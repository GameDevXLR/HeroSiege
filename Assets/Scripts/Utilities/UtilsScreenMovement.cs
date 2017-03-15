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
	public static void moveScreenWithMouse(Camera cameraCible, int zoneDetectionMouse,  int speed,float yRef, int layer_mask)
    {

		float xValueDeplacement = 0;
		float zValueDeplacement = 0;

		// move to the left
        if (Input.mousePosition.x >= 0 && Input.mousePosition.x <= zoneDetectionMouse)
        {
			xValueDeplacement = -speed * Time.deltaTime;
        }
		//move to the right
        else if (Input.mousePosition.x <= Screen.width && Input.mousePosition.x >= Screen.width - zoneDetectionMouse)
        {
			xValueDeplacement = speed * Time.deltaTime;
        }

		//move backward
        if (Input.mousePosition.y >= 0 && Input.mousePosition.y <= zoneDetectionMouse)
        {
			zValueDeplacement = -speed * Time.deltaTime;

        }
		//move forward
        else if (Input.mousePosition.y <= Screen.height && Input.mousePosition.y >= Screen.height - zoneDetectionMouse)
        {
			zValueDeplacement = speed * Time.deltaTime;
        }

		if(xValueDeplacement != 0 || zValueDeplacement != 0){
			Vector3 destination = cameraCible.transform.position + cameraCible.transform.TransformDirection (new Vector3 (xValueDeplacement, 0, zValueDeplacement));
			Vector3 hitPoint = new Vector3();

			if(Utils.hadDetectTheLayer (destination, layer_mask, out hitPoint)){
				destination.y = hitPoint.y + yRef;
				cameraCible.transform.localPosition = Vector3.Lerp(cameraCible.transform.position, destination, speed * Time.deltaTime);
			}
		}

    }




}
