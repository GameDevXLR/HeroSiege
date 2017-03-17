using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerGoldScript : NetworkBehaviour 
{
	//petit script qui gere l'or d'un joueur. rien de fou : synchro sur réseau.
	public Text goldDisplay;
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
		if (isLocalPlayer) {
			goldDisplay = GameObject.Find ("PlayerGold").GetComponent<Text> ();
		}
	}

	public void GoldActualize(int goldygold)
	{
		ActualGold = goldygold;
		if (isLocalPlayer) {
			goldDisplay.text = ActualGold.ToString ();
		}
	}
}
