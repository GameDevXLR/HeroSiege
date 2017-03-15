using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerXPScript : NetworkBehaviour 
{
	public Text playerLvl;
	public Text generalTxt;
	public RectTransform xpDisplay;
	public Text xpText;
	[SyncVar (hook= "XPActualize")]public int actualXP;
	public int requiredXPToUp = 50;
	public int actualLevel = 1;
	// Use this for initialization
	void Start () 
	{
		if (!isLocalPlayer) 
		{
			return;
		}
		playerLvl = GameObject.Find ("PlayerLevel").GetComponent<Text> ();
		generalTxt = GameObject.Find ("GeneralText").GetComponent<Text> ();
		xpDisplay = GameObject.Find ("ActualXP").GetComponent<RectTransform> ();
		xpText = GameObject.Find ("XPTextDisplay").GetComponent<Text> ();
		xpText.text = actualXP.ToString () + " / "+ requiredXPToUp.ToString();
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
	public void GetXP(int xp)
	{
		if (!isServer) 
		{
			return;
		}
		actualXP += xp;
		if (isLocalPlayer) 
		{
			xpText.text = actualXP.ToString () + " / "+ requiredXPToUp.ToString();

		}

	}

	IEnumerator LevelUpMessage()
	{
		generalTxt.enabled = true;
		generalTxt.text = "You have reach level " + actualLevel + " .";
		yield return new WaitForSeconds (2f);
		generalTxt.enabled = false;
		GetComponent<PlayerLevelUpManager> ().GetAlevel ();

	}
	public void XPActualize(int xp)
	{
		actualXP = xp;
		if (actualXP >= requiredXPToUp) 
		{
			actualLevel++;
			actualXP = 0;
			requiredXPToUp *= 1 + actualLevel;
			if (isLocalPlayer) 
			{
				StartCoroutine (LevelUpMessage ());
				playerLvl.text = actualLevel.ToString ();

			}
			GetComponent<GenericLifeScript> ().LevelUp ();
			GetComponent<GenericManaScript> ().LevelUp ();
			GetComponent<AutoAttackScript> ().LevelUp ();

		}
		if (isLocalPlayer) {
			float x = (float)actualXP / requiredXPToUp;
			xpDisplay.localScale = new Vector3 (x, 1f, 1f);
			xpText.text = actualXP.ToString () + " / "+ requiredXPToUp.ToString();

		}
	}
}
