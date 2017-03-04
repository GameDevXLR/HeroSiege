using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenericManaScript : MonoBehaviour {

	//ce script gere la mana de l'objet auquel il est attaché.

	public int maxMp = 100;
	public int currentMp = 80;
	public int regenMp;
	public int levelUpBonusMP;
	public float timeBetweenTic = 1f;
	private float lastTic;

	public RectTransform manaBar;
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
		if (currentMp < maxMp) 
		{
			currentMp += regenMp;
			float x = (float)currentMp / maxMp;
			manaBar.localScale = new Vector3 (x, 1f, 1f);
		} else 
		{
			manaBar.GetComponentInParent<Canvas> ().enabled = false;

		}
	}
	public void LooseManaPoints(int mana)
	{
		currentMp -= mana;
		float x = (float)currentMp / maxMp;
		manaBar.localScale = new Vector3 (x, 1f, 1f);
		manaBar.GetComponentInParent<Canvas> ().enabled = true;
	}
	public void LevelUp()
	{
		maxMp += levelUpBonusMP;
		currentMp = maxMp;
	}
}
