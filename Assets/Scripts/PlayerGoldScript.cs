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
	[SyncVar(hook ="GoldActualize" )]public int ActualGold;

	public void GetGold(int gold)
	{
		if (!isServer) 
		{
			return;
		}
		ActualGold += gold;
//		goldDisplay.text = ActualGold.ToString();
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
	[ServerCallback]
	public void Update()
	{
		if (Time.time > lastGoldDrop) 
		{
			lastGoldDrop += timeBetweenGoldDrop;
			ActualGold += goldPerDrop;
		}
	}

	public void GoldActualize(int goldygold)
	{
		ActualGold = goldygold;
		if (isLocalPlayer) 
		{
			goldDisplay.text = ActualGold.ToString ();
		}
	}
}
