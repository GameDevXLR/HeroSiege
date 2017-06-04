using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuLangageManager : MonoBehaviour 
{
	public Text createBtn;
	public Text oneLaneBtn;
	public Text joinBtn;
	public Text quitBtn;
	public Text langageBtn;
	[Header("options texts")]
	public Text genVolBtn;
	public Text musVolBtn;
	public Text BegGuideBtn;
	[Header("profile texts")]
	public Text changeNBtn;
	public Text changeBtn;


	// Use this for initialization
	void Start () 
	{
		if (PlayerPrefs.GetString ("LANGAGE") == "Fr") 
		{
			Invoke ("MakeMenuFr", 0.1f);
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	public void MakeMenuFr()
	{
		createBtn.text = "CREER";
		joinBtn.text = "REJOINDRE";
		oneLaneBtn.text = "UNE SEULE EQUIPE";
		quitBtn.text = "QUITTER";
		langageBtn.text = "English";
		genVolBtn.text = "Volume Général";
		musVolBtn.text = "Volume de la musique";
		BegGuideBtn.text = "Guide débutant";
		changeNBtn.text = "Changer de nom";
		changeBtn.text = "Changer";
	}
	public void MakeMenuEng()
	{
		createBtn.text = "CREATE";
		joinBtn.text = "JOIN";
		oneLaneBtn.text = "ONE LANE GAME";
		quitBtn.text = "QUIT";
		langageBtn.text = "Francais";
		genVolBtn.text = "General Volume";
		musVolBtn.text = "Music Volume";
		BegGuideBtn.text = "Beginner's Guide";
		changeNBtn.text = "Change name";
		changeBtn.text = "Change";
	}
	public void SwitchLangage()
	{

		if (langageBtn.text == "Francais") //fr
		{
			PlayerPrefs.SetString ("LANGAGE", "Fr");
			MakeMenuFr ();
		}		else //english
		{
			PlayerPrefs.SetString ("LANGAGE", "Eng");
			MakeMenuEng ();
		}
	}
}
