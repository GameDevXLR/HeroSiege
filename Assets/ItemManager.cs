using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

public class ItemManager : NetworkBehaviour 
{
	public Transform guardSpawnPoint;
	public UnityEvent[] itemEvents;
	public GameObject targetplayer;
	public GameObject guard1Prefab;

	public void Start()
	{
		if (!isServer) 
		{
			return;
		}
		guardSpawnPoint = GameObject.Find ("GuardSpawnPoint").transform;
	}

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
	public void RecruteAGuard()
	{
		int x = GameManager.instanceGM.GetComponent<PNJManager> ().GuardNbr;
		GameManager.instanceGM.GetComponent<PNJManager> ().GuardNbr++;
		GameObject newGuard = Instantiate (guard1Prefab, guardSpawnPoint.position, guardSpawnPoint.rotation) as GameObject;
		newGuard.GetComponent<PlayerClicToMove> ().startingPos = GameManager.instanceGM.GetComponent<PNJManager> ().campGuardPositions [x].position;
		NetworkServer.Spawn (newGuard);
	}
}
