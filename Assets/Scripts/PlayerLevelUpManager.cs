﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerLevelUpManager : NetworkBehaviour {

	//ce script gere les options de lvlup d'un joueur.
	public int specPts; //points de spécialisation
	public GameObject spell1LvlUpBtn;
	public GameObject spectPtIcon;
	// Use this for initialization
	void Start () 
	{
		spell1LvlUpBtn = GameObject.Find ("Spell1LvlUpBtn");
		spectPtIcon = GameObject.Find ("CompPtsIcon");

	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	public void GetAlevel()
	{
		specPts++;
		spectPtIcon.SetActive (true);
		spell1LvlUpBtn.SetActive (true);
	}
	public void LooseASpecPt()
	{
		specPts--;
		if (specPts <= 0) 
		{
			spectPtIcon.SetActive (false);
			spell1LvlUpBtn.SetActive (false);
		}
	}
}
