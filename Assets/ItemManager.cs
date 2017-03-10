using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

public class ItemManager : NetworkBehaviour 
{
	public UnityEvent[] itemEvents;
	public GameObject targetplayer;

	public void BuyItem(int itemID, int itemPrice)
	{
		CmdBuyThatForHim (itemID, itemPrice, GameManager.instanceGM.ID);
	}
	[Command]
	public void CmdBuyThatForHim (int itemID, int itemPrice, NetworkInstanceId ID)
	{
		targetplayer = NetworkServer.FindLocalObject (ID);
		targetplayer.GetComponent<PlayerGoldScript> ().ActualGold -= itemPrice;
		itemEvents [itemID].Invoke ();
	}

	public void UpMyLife()
	{
		targetplayer.GetComponent<GenericLifeScript> ().maxHp += 20;
	}
	public void UpMyMana()
	{
		targetplayer.GetComponent<GenericManaScript> ().maxMp += 20;
	}
	public void UpMyDamage()
	{
		targetplayer.GetComponent<AutoAttackScript> ().damage += 5;
	}
}
