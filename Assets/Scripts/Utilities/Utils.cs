using System;
using UnityEngine;

public static class Utils
{
	public static Boolean hadDetectTheLayer(Vector3 position, int layer_mask){


		RaycastHit hit;
		if (Physics.Raycast (position, -Vector3.up, out hit, layer_mask)) {	
			Debug.DrawLine(position, new Vector3(position.x, position.y - 50, position.z), Color.green);
			return true;
		}
		Debug.DrawLine(position, new Vector3(position.x, position.y - 50, position.z), Color.red);

		return false;
	}


}

