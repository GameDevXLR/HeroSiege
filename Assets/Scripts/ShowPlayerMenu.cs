﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class ShowPlayerMenu: MonoBehaviour {
	private bool actifTab;
	private GameObject ChildMenu;
	public Slider myVol; 
	public Slider musicVol;
	public AudioSource musicSource;

	void Awake()
	{
		ChildMenu = gameObject;
		ChildMenu.GetComponent<Canvas>().enabled = false;	
	}
	void Start()
	{
		musicSource.volume = PlayerPrefs.GetFloat ("MUSIC_VOLUME");
		AudioListener.volume = PlayerPrefs.GetFloat ("GENERAL_VOLUME");
		myVol.value = AudioListener.volume;
		musicVol.value = musicSource.volume;
	}
	public void ToggleMenu()
	{

		if (actifTab == false)
		{

			ChildMenu.GetComponent<Canvas>().enabled = true;
			actifTab = true;

			return;
		}
		if (actifTab == true)
		{

			ChildMenu.GetComponent<Canvas>().enabled = false;
			actifTab = false;			
		}

	}	

	public void ChangeGeneralVolume()
	{
		AudioListener.volume = myVol.value;
		PlayerPrefs.SetFloat ("GENERAL_VOLUME", myVol.value);
	}
	public void ChangeMusicVolume()
	{
		musicSource.volume = musicVol.value;
		PlayerPrefs.SetFloat ("MUSIC_VOLUME", musicVol.value);

	}
	void Update()
	{
		if (Input.GetKeyUp (KeyCode.Escape)) 
		{
			ToggleMenu ();
		}
	}
	public void QuitTheGame()
	{
		Application.Quit();
	}

	public void DisconnectThePlayer()
	{
		GameManager.instanceGM.DisconnectThePlayer ();
	}

}