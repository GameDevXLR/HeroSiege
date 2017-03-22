﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerXPScript : NetworkBehaviour 
{
	//gere l'xp d'un joueur; l'xp est sync sur le réseau mais le lvl up se fait en local.

	public Text playerLvl;
	public Text generalTxt;
	public RectTransform xpDisplay;
	public Text xpText;
	[SyncVar (hook= "XPActualize")]public int actualXP;
	public int requiredXPToUp = 50;
	public int actualLevel = 1;
	private ParticleSystem LvlUpParticle;
	// Use this for initialization
	void Start () 
	{
		LvlUpParticle = GetComponentInChildren <ParticleSystem> ();
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
			LvlUpParticle.Play ();
			actualXP = 0;
			requiredXPToUp *= 1 + actualLevel;
			if (isLocalPlayer) 
			{
				StartCoroutine (LevelUpMessage ());
				playerLvl.text = actualLevel.ToString ();

			}
			GetComponent<GenericLifeScript> ().LevelUp ();
			GetComponent<GenericManaScript> ().LevelUp ();
			GetComponent<PlayerAutoAttack> ().LevelUp ();

		}
		if (isLocalPlayer) {
			float x = (float)actualXP / requiredXPToUp;
			xpDisplay.localScale = new Vector3 (x, 1f, 1f);
			xpText.text = actualXP.ToString () + " / "+ requiredXPToUp.ToString();

		}
	}
}
