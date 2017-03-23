using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PotionsScript : MonoBehaviour 
{
	public int potionCharges;
	public Text chargesDisplay;
	public string potionDesc;

	public void Start()
	{
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
}
