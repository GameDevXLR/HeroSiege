using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryShortcut : MonoBehaviour 
{

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKeyDown (KeyCode.Alpha1) && transform.GetChild(0).transform.childCount != 0) 
		{
			transform.GetChild(0).transform.GetChild(0).GetComponent<Button> ().onClick.Invoke ();
		}
		if (Input.GetKeyDown (KeyCode.Alpha2) && transform.GetChild(1).transform.childCount != 0) 
		{
			transform.GetChild(1).transform.GetChild(0).GetComponent<Button> ().onClick.Invoke ();
		}
		if (Input.GetKeyDown (KeyCode.Alpha3) && transform.GetChild(2).transform.childCount != 0) 
		{
			transform.GetChild(2).transform.GetChild(0).GetComponent<Button> ().onClick.Invoke ();
		}
		if (Input.GetKeyDown (KeyCode.Alpha4) && transform.GetChild(3).transform.childCount != 0) 
		{
			transform.GetChild(3).transform.GetChild(0).GetComponent<Button> ().onClick.Invoke ();
		}
		if (Input.GetKeyDown (KeyCode.Alpha5) && transform.GetChild(4).transform.childCount != 0) 
		{
			transform.GetChild(4).transform.GetChild(0).GetComponent<Button> ().onClick.Invoke ();
		}
		if (Input.GetKeyDown (KeyCode.Alpha6) && transform.GetChild(5).transform.childCount != 0) 
		{
			transform.GetChild(5).transform.GetChild(0).GetComponent<Button> ().onClick.Invoke ();
		}
		if (Input.GetKeyDown (KeyCode.Alpha7) && transform.GetChild(6).transform.childCount != 0) 
		{
			transform.GetChild(6).transform.GetChild(0).GetComponent<Button> ().onClick.Invoke ();
		}
		if (Input.GetKeyDown (KeyCode.Alpha8) && transform.GetChild(7).transform.childCount != 0) 
		{
			transform.GetChild(7).transform.GetChild(0).GetComponent<Button> ().onClick.Invoke ();
		}
	}
}
