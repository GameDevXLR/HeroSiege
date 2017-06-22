using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour {

	public int zoomInLimite = 20;
	public int normal = 60;
	public int zoomOutLimite = 100;
	int zoom = 60;
	public float smooth = 5;

	void Update()
	{
		if(Input.GetAxis("Mouse ScrollWheel") > 0)
		{
			int variation = (int)(Input.GetAxis ("Mouse ScrollWheel") * 10);
			zoom = Mathf.Clamp (zoom - 10 * variation, zoomInLimite, zoomOutLimite);
		}
		else if(Input.GetAxis("Mouse ScrollWheel") < 0 )
		{
			int variation = (int)(Input.GetAxis ("Mouse ScrollWheel") * 10);
			zoom = Mathf.Clamp (zoom + 10, zoomInLimite, zoomOutLimite);

		}
		GetComponent<Camera> ().fieldOfView = Mathf.Lerp (GetComponent<Camera> ().fieldOfView, zoom, Time.deltaTime * smooth);
		float x = GetComponent<Camera> ().fieldOfView;
		transform.GetChild (0).localScale = new Vector3 (x, x, x);

	}

}
