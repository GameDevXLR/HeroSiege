using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using NATTraversal;

public class MainMenuManager : MonoBehaviour 
{
	private bool actifTabFG;
	private bool actifTabProfile;
	public bool actifTabOptions;
	public bool actifTabCredits;
	public InputField nameField;
	public Text PlayerNameDisplay;
	public GameObject FindGamesCanvasObj;
	public GameObject ProfileCanvasObj;
	public GameObject OptionsCanvasObj;
	public GameObject CreditsCanvasObj;
	public string PlayerNickname;
	public Slider menuMusicVolume;
	public AudioSource menuMusic;
	public string langage;
	public AudioClip clicSound1;
	public AudioClip clicSound2;
//	public Toggle oneLaneTog;
	public Slider generalVolume;
	public GameObject NewPlayerPanel;

	void Awake()
	{
		FindGamesCanvasObj.GetComponent<Canvas>().enabled = false;	
		ProfileCanvasObj.GetComponent<Canvas>().enabled = false;
		OptionsCanvasObj.GetComponent<Canvas> ().enabled = false;
		CreditsCanvasObj.GetComponent<Canvas> ().enabled = false;
//		PlayerPrefs.DeleteKey ("PlayerNN");

	}

	void OnEnable()
	{
//		if (NATTraversal.NetworkManager.singleton.GetComponent<MusicBackgroundSwitch> ().audioSource !=null) {
//			ConfSound ();
//		} else 
//		{
			Invoke ("ConfSound", 1f);
//		}
	}
	public void ConfSound()
	{
		menuMusic = NATTraversal.NetworkManager.singleton.GetComponent<MusicBackgroundSwitch> ().audioSource;
		NATTraversal.NetworkManager.singleton.GetComponent<MusicBackgroundSwitch> ().LoadMenuMusic ();
	}
	public void Start()
	{
        //		if (PlayerPrefs.GetString ("PlayerNN") == null) 
        //		{
        //			PlayerPrefs.SetString ("PlayerNN", "NoName");
        //		}		
        if (!PlayerPrefs.HasKey("PlayerNN"))
        {
			PlayerPrefs.SetString("PlayerNN", "JohnDoe"+ Random.Range(1,999999).ToString() );
			FirstTimePlaying ();
        }
		PlayerNameDisplay.text = PlayerPrefs.GetString("PlayerNN");
        PlayerNickname = PlayerPrefs.GetString("PlayerNN") ;
		NATTraversal.NetworkManager.singleton.GetComponent<MusicBackgroundSwitch>().audioSource.volume = PlayerPrefs.GetFloat ("MUSIC_VOLUME", 0.5f);
		menuMusicVolume.value = NATTraversal.NetworkManager.singleton.GetComponent<MusicBackgroundSwitch>().audioSource.volume;
		langage = PlayerPrefs.GetString ("LANGAGE", "Eng");
		NATTraversal.NetworkManager.singleton.GetComponent<MusicBackgroundSwitch>().audioSource.volume = PlayerPrefs.GetFloat ("GENERAL_VOLUME", 0.5f);
		generalVolume.value = NATTraversal.NetworkManager.singleton.GetComponent<MusicBackgroundSwitch>().audioSource.volume;
		if (PlayerNameDisplay.text == "") 
		{
			PlayerNameDisplay.text = "NewPlayer";
		}
		GetComponent<NetHostGame> ().SetRoomName (PlayerNickname);
	}

	public void FirstTimePlaying ()
	{
		ToggleProfileCanvas ();
		NewPlayerPanel.SetActive (true);

	}

	public void CloseNewPlayerPanel(){
		NewPlayerPanel.SetActive (false);

	}

	public void ToggleFindGCanvas()
	{
		GetComponent<AudioSource> ().PlayOneShot (clicSound1);

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
			if (actifTabCredits) 
			{
				ToggleCreditsCanvas ();
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
		GetComponent<AudioSource> ().PlayOneShot (clicSound1);

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
			if (actifTabCredits) 
			{
				ToggleCreditsCanvas ();
			}
			return;
		}
		if (actifTabProfile == true)
		{

			ProfileCanvasObj.GetComponent<Canvas>().enabled = false;
			actifTabProfile = false;			
		}

	}
	public void ToggleCreditsCanvas()
	{
		GetComponent<AudioSource> ().PlayOneShot (clicSound1);

		if (actifTabCredits == false)
		{

			CreditsCanvasObj.GetComponent<Canvas>().enabled = true;
			actifTabCredits = true;
			if (actifTabFG) 
			{
				ToggleFindGCanvas ();
			}
			if (actifTabOptions) 
			{
				ToggleOptionsCanvas ();
			}
			if (actifTabProfile) 
			{
				ToggleProfileCanvas ();
			}
			return;
		}
		if (actifTabCredits == true)
		{

			CreditsCanvasObj.GetComponent<Canvas>().enabled = false;
			actifTabCredits = false;			
		}

	}
	public void ToggleOptionsCanvas()
	{
		GetComponent<AudioSource> ().PlayOneShot (clicSound1);

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
			if (actifTabCredits) 
			{
				ToggleCreditsCanvas ();
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
		if (!menuMusic) 
		{
			return;
		}
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
		GetComponent<AudioSource> ().PlayOneShot (clicSound2);

		if (nameField.text.Length > 12) 
		{
			nameField.text = nameField.text.Substring (0, 12);
		}
		PlayerNickname = nameField.text;
		PlayerPrefs.SetString("PlayerNN", PlayerNickname);
		GetComponent<NetHostGame> ().SetRoomName (PlayerNickname);
		PlayerNameDisplay.text = nameField.text;
		nameField.text = "";
	}


}
