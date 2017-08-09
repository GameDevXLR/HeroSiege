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
		if (Input.GetKeyDown (CommandesController.Instance.getKeycode(CommandesEnum.Item1)) && transform.GetChild(0).transform.childCount != 0) 
		{
			transform.GetChild(0).transform.GetChild(0).GetComponent<Button> ().onClick.Invoke ();
		}
		if (Input.GetKeyDown (CommandesController.Instance.getKeycode(CommandesEnum.Item2)) && transform.GetChild(1).transform.childCount != 0) 
		{
			transform.GetChild(1).transform.GetChild(0).GetComponent<Button> ().onClick.Invoke ();
		}
		if (Input.GetKeyDown (CommandesController.Instance.getKeycode(CommandesEnum.Item3)) && transform.GetChild(2).transform.childCount != 0) 
		{
			transform.GetChild(2).transform.GetChild(0).GetComponent<Button> ().onClick.Invoke ();
		}
		if (Input.GetKeyDown (CommandesController.Instance.getKeycode(CommandesEnum.Item4)) && transform.GetChild(3).transform.childCount != 0) 
		{
			transform.GetChild(3).transform.GetChild(0).GetComponent<Button> ().onClick.Invoke ();
		}
		if (Input.GetKeyDown (CommandesController.Instance.getKeycode(CommandesEnum.Item5)) && transform.GetChild(4).transform.childCount != 0) 
		{
			transform.GetChild(4).transform.GetChild(0).GetComponent<Button> ().onClick.Invoke ();
		}
		if (Input.GetKeyDown (CommandesController.Instance.getKeycode(CommandesEnum.Item6)) && transform.GetChild(5).transform.childCount != 0) 
		{
			transform.GetChild(5).transform.GetChild(0).GetComponent<Button> ().onClick.Invoke ();
		}
		if (Input.GetKeyDown (CommandesController.Instance.getKeycode(CommandesEnum.Item7)) && transform.GetChild(6).transform.childCount != 0) 
		{
			transform.GetChild(6).transform.GetChild(0).GetComponent<Button> ().onClick.Invoke ();
		}
		if (Input.GetKeyDown (CommandesController.Instance.getKeycode(CommandesEnum.Item8)) && transform.GetChild(7).transform.childCount != 0) 
		{
			transform.GetChild(7).transform.GetChild(0).GetComponent<Button> ().onClick.Invoke ();
		}
	}
}
