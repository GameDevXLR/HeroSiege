using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerXPScript : MonoBehaviour 
{
	public Text playerLvl;
	public RectTransform xpDisplay;
	public int actualXP;
	public int requiredXPToUp = 50;
	public int actualLevel = 1;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public void GetXP(int xp)
	{
		actualXP += xp;

		if (actualXP >= requiredXPToUp) 
		{
			actualLevel++;
			actualXP = 0;
			requiredXPToUp *= 1 + actualLevel;
			playerLvl.text = actualLevel.ToString ();
			GetComponent<GenericLifeScript> ().LevelUp ();
			GetComponent<GenericManaScript> ().LevelUp ();
			GetComponent<AutoAttackScript> ().LevelUp ();

		}
		float x = (float)actualXP / requiredXPToUp;
		xpDisplay.localScale = new Vector3 (x, 1f, 1f);
	}
}
