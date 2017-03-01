using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericManaScript : MonoBehaviour {

	//ce script gere la mana de l'objet auquel il est attaché.

	public int maxMp = 100;
	public int currentMp = 80;
	public int regenMp;
	public float timeBetweenTic = 1f;
	private float lastTic;

	// Use this for initialization
	void Start () {
		lastTic = 0f;

	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time > lastTic) 
		{
			lastTic = Time.time + timeBetweenTic;
			RegenerateMp ();
		}
		if (currentMp > maxMp) 
		{
			currentMp = maxMp;
		}
		if (currentMp < 0) 
		{
			currentMp = 0;
		}
	}

	public void RegenerateMp ()
	{
		currentMp += regenMp;
	}
	public void LooseManaPoints(int mana)
	{
		currentMp -= mana;
	}
}
