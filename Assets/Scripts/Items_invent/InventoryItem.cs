using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItem : MonoBehaviour
{
	public int sellingPrice = 1;
	public virtual void SellBackItem()
	{
		GameManager.instanceGM.playerObj.GetComponent<ItemManager> ().AskForRefund (sellingPrice);
		GameObject.Destroy (gameObject, 0.1f);
	}	
}
