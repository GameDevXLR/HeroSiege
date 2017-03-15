using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopScript : MonoBehaviour 
{
	public Transform shopPanel;
	private bool isOpen;
	public string shopName;

	public void Start()
	{
		shopPanel = GameObject.Find (shopName).transform;
		shopPanel.gameObject.SetActive (false);

	}

	public void OpenYourMenu()
	{
		shopPanel.gameObject.SetActive (true);
		isOpen = true;
	}
	public void CloseYourMenu()
	{
		shopPanel.gameObject.SetActive (false);
		isOpen = false;
	}

	public void Update()
	{
		if(Input.GetKeyUp(KeyCode.B))
			{
			
			if (isOpen) 
			{
				CloseYourMenu ();
				return;
			} else 
			{
				OpenYourMenu ();
			}
			}
		if (isOpen) 
		{
			if (Vector3.Distance (gameObject.transform.position, GameManager.instanceGM.playerObj.transform.position) > 15f) 
			{
				CloseYourMenu ();
			}
		}
	}
}
