using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericLifeScript : MonoBehaviour {

	// ce script sert a gerer la vie de l'objet auquel il est attacher. en cas de mort; l'objet est détruit.

	public int maxHp;
	public int currentHp;
	public int armorScore;

	public uint regenHp;

	private float lastTic;
	public float timeBetweenTic = 5f;
	private float startPopo;
	public float timeofPotion;


	// Use this for initialization
	void Start () {
		lastTic = 0f;
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time > timeofPotion) 
		{
			stopPotion ();
		}

		if (Time.time > lastTic) 
		{
			lastTic = Time.time + timeBetweenTic;
//			RegenYourHP ();
		}


		if (currentHp > maxHp) 
		{
			currentHp = maxHp;
		}

		if (currentHp < 0) 
		{
			currentHp = 0;
		}
		if(Input.GetKeyDown(KeyCode.A)){
			activatePotion();
		}
	}

	public void LooseHealth(int dmg, bool trueDmg)
	{
		if (currentHp > 0)
		{
			if (trueDmg) 
			{
				currentHp -= dmg;
				return;
			}
			float multiplicatorArmor = 100 / (100 + armorScore);

			currentHp -= (int)Mathf.Abs( dmg * multiplicatorArmor);
		}
	}
//	public void	RegenYourHP ()
//	{
//		currentHp += regenHp;
//	}
//
		
	public void activatePotion()
	{
		startPopo = Time.time;
		regenHp = 10;
		timeBetweenTic = 2;
		timeofPotion = Time.time + 10f;

	}
			public void stopPotion (){
		regenHp = 2;
				//...
	}
}
