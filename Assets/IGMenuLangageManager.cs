using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IGMenuLangageManager : MonoBehaviour 
{
	[Header("Les éléments du menu")]
	public Text resumeBtn;
	public Text backtoMBtn;
	public Text musicTxt;
	public Text quitGameBtn;

	[Header("La select de difficulté")]
	public Text descriptionTxt;
	public Text difficileTxt;
	public Text nightmareTxt;
	public Text messDattenteTxt;

	[Header("les éléments d'interface")]
	public Text xpDescTxt;

	// Use this for initialization
	void Start () 
	{
        
		if (PlayerPrefs.GetString ("LANGAGE") == "Fr") 
		{
			MakeTheGameFr ();
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	void MakeTheGameFr()
	{
		resumeBtn.text = "Reprendre la partie";
		backtoMBtn.text = "Retourner au menu";
		quitGameBtn.text = "Quitter le jeu";
		musicTxt.text = "Musique";
		descriptionTxt.text = "Choisissez la difficulté de la partie. ATTENTION: ATTENDEZ QUE TOUS LES JOUEURS SOIENT CONNECTE.";
		messDattenteTxt.text = "Attendez que le joueur ayant créer la partie choisisse la difficulté";
		difficileTxt.text = "Difficile";
		nightmareTxt.text = "Cauchemar";
		xpDescTxt.text = "Expérience nécessaire pour passer au niveau suivant";
	}
}
