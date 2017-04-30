using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour 
{
	private bool actifTabFG;
	private bool actifTabProfile;
	public InputField nameField;
	public Text PlayerNameDisplay;
	public GameObject FindGamesCanvasObj;
	public GameObject ProfileCanvasObj;
	public string PlayerNickname;

	void Awake()
	{
		FindGamesCanvasObj.GetComponent<Canvas>().enabled = false;	
		ProfileCanvasObj.GetComponent<Canvas>().enabled = false;	
	}
	public void Start()
	{
//		DontDestroyOnLoad (gameObject);
		PlayerNameDisplay.text = PlayerPrefs.GetString("PlayerNN");
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

			return;
		}
		if (actifTabProfile == true)
		{

			ProfileCanvasObj.GetComponent<Canvas>().enabled = false;
			actifTabProfile = false;			
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

	public void ChangeName()
	{
		if (nameField.text == "") 
		{
			return;
		}
		
		if (nameField.text.Length > 24) 
		{
			nameField.text = nameField.text.Substring (0, 24);
		}
		PlayerNickname = nameField.text;
		PlayerPrefs.SetString("PlayerNN", PlayerNickname);
		PlayerNameDisplay.text = nameField.text;
		nameField.text = "";
	}
}
