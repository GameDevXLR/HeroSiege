using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
	public Text gameOverTxt;
	public static GameManager instanceGM = null;
	public GameObject[] ennemies;


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
		if (lifeOfTheTeam <= 0)
		{
			Debug.Log ("GameOver");
			ennemies = GameObject.FindObjectsOfType<GameObject> ();
			foreach (GameObject g in ennemies) 
			{
				if (g.layer == 9) 
				{
					Destroy (g);
				}
				if (g.layer == 8) 
				{
					g.SetActive (false);
				}
			}
			StartCoroutine (RestartTheLevel ());

			//faire ici ce qui doit se passer si on a pu de vie et que la partie est donc finie.
		}
	}

	IEnumerator RestartTheLevel()
	{
		gameOverTxt.enabled = true;
		yield return new WaitForSeconds (3f);
		SceneManager.LoadSceneAsync (0);

	}
}
