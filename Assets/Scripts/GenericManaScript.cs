using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

[NetworkSettings(channel = 0, sendInterval = 0.5f)]
public class GenericManaScript : NetworkBehaviour {

	//ce script gere la mana de l'objet auquel il est attaché.

	public int maxMp = 100;
	[SyncVar]public int currentMp = 80;
	public int regenMp;
	public int levelUpBonusMP;
	public float timeBetweenTic = 1f;
	private float lastTic;

	public RectTransform manaBar;
	public RectTransform manaBarMain;
	public Text playerMPTxt;
	// Use this for initialization
	void Start () 
	{
		lastTic = 0f;
		if (isLocalPlayer) 
		{
			manaBarMain = GameObject.Find ("PlayerManaBarMain").GetComponent<RectTransform> ();
			playerMPTxt = GameObject.Find ("PlayerMpText").GetComponent<Text>();
			playerMPTxt.text = currentMp.ToString () + " / " + maxMp.ToString ();

		}
	}
	
	// Update is called once per frame
	void Update () {
		if (!isServer) 
		{
			return;
		}
		if (Time.time > lastTic) 
		{
			lastTic = Time.time + timeBetweenTic;
			RegenerateMp ();
		}
		if (currentMp > maxMp) 
		{
			currentMp = maxMp;
			manaBar.GetComponentInParent<Canvas> ().enabled = false;
			if (isLocalPlayer) {
				playerMPTxt.text = currentMp.ToString () + " / " + maxMp.ToString ();
			}

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
			manaBar.GetComponentInParent<Canvas> ().enabled = true;

			manaBar.localScale = new Vector3 (x, 1f, 1f);
			if (isLocalPlayer) 
			{
				manaBarMain.localScale = new Vector3 (x, 1f, 1f);
				playerMPTxt.text = currentMp.ToString () + " / " + maxMp.ToString ();

			}
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
		if (isLocalPlayer) 
		{
			manaBarMain.localScale = new Vector3 (x, 1f, 1f);
			playerMPTxt.text = currentMp.ToString () + " / " + maxMp.ToString ();

		}
		manaBar.GetComponentInParent<Canvas> ().enabled = true;
	}
	public void LevelUp()
	{
		maxMp += levelUpBonusMP;
		currentMp = maxMp;
		if (isLocalPlayer) 
		{
			manaBarMain.localScale = new Vector3 (1f, 1f, 1f);
			playerMPTxt.text = currentMp.ToString () + " / " + maxMp.ToString ();


		}
	}
}
