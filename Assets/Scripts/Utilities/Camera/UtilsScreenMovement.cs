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
	public static void moveScreenWithMouse(Camera cameraCible, int zoneDetectionMouse,  int speed, int layer_mask)
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
				destination.y = cameraCible.transform.position.y;
				cameraCible.transform.localPosition = Vector3.Lerp(cameraCible.transform.position, destination, speed * Time.deltaTime);
			}
		}

    }

    public static void moveScreenWithMouse(Camera cameraCible, CameraBoundaries boundaries, int zoneDetectionMouse, int speed, int layer_mask)
    {

        float yValue = (Input.mousePosition.y - (Screen.height / 2)) / (Screen.height / 2);
        float xValue = (Input.mousePosition.x - (Screen.width / 2)) / (Screen.width / 2);
        float zoneDectectWidth = (float)zoneDetectionMouse / Screen.width;
        float zoneDectectHeight = (float)zoneDetectionMouse / Screen.height;

        float xValueDeplacement = 0;
        float zValueDeplacement = 0;

        if((xValue <= 1 && xValue >= 1 - zoneDectectWidth) 
            || (xValue >= -1 && xValue <= -1 + zoneDectectWidth)
            || (yValue <= 1 && yValue >= 1 - zoneDectectHeight)
            || (yValue >= -1 && yValue <= -1 + zoneDectectHeight))
        {
            if ((xValue > 0 && cameraCible.transform.position.z <= boundaries.getEst())
                || (xValue < 0 && cameraCible.transform.position.z >= boundaries.getOuest()))
            {
                xValueDeplacement = speed * Time.deltaTime * xValue;
            }

            if ((yValue > 0 && cameraCible.transform.position.x >= boundaries.getNord())
                || (yValue < 0 && cameraCible.transform.position.x <= boundaries.getSud()))
            {
                zValueDeplacement = speed * Time.deltaTime * yValue;
            }
        }

        if (xValueDeplacement != 0 || zValueDeplacement != 0)
        {
           // Debug.Log("CameraCible position : " + cameraCible.transform.position);
            Vector3 destination = cameraCible.transform.position + cameraCible.transform.TransformDirection(new Vector3(xValueDeplacement, 0, zValueDeplacement));
            destination.y = cameraCible.transform.position.y;

            Vector3 hitPointDest = new Vector3();
            Vector3 hitPointDepart = new Vector3();

            if (Utils.hadDetectTheLayer(destination, layer_mask, out hitPointDest) && Utils.hadDetectTheLayer(cameraCible.transform.position, layer_mask, out hitPointDepart))
            {
                destination.y = cameraCible.transform.position.y - (hitPointDepart.y - hitPointDest.y);
            }
            cameraCible.transform.localPosition = Vector3.Lerp(cameraCible.transform.position, destination, speed * Time.deltaTime);

        }

    }




}
