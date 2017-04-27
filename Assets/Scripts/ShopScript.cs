using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopScript : MonoBehaviour 
{
	//ce script est a placé sur un collider trigger
	//il permet d'ouvrir un shop si on est a portée de l'objet qui a le script.
	//ATTENTION : il faut ecrire dans le shopName le nom exact du gameobject d'interface joueur lié a ce menu.
	//ATTENTION : un seul menu par shop. pour le moment...(a corriger)
	public Transform shopPanel;
	private bool isOpen;
	public string shopName;
	public bool isAccessible = true;
	public GameObject ShopIconObj;
	public Sprite openedShopImg;
	public Sprite closedShopImg;

	public void Start()
	{
		shopPanel = GameObject.Find (shopName).transform;
		shopPanel.gameObject.SetActive (false);

	}

	public void OpenYourMenu()
	{
		if (isAccessible) 
		{
			ShopIconObj.GetComponent<Image> ().sprite = openedShopImg;
		shopPanel.gameObject.SetActive (true);
		isOpen = true;
		}
	}
	public void CloseYourMenu()
	{
		ShopIconObj.GetComponent<Image> ().sprite = closedShopImg;
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
