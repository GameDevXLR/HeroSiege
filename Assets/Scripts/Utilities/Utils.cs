using System;
using UnityEngine;

public static class Utils
{
	public static Boolean hadDetectTheLayer(Vector3 position, int layer_mask){


		RaycastHit hit;

		if (Physics.Raycast (position, -Vector3.up, out hit, layer_mask)) {	
			return true;
		}
		return false;
	}


}

