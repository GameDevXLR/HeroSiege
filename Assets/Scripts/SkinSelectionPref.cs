using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinSelectionPref : MonoBehaviour 
{

	public Image avatarImg;
	public Sprite avatarDesired;
	public void SetSkinColor(int i)
	{
		PlayerPrefs.SetInt ("SKIN_COLOR", i);
//		avatarImg.sprite = avatarDesired;

	}

}
