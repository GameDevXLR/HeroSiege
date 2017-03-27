using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerLevelUpManager : MonoBehaviour 
{

	//ce script gere les options de lvlup d'un joueur.
	// il dit au joueur quoi faire en cas de up
	public int specPts; //points de spécialisation
	public GameObject spell1LvlUpBtn;
	public GameObject spell2LvlUpBtn;
	public GameObject spectPtIcon;
	// Use this for initialization
	void Start () 
	{
		spell1LvlUpBtn = GameObject.Find ("Spell1LvlUpBtn");
		spell2LvlUpBtn = GameObject.Find ("Spell2LvlUpBtn");
		//rajouter ici les futurs sorts a faire up.
		spectPtIcon = GameObject.Find ("CompPtsIcon");

	}

	public void GetAlevel()
	{
		specPts++;
		spectPtIcon.SetActive (true);
		spell1LvlUpBtn.SetActive (true);
		spell2LvlUpBtn.SetActive (true);
	}
	public void LooseASpecPt()
	{
		specPts--;
		if (specPts <= 0) 
		{
			spectPtIcon.SetActive (false);
			spell1LvlUpBtn.SetActive (false);
			spell2LvlUpBtn.SetActive (false);

		}
	}
}
