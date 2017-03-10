using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;


public class GameManager : NetworkBehaviour 
{

	public AudioClip LooseLifeSound;
	public Text generalTxt;
	public Text gameOverTxt;
	public static GameManager instanceGM = null;
	private GameObject[] ennemies;
	public GameObject playerObj;
	public NetworkInstanceId ID;
	[SyncVar(hook = "LooseLife")]public int lifeOfTheTeam = 5;

	[SyncVar]public int gameDifficulty = 1;

	//on s'assure en Awake que le script est bien unique. sinon on détruit le nouvel arrivant.
	void Awake(){
		if (instanceGM == null) {
			instanceGM = this;
			
		} else if (instanceGM != this) 
		{
			Destroy (gameObject);
		}
		
	}
	void Start()
	{
		generalTxt = GameObject.Find ("GeneralText").GetComponent<Text>();
	}
	public void LooseLife(int life)
	{
		lifeOfTheTeam = life;
		StartCoroutine (LooseALifeAlert ());
	}

	public void LooseALife()
	{
		lifeOfTheTeam -= 1 ;
		GetComponent<AudioSource> ().PlayOneShot (LooseLifeSound);
		if (lifeOfTheTeam <= 0)
		{
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
		if (isServer) 
		{
			NetworkManager.singleton.ServerChangeScene ("scene2");		
		}

	}
	IEnumerator LooseALifeAlert()
	{
		generalTxt.enabled = true;
		generalTxt.text = lifeOfTheTeam + " vie(s) restante...";
		yield return new WaitForSeconds (2f);
		generalTxt.text = "";
		generalTxt.enabled = false;
	}

	public void StopPlayerFromJoining()
	{
//		NetworkManager.singleton.matchMaker.SetMatchAttributes(UnityEngine.Networking.Types.NetworkID, false, 1, basic
	}
}