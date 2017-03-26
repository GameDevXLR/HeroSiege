using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentItem : InventoryItem
{
public int desiredPrice;

	public override void SellBackItem ()
	{
		sellingPrice = desiredPrice;
		base.SellBackItem ();
	}
}
