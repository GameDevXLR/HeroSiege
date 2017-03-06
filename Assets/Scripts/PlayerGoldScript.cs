using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGoldScript : MonoBehaviour {

	public Text goldDisplay;
	public int ActualGold;

	public void GetGold(int gold)
	{
		ActualGold += gold;
		goldDisplay.text = ActualGold.ToString();
	}

	void Start()
	{
		goldDisplay = GameObject.Find ("PlayerGold").GetComponent<Text> ();
	}
}
