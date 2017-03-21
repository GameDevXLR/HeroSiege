using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthPotionScript : MonoBehaviour 
{
	public int potionCharges;
	public Text chargesDisplay;

	public void LooseOneCharge()
	{
		if (potionCharges > 0) 
		{

			chargesDisplay.text = "Heal for 100HP. " + potionCharges.ToString () + " available.";
			potionCharges--;

			return;
		} else {
			Destroy (gameObject);
		}
	}
}
