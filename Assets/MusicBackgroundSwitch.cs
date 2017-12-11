using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicBackgroundSwitch : MonoBehaviour {

	private AudioSource audioSource;
	public AudioClip Menu;
	public AudioClip Night;
	public AudioClip Day;
	// Use this for initialization
	void Start () 
	{
		audioSource = GetComponent<AudioSource> ();
		audioSource.clip = Menu;
		audioSource.Play ();
	}
	
	// Update is called once per frame
	public void StartDayNightMusic(bool night)
	{
		if (night) {
			audioSource.clip = Night;
			audioSource.Play ();
		} 
		else
		{
			audioSource.clip = Day;
			audioSource.Play ();
		}
	}

	public void LoadMenuMusic()
	{
		audioSource.clip = Menu;
		audioSource.Play ();
	}
}
