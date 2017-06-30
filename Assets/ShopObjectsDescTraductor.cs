using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopObjectsDescTraductor : MonoBehaviour {

	public string objectDescFr;

	// Use this for initialization
	void Start () 
	{
		if (PlayerPrefs.GetString ("LANGAGE") == "Fr") 
		{
			GetComponent<Text> ().text = objectDescFr;
		}
		
	}

}
