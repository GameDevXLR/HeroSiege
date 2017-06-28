using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PotionsScript : InventoryItem 
{
	private int maxCharges;
	public int potionCharges;
	public Text chargesDisplay;
	public string potionDesc;
	public string potionDescFr;
	public int DesiredSellPrice = 25;
	public AudioClip Potion;
	public Sprite potionFull;
	public Sprite potionHalfFilled;
	public Sprite potionAlmostEmpty;

	public void Start()
	{
		maxCharges = potionCharges;
		chargesDisplay.text = potionDesc + potionCharges.ToString () + " available.";
		if (PlayerPrefs.GetString ("LANGAGE") == "Fr") 
		{
			potionDesc = potionDescFr;
			chargesDisplay.text = potionDesc + potionCharges.ToString () + " disponible.";

		}
	}

	public void LooseOneCharge()
	{
		if (GameManager.instanceGM.playerObj.GetComponent<PlayerIGManager> ().isDead) 
		{
			return;
		}

		if (potionCharges > 1) 
		{
			GetComponent<AudioSource> ().PlayOneShot (Potion);
			potionCharges--;
			chargesDisplay.text = potionDesc + potionCharges.ToString () + " available.";
			if (PlayerPrefs.GetString ("LANGAGE") == "Fr") 
			{
				chargesDisplay.text = potionDesc + potionCharges.ToString () + " disponible.";

			}
			if (potionCharges == 1) 
			{
				transform.Find("Image").GetComponent<Image> ().overrideSprite = potionAlmostEmpty;
			}
			if (potionCharges == 2) 
			{
				transform.Find("Image").GetComponent<Image> ().overrideSprite = potionHalfFilled;
			}

			return;
		} else {
			Destroy (gameObject);
		}
	}
	public override void SellBackItem ()
	{
		sellingPrice = potionCharges * (DesiredSellPrice / maxCharges);
		base.SellBackItem ();
	}

}
