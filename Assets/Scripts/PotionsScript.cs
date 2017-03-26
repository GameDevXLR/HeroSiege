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
	public int DesiredSellPrice = 25;

	public void Start()
	{
		maxCharges = potionCharges;
		chargesDisplay.text = potionDesc + potionCharges.ToString () + " available.";
	}

	public void LooseOneCharge()
	{
		if (potionCharges > 1) 
		{

			potionCharges--;
			chargesDisplay.text = potionDesc + potionCharges.ToString () + " available.";


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
