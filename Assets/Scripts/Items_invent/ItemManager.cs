using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using UnityEngine.AI;

public class ItemManager : NetworkBehaviour 
{
	//ce script doit etre placé sur le joueur pour des raisons d'autorité sur le réseau.
	// il contient une liste de fonction appelée par l'achat d'un objet voir le script : 
	// BuyableItemScript qui gere lui l'action de selectionner un objet a acheter puis fait
	//appel a ce script pour l'achat sur le réseau.
	public Transform guardSpawnPoint;
	public UnityEvent[] itemEvents;
	public GameObject targetplayer;
	public NetworkInstanceId targetID;
	public GameObject guard1Prefab;
	public Transform inventoryPanel;
	public Transform selectableSlot;
	[Header ("Prefabs achetable/spawnable")]
	public GameObject healthPotionPrefab;
	public GameObject manaPotionPrefab;
	public GameObject bigHealthPotionPrefab;
	public GameObject bigManaPotionPrefab;
	public GameObject tpScrollPrefab;
	public GameObject tpBackPortalPrefab;
	public GameObject ArchiRingPrefab;
	public GameObject RunnerBootsPrefab;
	public GameObject IgdraBraceletPrefab;
	public GameObject OrbOfPowerPrefab;
	public GameObject buyTomeParticle;
	public AudioClip Gold;
	public bool tip1Given;

	public void Start()
	{
		if (isLocalPlayer) 
		{
			inventoryPanel = GameObject.Find ("InventoryPanel").transform;
		}
	}
	#region Ne pas toucher : Fonctions de bases.
	public void BuyItem(int itemID, int itemPrice, bool recquireSlot)
	{
		if (recquireSlot) 
		{
//			targetplayer = NetworkServer.FindLocalObject (targetID);
			for (int i = 0; i < 8; i++) 
			{
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
			if (!tip1Given) 
			{
				GameManager.instanceGM.ShowAGameTip ("You can use the keys 1 to 9 of your keyboard to consume items of your inventory.");
				if (PlayerPrefs.GetString ("LANGAGE") == "Fr") 
				{
					GameManager.instanceGM.ShowAGameTip ("Vous pouvez utiliser les touches 1 a 9 de votre clavier pour utiliser un consommable de votre inventaire.");

				}
				tip1Given = true;
			}
		}
	}

	public void AskForRefund(int goldy)
	{
		CmdRefundAnObject (goldy);
	}

	[Command]
	public void CmdBuyThatForHim (int itemID, int itemPrice, NetworkInstanceId ID)
	{
		targetID = ID;
		targetplayer = NetworkServer.FindLocalObject (ID);
		targetplayer.GetComponent<PlayerGoldScript> ().ActualGold -= itemPrice;
		RpcInvokeTheGoodEvent (itemID);
	}

	[ClientRpc]
	public void RpcInvokeTheGoodEvent(int itemID)
	{

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
		StopCoroutine (BuyTomeEffect ());
		StartCoroutine (BuyTomeEffect ());
		if (isServer) {
			targetplayer = NetworkServer.FindLocalObject (targetID);
			targetplayer.GetComponent<PlayerIGManager> ().maxHp += 50;
			targetplayer.GetComponent<PlayerIGManager> ().bonusHp += 50;

		}
	}
	[ClientRpc]
	public void RpcUpMyMana()
	{
		StopCoroutine (BuyTomeEffect ());
		StartCoroutine (BuyTomeEffect ());
		if (isServer) {
			targetplayer = NetworkServer.FindLocalObject (targetID);
			targetplayer.GetComponent<GenericManaScript> ().maxMp += 30;
			targetplayer.GetComponent<GenericManaScript> ().bonusMp += 30;

		}
	}
	[ClientRpc]
	public void RpcUpMyDamage()
	{
		StopCoroutine (BuyTomeEffect ());
		StartCoroutine (BuyTomeEffect ());
		if (isServer) 
		{
			targetplayer = NetworkServer.FindLocalObject (targetID);
			targetplayer.GetComponent<PlayerAutoAttack> ().damage += 5;
			targetplayer.GetComponent<PlayerAutoAttack> ().bonusDamage += 5;

		}

	}
	[ClientRpc]
	public void RpcUpMyExp()
	{
		StopCoroutine (BuyTomeEffect ());
		StartCoroutine (BuyTomeEffect ());
		if (isServer) 
		{
			targetplayer = NetworkServer.FindLocalObject (targetID);
			targetplayer.GetComponent<PlayerXPScript> ().GetXP (targetplayer.GetComponent<PlayerXPScript> ().requiredXPToUp-targetplayer.GetComponent<PlayerXPScript> ().actualXP);
		}

	}
	[ClientRpc]
	public void RpcUpMyRegen()
	{
		StopCoroutine (BuyTomeEffect ());
		StartCoroutine (BuyTomeEffect ());
		if (isServer) 
		{
			targetplayer = NetworkServer.FindLocalObject (targetID);
			targetplayer.GetComponent<PlayerIGManager> ().regenHp += 2;
			targetplayer.GetComponent<GenericManaScript> ().regenMp += 2;
		}

	}

	IEnumerator BuyTomeEffect()
	{
		
		buyTomeParticle.SetActive (true);
		yield return new WaitForSeconds (2f);
		buyTomeParticle.SetActive (false);
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

	#endregion

	#region Bosses (up de boss)

	[ClientRpc]
	public void RpcUpTheBossT1()
	{
		GameManager GM;
		GM = GameManager.instanceGM;
		if (GM.isTeam1) 
		{
			if (PlayerPrefs.GetString ("LANGAGE") == "Fr") 
			{
				GM.messageManager.SendAnAlertMess ("Notre boss devient plus fort.", Color.green);

			} else 
			{
				GM.messageManager.SendAnAlertMess ("Our boss is growing stronger", Color.green);
			}
		} else 
		{
			if (PlayerPrefs.GetString ("LANGAGE") == "Fr") 
			{
				GM.messageManager.SendAnAlertMess ("Le boss ennemi devient plus fort!", Color.red);

			} else 
			{
				GM.messageManager.SendAnAlertMess ("The enemy boss is getting stronger.", Color.red);
			}
		}
		if (isServer) 
		{
			GameManager.instanceGM.gameObject.GetComponent<BossSpawnManager> ().bossLvlT1++;
		}

	}
	[ClientRpc]
	public void RpcUpTheBossT2()
	{
		GameManager GM;
		GM = GameManager.instanceGM;
		if (GM.isTeam2) 
		{
			if (PlayerPrefs.GetString ("LANGAGE") == "Fr") 
			{
				GM.messageManager.SendAnAlertMess ("Notre boss devient plus fort.", Color.green);

			} else 
			{
				GM.messageManager.SendAnAlertMess ("Our boss is growing stronger.", Color.green);
			}
		} else 
		{
			if (PlayerPrefs.GetString ("LANGAGE") == "Fr") 
			{
				GM.messageManager.SendAnAlertMess ("Le boss ennemi devient plus fort!", Color.red);

			} else 
			{
				GM.messageManager.SendAnAlertMess ("The enemy boss is getting stronger!", Color.red);
			}
		}
		if (isServer) 
		{
			GameManager.instanceGM.gameObject.GetComponent<BossSpawnManager> ().bossLvlT2++;
		}

	}

	#endregion

	#region Consommables (potions / parchemins etc)

	[ClientRpc]
	public void RpcBuyHealthPotion()
	{
		if (isLocalPlayer) 
		{

			GameObject go = Instantiate (healthPotionPrefab, selectableSlot);
			go.transform.localScale = new Vector3 (1f, 1f, 1f);
			GetComponent<AudioSource> ().PlayOneShot (Gold);
		}
	}
	[ClientRpc]
	public void RpcUseHealthPotion()
	{
		if (isServer) 
		{
			GetComponent<PlayerIGManager> ().currentHp += 500;
		}
	}
	[ClientRpc]
	public void RpcBuyBigHealthPotion()
	{
		if (isLocalPlayer) 
		{

			GameObject go = Instantiate (bigHealthPotionPrefab, selectableSlot);
			go.transform.localScale = new Vector3 (1f, 1f, 1f);
			GetComponent<AudioSource> ().PlayOneShot (Gold);
		}
	}
	[ClientRpc]
	public void RpcUseBigHealthPotion()
	{
		if (isServer) 
		{
			GetComponent<PlayerIGManager> ().currentHp += 2000;
		}
	}

	[ClientRpc]
	//rend 200 mana par charge. voir objet pour charges associés.
	public void RpcBuyManaPotion()
	{
		if (isLocalPlayer) 
		{

			GameObject go = Instantiate (manaPotionPrefab, selectableSlot);
			go.transform.localScale = new Vector3 (1f, 1f, 1f);
			GetComponent<AudioSource> ().PlayOneShot (Gold);
		}
	}
	[ClientRpc]
	public void RpcUseManaPotion()
	{
		if (isServer) 
		{
			GetComponent<GenericManaScript> ().currentMp += 200;
		}
	}
	[ClientRpc]
	//rend 200 mana par charge. voir objet pour charges associés.
	public void RpcBuyBigManaPotion()
	{
		if (isLocalPlayer) 
		{

			GameObject go = Instantiate (bigManaPotionPrefab, selectableSlot);
			go.transform.localScale = new Vector3 (1f, 1f, 1f);
			GetComponent<AudioSource> ().PlayOneShot (Gold);
		}
	}
	[ClientRpc]
	public void RpcUseBigManaPotion()
	{
		if (isServer) 
		{
			GetComponent<GenericManaScript> ().currentMp += 1000;
		}
	}

	[ClientRpc]
	//TP back le gars qui marche dessus. une charge de base
	public void RpcBuyTPScroll()
	{
		if (isLocalPlayer) 
		{

			GameObject go = Instantiate (tpScrollPrefab, selectableSlot);
			go.transform.localScale = new Vector3 (1f, 1f, 1f);
			GetComponent<AudioSource> ().PlayOneShot (Gold);
		}
	}
	[ClientRpc]
	public void RpcUseTPScroll()
	{
		if (isServer) 
		{
			GameObject go = Instantiate (tpBackPortalPrefab, transform.position, Quaternion.Euler(-90f, 0f, 0f));
			NetworkServer.Spawn (go);
		}
	}
	#endregion


	#region Stuffs (equipement divers)

	[ClientRpc]
	//anneau qui boost la mana mais se revend pas cher une merde...
	public void RpcBuyArchimageRing()
	{
		if (isServer) 
		{
			GetComponent<GenericManaScript> ().maxMp += 150;
			GetComponent<GenericManaScript> ().bonusMp += 150;

			GetComponent<GenericManaScript> ().regenMp += 5;
		}
		if (isLocalPlayer) 
		{
			GameObject go = Instantiate (ArchiRingPrefab, selectableSlot);
			go.transform.localScale = new Vector3 (1f, 1f, 1f);
			GetComponent<AudioSource> ().PlayOneShot (Gold);
		}
	}
	[ClientRpc]
	public void RpcSellArchimageRing()
	{
		if (isServer) 
		{
			GetComponent<GenericManaScript> ().maxMp -= 150;
			GetComponent<GenericManaScript> ().bonusMp -= 150;

			GetComponent<GenericManaScript> ().regenMp -= 5;
		}

	}
	[ClientRpc]
	//des bottes qui te speed up.
	public void RpcBuyRunnerBoots()
	{
//		GetComponent<NavMeshAgent> ().speed += 0.5f;
		if (isLocalPlayer) 
		{
			GameObject go = Instantiate (RunnerBootsPrefab, selectableSlot);
			go.transform.localScale = new Vector3 (1f, 1f, 1f);
			GetComponent<AudioSource> ().PlayOneShot (Gold);
		}
		if (isServer) 
		{
			
			GetComponent<PlayerClicToMove> ().playerSpeed += 0.5f;
			GetComponent<PlayerIGManager> ().dodge += 5f;
		}
	}
	[ClientRpc]
	public void RpcSellRunnerBoots()
	{
		GetComponent<NavMeshAgent> ().speed -= 0.5f;
		if (isServer) 
		{
			GetComponent<PlayerClicToMove> ().playerSpeed -= 0.5f;
			GetComponent<PlayerIGManager> ().dodge -= 5f;

		}
	}
	[ClientRpc]
	//un putin de bracelet bien cheaté.
	public void RpcBuyIgdrasilBracelet()
	{
		if (isServer) 
		{
			GetComponent<PlayerIGManager> ().maxHp += 550;
			GetComponent<PlayerIGManager> ().bonusHp += 550;

			GetComponent<PlayerIGManager> ().regenHp += 10;
			GetComponent<PlayerIGManager> ().armorScore += 50;
			GetComponent<PlayerIGManager> ().bonusArmorScore += 50;

		}
		if (isLocalPlayer) 
		{
			GameObject go = Instantiate (IgdraBraceletPrefab, selectableSlot);
			go.transform.localScale = new Vector3 (1f, 1f, 1f);
			GetComponent<AudioSource> ().PlayOneShot (Gold);
		}
	}
	[ClientRpc]
	public void RpcSellIgdrasilBracelet()
	{
		if (isServer) 
		{
			GetComponent<PlayerIGManager> ().maxHp -= 550;
			GetComponent<PlayerIGManager> ().bonusHp -= 550;

			GetComponent<PlayerIGManager> ().regenHp -= 10;
			GetComponent<PlayerIGManager> ().armorScore -= 50;

		}
	}
	[ClientRpc]
	//une orbe qui boost les dégats et augmente l'armure.
	public void RpcBuyOrbOfPower()
	{
		if (isServer) 
		{
			GetComponent<PlayerAutoAttack> ().damage += 200;
			GetComponent<PlayerAutoAttack> ().bonusDamage += 200;

		}
		if (isLocalPlayer) 
		{
			GameObject go = Instantiate (OrbOfPowerPrefab, selectableSlot);
			go.transform.localScale = new Vector3 (1f, 1f, 1f);
			GetComponent<AudioSource> ().PlayOneShot (Gold);
		}
	}
	[ClientRpc]
	public void RpcSellOrbOfPower()
	{
		if (isServer) 
		{
			GetComponent<PlayerAutoAttack> ().damage -= 200;
			GetComponent<PlayerAutoAttack> ().bonusDamage -= 200;

		}
	}

	#endregion
}
