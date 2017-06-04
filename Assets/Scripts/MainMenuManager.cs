using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour 
{
	private bool actifTabFG;
	private bool actifTabProfile;
	public bool actifTabOptions;
	public InputField nameField;
	public Text PlayerNameDisplay;
	public GameObject FindGamesCanvasObj;
	public GameObject ProfileCanvasObj;
	public GameObject OptionsCanvasObj;
	public string PlayerNickname;
	public Slider menuMusicVolume;
	public AudioSource menuMusic;
	public string langage;

	public Slider generalVolume;
	void Awake()
	{
		FindGamesCanvasObj.GetComponent<Canvas>().enabled = false;	
		ProfileCanvasObj.GetComponent<Canvas>().enabled = false;
		OptionsCanvasObj.GetComponent<Canvas> ().enabled = false;

	}
	public void Start()
	{
//		if (PlayerPrefs.GetString ("PlayerNN") == null) 
//		{
//			PlayerPrefs.SetString ("PlayerNN", "NoName");
//		}		
		PlayerNameDisplay.text = PlayerPrefs.GetString("PlayerNN");
		PlayerNickname = PlayerPrefs.GetString ("PlayerNN");
		menuMusic.volume = PlayerPrefs.GetFloat ("MUSIC_VOLUME", 0.5f);
		menuMusicVolume.value = menuMusic.volume;
		langage = PlayerPrefs.GetString ("LANGAGE", "Eng");
		AudioListener.volume = PlayerPrefs.GetFloat ("GENERAL_VOLUME", 0.5f);
		generalVolume.value = AudioListener.volume;
		if (PlayerNameDisplay.text == "") 
		{
			PlayerNameDisplay.text = "NewPlayer";
		}
		GetComponent<NetHostGame> ().roomName = PlayerNickname + "'s room.";
	}
	public void ToggleFindGCanvas()
	{

		if (actifTabFG == false)
		{

			FindGamesCanvasObj.GetComponent<Canvas>().enabled = true;
			actifTabFG = true;
			if (actifTabProfile) 
			{
				ToggleProfileCanvas ();
			}
			if (actifTabOptions) 
			{
				ToggleOptionsCanvas ();
			}

			return;
		}
		if (actifTabFG == true)
		{

			FindGamesCanvasObj.GetComponent<Canvas>().enabled = false;
			actifTabFG = false;			
		}

	}
	public void ToggleProfileCanvas()
	{

		if (actifTabProfile == false)
		{

			ProfileCanvasObj.GetComponent<Canvas>().enabled = true;
			actifTabProfile = true;
			if (actifTabFG) 
			{
				ToggleFindGCanvas ();
			}
			if (actifTabOptions) 
			{
				ToggleOptionsCanvas ();
			}
			return;
		}
		if (actifTabProfile == true)
		{

			ProfileCanvasObj.GetComponent<Canvas>().enabled = false;
			actifTabProfile = false;			
		}

	}
	public void ToggleOptionsCanvas()
	{

		if (actifTabOptions == false)
		{

			OptionsCanvasObj.GetComponent<Canvas>().enabled = true;
			actifTabOptions = true;
			if (actifTabFG) 
			{
				ToggleFindGCanvas ();
			}
			if (actifTabProfile) 
			{
				ToggleProfileCanvas ();
			}
			return;
		}
		if (actifTabOptions == true)
		{

			OptionsCanvasObj.GetComponent<Canvas>().enabled = false;
			actifTabOptions = false;			
		}

	}

	public void MenuQuitGame()
	{
		Application.Quit ();
//		if(UnityEditor.EditorApplication.isPlaying)
//			{
//				UnityEditor.EditorApplication.isPlaying = false;
//			}
	}

	public void ChangeMusicMenuVolume()
	{
		menuMusic.volume = menuMusicVolume.value;
		PlayerPrefs.SetFloat ("MUSIC_VOLUME", menuMusicVolume.value);
	}

	public void ChangeGeneralVolume()
	{
		AudioListener.volume = generalVolume.value;
		PlayerPrefs.SetFloat ("GENERAL_VOLUME", generalVolume.value);
	}

	public void ChangeName()
	{
		if (nameField.text == "") 
		{
			return;
		}
		
		if (nameField.text.Length > 18) 
		{
			nameField.text = nameField.text.Substring (0, 18);
		}
		PlayerNickname = nameField.text;
		PlayerPrefs.SetString("PlayerNN", PlayerNickname);
		PlayerNameDisplay.text = nameField.text;
		nameField.text = "";
	}
}
