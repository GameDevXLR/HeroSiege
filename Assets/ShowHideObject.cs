using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowHideObject : MonoBehaviour {


	public GameObject objectToHide;

	public void ShowHideObjectDesired()
	{
		objectToHide.SetActive (!objectToHide.activeInHierarchy);
	}
}
