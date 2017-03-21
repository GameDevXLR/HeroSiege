using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

public class ItemManager : NetworkBehaviour 
{
	//ce script doit etre placé sur le joueur pour des raisons d'autorité sur le réseau.
	// il contient une liste de fonction appelée par l'achat d'un objet voir le script : 
	// BuyableItemScript qui gere lui l'action de selectionner un objet a acheter puis fait
	//appel a ce script pour l'achat sur le réseau.

	public Transform guardSpawnPoint;
	public UnityEvent[] itemEvents;
	public GameObject targetplayer;
	[SyncVar]public NetworkInstanceId targetID;
	public GameObject guard1Prefab;
	public Transform inventoryPanel;
	public Transform selectableSlot;
	public GameObject healthPotionPrefab;


	public void Start()
	{
		if (isLocalPlayer) 
		{
			inventoryPanel = GameObject.Find ("InventoryPanel").transform;
		}
	}

	public void BuyItem(int itemID, int itemPrice, bool recquireSlot)
	{
		if (recquireSlot) 
		{
			targetplayer = NetworkServer.FindLocalObject (targetID);
			for (int i = 0; i < 8; i++) {
				if (inventoryPanel.GetChild (i).childCount == 0) 
				{
					selectableSlot = inventoryPanel.GetChild (i).transform;
					CmdBuyThatForHim (itemID, itemPrice, GameManager.instanceGM.ID);
					return;
				}
				if (inventoryPanel.GetChild (i).childCount == 1 && i == 7) {
					return; 
				}

			}
			return;
		} else 
		{
			CmdBuyThatForHim (itemID, itemPrice, GameManager.instanceGM.ID);
		}
	}

	[Command]
	public void CmdBuyThatForHim (int itemID, int itemPrice, NetworkInstanceId ID)
	{
		targetID = ID;
		targetplayer = NetworkServer.FindLocalObject (ID);
		targetplayer.GetComponent<PlayerGoldScript> ().ActualGold -= itemPrice;
		itemEvents [itemID].Invoke ();
	}
	[Command]
	public void CmdRefundAnObject(int gold)
	{
		GetComponent<PlayerGoldScript> ().GetGold (gold);
	}

	[ClientRpc]
	public void RpcUpMyLife()
	{
		targetplayer = NetworkServer.FindLocalObject (targetID);
		if (isServer) {
			targetplayer.GetComponent<GenericLifeScript> ().maxHp += 20;
		}
	}
	[ClientRpc]
	public void RpcUpMyMana()
	{
		targetplayer = NetworkServer.FindLocalObject (targetID);
		if (isServer) {
			targetplayer.GetComponent<GenericManaScript> ().maxMp += 20;
		}
	}
	[ClientRpc]
	public void RpcUpMyDamage()
	{
		targetplayer = NetworkServer.FindLocalObject (targetID);

		if (isServer) 
		{
			targetplayer.GetComponent<PlayerAutoAttack> ().damage += 5;
		}

	}
	[ClientRpc]
	public void RpcRecruteAGuard()
	{
		return; //provisoire
//		int x = GameManager.instanceGM.GetComponent<PNJManager> ().GuardNbr;
//		GameManager.instanceGM.GetComponent<PNJManager> ().GuardNbr++;
//		GameObject newGuard = Instantiate (guard1Prefab, guardSpawnPoint.position, guardSpawnPoint.rotation) as GameObject;
////		newGuard.GetComponent<PlayerClicToMove> ().startingPos = GameManager.instanceGM.GetComponent<PNJManager> ().campGuardPositions [x].position;
//		NetworkServer.Spawn (newGuard);
	}
	[ClientRpc]
	public void RpcBuyHealthPotion()
	{
		if (isLocalPlayer) 
		{
			GameObject go = Instantiate (healthPotionPrefab, selectableSlot);
			go.transform.localScale = new Vector3 (1f, 1f, 1f);
		}
	}
	[ClientRpc]
	public void RpcUseHealthPotion()
	{
		if (isServer) 
		{
			GetComponent<GenericLifeScript> ().currentHp += 100;
		}
	}
}
