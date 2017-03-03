using UnityEngine;

public static class UtilsScreenMovement
{
	public static void moveScreenWithMouse(Camera cameraCible,int zoneDetectionMouse,  int speed)
    {
        if (Input.mousePosition.x >= 0 && Input.mousePosition.x <= zoneDetectionMouse)
        {
            cameraCible.transform.Translate(new Vector3(-speed * Time.deltaTime, 0, 0));
        }
        else if (Input.mousePosition.x <= Screen.width && Input.mousePosition.x >= Screen.width - zoneDetectionMouse)
        {
            cameraCible.transform.Translate(new Vector3(speed * Time.deltaTime, 0, 0));
        }

        if (Input.mousePosition.y >= 0 && Input.mousePosition.y <= zoneDetectionMouse)
        {
            cameraCible.transform.Translate(new Vector3(0, 0, -speed * Time.deltaTime));
        }
        else if (Input.mousePosition.y <= Screen.height && Input.mousePosition.y >= Screen.height - zoneDetectionMouse)
        {
            cameraCible.transform.Translate(new Vector3(0, 0, speed * Time.deltaTime));
        }
    }
}
