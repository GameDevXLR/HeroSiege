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

	[ClientRpc]
	public void RpcUpMyLife()
	{
		targetplayer.GetComponent<GenericLifeScript> ().maxHp += 20;
	}
	[ClientRpc]
	public void RpcUpMyMana()
	{
		targetplayer.GetComponent<GenericManaScript> ().maxMp += 20;
	}
	[ClientRpc]
	public void RpcUpMyDamage()
	{
		targetplayer.GetComponent<PlayerAutoAttack> ().damage += 5;
		if (isLocalPlayer) 
		{
			targetplayer.GetComponent<PlayerAutoAttack> ().damageDisplay.text = targetplayer.GetComponent<PlayerAutoAttack> ().damage.ToString ();
		}
	}
	[ClientRpc]
	public void RpcRecruteAGuard()
	{
		return; //provisoire
		int x = GameManager.instanceGM.GetComponent<PNJManager> ().GuardNbr;
		GameManager.instanceGM.GetComponent<PNJManager> ().GuardNbr++;
		GameObject newGuard = Instantiate (guard1Prefab, guardSpawnPoint.position, guardSpawnPoint.rotation) as GameObject;
//		newGuard.GetComponent<PlayerClicToMove> ().startingPos = GameManager.instanceGM.GetComponent<PNJManager> ().campGuardPositions [x].position;
		NetworkServer.Spawn (newGuard);
	}
}
