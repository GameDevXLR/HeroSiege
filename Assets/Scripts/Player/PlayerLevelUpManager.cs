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
	public GameObject spellUltLvlUpBtn;
	public int spellUltLvl = 1;
	public GameObject spectPtIcon;

	// Use this for initialization
	void Start () 
	{
		
		spell1LvlUpBtn = GameObject.Find ("Spell1LvlUpBtn");
		spell2LvlUpBtn = GameObject.Find ("Spell2LvlUpBtn");
		spellUltLvlUpBtn = GameObject.Find ("Spell3LvlUpBtn");
		//rajouter ici les futurs sorts a faire up.
		spectPtIcon = GameObject.Find ("CompPtsIcon");
		Invoke ("AvoidEarlyUltiUp", 0.3f);

	}
	void AvoidEarlyUltiUp()
	{

			spellUltLvlUpBtn.SetActive (false);

	}
	public void GetAlevel()
	{
		specPts++;
		playerLvl++;
//		GetComponent<PlayerManager> ().GetALevel ();
		if (playerLvl == 3 || playerLvl == 6 || playerLvl == 9 || playerLvl == 12) 
		{
			ultiSpecPts++;
		}
		spell1LvlUpBtn.GetComponent<Animator> ().Play ("BtnCompPts");
		spectPtIcon.SetActive (true);
		spell1LvlUpBtn.SetActive (true);
		spell2LvlUpBtn.SetActive (true);
//		StartCoroutine (LevelUpProcess ());
		if (ultiSpecPts > 0) 
		{
			spellUltLvlUpBtn.SetActive (true);
		}
	}

//	IEnumerator LevelUpProcess()
//	{
//		yield return new WaitForSeconds (0.66f);
//	}

	public void LooseASpecPt(int spell)
	{
		if (spell == 3) 
		{
			ultiSpecPts--;
			spellUltLvl++;
			if (ultiSpecPts == 0) 
			{
				spellUltLvlUpBtn.SetActive (false);
			}
		}
		if(spell == 1){
			spell1Lvl++;
		}
		if(spell == 2){
			spell2Lvl++;
		}
		specPts--;
		if (specPts <= 0) 
		{
			spectPtIcon.SetActive (false);
			spell1LvlUpBtn.SetActive (false);
			spell2LvlUpBtn.SetActive (false);
			spellUltLvlUpBtn.SetActive (false);

		}
	}
}
