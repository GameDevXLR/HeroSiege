using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	public static GameManager instanceGM = null;


	//on s'assure en Awake que le script est bien unique. sinon on détruit le nouvel arrivant.
	void Awake(){
		if (instanceGM == null) {
			instanceGM = this;
			
		} else if (instanceGM != this) 
		{
			Destroy (gameObject);
		}
		
	}
	public int lifeOfTheTeam = 5;

	public void LooseALife()
	{
		lifeOfTheTeam -= 1;
		if (lifeOfTheTeam <=0)
		{
			Debug.Log ("GameOver");
			//faire ici ce qui doit se passer si on a pu de vie et que la partie est donc finie.
		}
	}
}
