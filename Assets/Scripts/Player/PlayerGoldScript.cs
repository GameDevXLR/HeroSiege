using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerGoldScript : NetworkBehaviour 
{
	//petit script qui gere l'or d'un joueur. rien de fou : synchro sur réseau.
	public Text goldDisplay;
	public float timeBetweenGoldDrop;
	private float lastGoldDrop;
	public int goldPerDrop;
	[SyncVar]public bool isDropping;
	[SyncVar(hook ="GoldActualize" )]public int ActualGold;

	public void GetGold(int gold)
	{
		if (!isServer) 
		{
			return;
		}
		ActualGold += gold;
	}

	void Start()
	{
		if (isLocalPlayer) 
		{
			goldDisplay = GameObject.Find ("PlayerGold").GetComponent<Text> ();
		}
		if (isServer) 
		{
			lastGoldDrop = Time.time + timeBetweenGoldDrop;
		}
	}
	public void Update()
	{
		if (!isServer || !isDropping) 
		{
			return;
		}
		if (Time.time > lastGoldDrop) 
		{
			lastGoldDrop = Time.time + timeBetweenGoldDrop;
			ActualGold += goldPerDrop;
		}
	}

	public void GoldActualize(int goldygold)
	{
		ActualGold = goldygold;
		if (isLocalPlayer) 
		{
			if (goldDisplay == null) 
			{
				goldDisplay = GameObject.Find ("PlayerGold").GetComponent<Text> ();

			}
			goldDisplay.text = ActualGold.ToString ();
		}
	}
}
