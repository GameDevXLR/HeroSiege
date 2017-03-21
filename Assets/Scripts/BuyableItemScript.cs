using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyableItemScript : MonoBehaviour 
{
	//ce script permet d'acheter un objet du ItemManager.
	//il doit etre configurer manuellement.
	//a placer sur un bouton.

	public GameObject player;
	public int itemID;
	public int itemPrice;
	public Button thisBtn;
	public bool recquireSlot;

	void Start()
	{
		player = GameManager.instanceGM.playerObj;
	}

	public void BuyThatItem()
	{
		if (GameManager.instanceGM.playerObj.GetComponent<PlayerGoldScript> ().ActualGold >= itemPrice) 
		{
			StartCoroutine (DisableTheButtonTemporarily ());
			player = GameManager.instanceGM.playerObj;

			player.GetComponent<ItemManager> ().BuyItem (itemID, itemPrice, recquireSlot);
		}
	}

	IEnumerator DisableTheButtonTemporarily()
	{
		thisBtn.interactable = false;
		yield return new WaitForSeconds (1f);
		thisBtn.interactable = true;

	}
}
