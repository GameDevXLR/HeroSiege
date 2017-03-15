using System;
using UnityEngine;

public static class Utils
{
	// Permet de détecter si un point est au dessus d'un layer particuler
	public static Boolean hadDetectTheLayer(Vector3 position, int layer_mask){
		RaycastHit hit;
		if (Physics.Raycast (position, -Vector3.up, out hit, layer_mask)) {	
			//Debug.DrawLine(position, new Vector3(position.x, position.y - 50, position.z), Color.green);
			return true;
		}
		//Debug.DrawLine(position, new Vector3(position.x, position.y - 50, position.z), Color.red);
		if (Physics.Raycast (position, Vector3.up, out hit, layer_mask)) {	
			//Debug.DrawLine(position, new Vector3(position.x, position.y - 50, position.z), Color.green);
			return true;
		}
		return false;
	}

	/*** 
	 * Permet de détecter si un point est au dessus d'un layer particuler
	 * \param position : position initial
	 * \param layer_mask : layer visé
	 * \param  out hitPoint : permet d'avoir les valeur du point d'intersection entre le raycast et le layer ground
	 * */
	public static Boolean hadDetectTheLayer(Vector3 position, int layer_mask, out Vector3 hitPoint){
		RaycastHit hit;
		if (Physics.Raycast (position, -Vector3.up, out hit, layer_mask)) {	
			//Debug.DrawLine(position, new Vector3(position.x, position.y - 50, position.z), Color.green);
			hitPoint = hit.point;
			return true;
		}
		//Debug.DrawLine(position, new Vector3(position.x, position.y - 50, position.z), Color.red);
		if (Physics.Raycast (position, Vector3.up, out hit, layer_mask)) {	
			//Debug.DrawLine(position, new Vector3(position.x, position.y - 50, position.z), Color.green);
			hitPoint = hit.point;

			return true;
		}
		hitPoint = new Vector3 ();
		return false;
	}
}

