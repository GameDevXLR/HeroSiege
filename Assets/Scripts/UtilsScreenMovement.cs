/***
 * 
 * UtilsScreenMovement is a util class which will contain 
 * severall userfull function for the camera movement 
 * 
 * */


using UnityEngine;

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
	public static void moveScreenWithMouse(Camera cameraCible, int zoneDetectionMouse,  int speed)
    {
		float y = cameraCible.transform.position.y;
		// move to the left
        if (Input.mousePosition.x >= 0 && Input.mousePosition.x <= zoneDetectionMouse)
        {
			cameraCible.transform.Translate(new Vector3(-speed * Time.deltaTime, 0, 0));

        }
		//move to the right
        else if (Input.mousePosition.x <= Screen.width && Input.mousePosition.x >= Screen.width - zoneDetectionMouse)
        {
            cameraCible.transform.Translate(new Vector3(speed * Time.deltaTime, 0, 0));
        }

		//move backward
        if (Input.mousePosition.y >= 0 && Input.mousePosition.y <= zoneDetectionMouse)
        {
			
            cameraCible.transform.Translate(new Vector3(0, 0, -speed * Time.deltaTime));
			Vector3 destination = new Vector3 () {
				x = cameraCible.transform.position.x,
				y = y ,
				z = cameraCible.transform.position.z
			};
			cameraCible.transform.position = destination;

        }
		//move forward
        else if (Input.mousePosition.y <= Screen.height && Input.mousePosition.y >= Screen.height - zoneDetectionMouse)
        {
            cameraCible.transform.Translate(new Vector3(0, 0, speed * Time.deltaTime));
			Vector3 destination = new Vector3 () {
				x = cameraCible.transform.position.x,
				y = y ,
				z = cameraCible.transform.position.z
			};
			cameraCible.transform.position = destination;
        }
    }
}
