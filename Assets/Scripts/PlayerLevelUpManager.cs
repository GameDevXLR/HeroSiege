using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerLevelUpManager : MonoBehaviour 
{

	//ce script gere les options de lvlup d'un joueur.
	// il dit au joueur quoi faire en cas de up
	public int playerLvl=1; // le niveau du joueur. en local.
	public int specPts;//points de spécialisation / un de base.
	public int ultiSpecPts; // pts pour lvl up l'ulti!
	public GameObject spell1LvlUpBtn;
	public int spell1Lvl = 1;
	public GameObject spell2LvlUpBtn;
	public int spell2Lvl = 1;
	public GameObject spectPtIcon;

	// Use this for initialization
	void Start () 
	{
		spell1LvlUpBtn = GameObject.Find ("Spell1LvlUpBtn");
		spell2LvlUpBtn = GameObject.Find ("Spell2LvlUpBtn");
		//rajouter ici les futurs sorts a faire up.
		spectPtIcon = GameObject.Find ("CompPtsIcon");
		Invoke ("AvoidEarlyUltiUp", 0.3f);

	}
	void AvoidEarlyUltiUp()
	{

			spell2LvlUpBtn.SetActive (false);

	}
	public void GetAlevel()
	{
		specPts++;
		playerLvl++;
		if (playerLvl == 3 || playerLvl == 6 || playerLvl == 9 || playerLvl == 12) 
		{
			ultiSpecPts++;
		}
		spectPtIcon.SetActive (true);
		spell1LvlUpBtn.SetActive (true);
		if (ultiSpecPts > 0) 
		{
			spell2LvlUpBtn.SetActive (true);
		}
	}
	public void LooseASpecPt(bool isUlt)
	{
		if (isUlt) {
			ultiSpecPts--;
			spell2Lvl++;
			if (ultiSpecPts == 0) {
				spell2LvlUpBtn.SetActive (false);
			}
		} else 
		{
			spell1Lvl++;
		}
		
		specPts--;
		if (specPts <= 0) 
		{
			spectPtIcon.SetActive (false);
			spell1LvlUpBtn.SetActive (false);
			spell2LvlUpBtn.SetActive (false);

		}
	}
}
