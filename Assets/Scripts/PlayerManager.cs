using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerManager : NetworkBehaviour 
{
	[SyncVar(hook = "ActualizeKillCount")]public int killCount;
//	[SyncVar(hook= "ActualizeMyNN")]
	public string playerNickname;
	public Text playerNNTxt;
	public Text playerKillsTxt;
	public Text playerLifeTxt; // en gros : 200/250 par exemple.
	public Text playerManaTxt;
	public Transform playerLifeBar; // a rescale en fonction des pv. lié au script Genericlifescript.
	public Transform playerManaBar; //a noté que c'est que pour les alliés.

	public GameObject UIPrefab;
	public GameObject playerUI;
	public Transform playersStatsView;

	public void Start()
	{
		playersStatsView = GameObject.Find ("PlayersStatsView").transform;
		if (!isLocalPlayer) 
		{
			SpawnPlayerUI ();
			playerUI.transform.GetChild (0).GetComponent<Text> ().text = playerNickname;
			//systeme de nom provisoire juste pour distinguer : en attendant le menu avant jeu.
		}
		ActualizeKillCount (killCount);

	}

	public void SpawnPlayerUI()
	{

		playerUI = Instantiate (UIPrefab, playersStatsView, true);
		playerKillsTxt = playerUI.transform.GetChild (2).GetComponent<Text> ();
		playerNNTxt = playerUI.transform.GetChild (0).GetComponent<Text> ();
		playerUI.transform.localScale = Vector3.one;
		playerLifeTxt = playerUI.transform.Find ("AllyLifeBarMain/AllyHPTxt").GetComponent<Text> ();
		playerLifeBar = playerUI.transform.Find ("AllyLifeBarMain/AllyActualLifeBar").transform;
		playerManaTxt = playerUI.transform.Find ("AllyManaBarMain/AllyMpText").GetComponent<Text> ();
		playerManaBar = playerUI.transform.Find ("AllyManaBarMain/AllyActualManaBar").transform;
		playerLifeTxt.text = "250 / 250";
		playerManaTxt.text = "150 / 150";
		playerKillsTxt.text = "0";
	}

	public void ActualizeKillCount(int kills)
	{
		killCount = kills;
		if (isLocalPlayer) 
		{
			return;
		}
		playerKillsTxt.text = killCount.ToString ();
	}

//	public void ActualizeMyNN(string NN)
//	{
//		playerNickname = NN;
//		playerUI.transform.GetChild (0).GetComponent<Text> ().text = NN;
//	}

	public void OnDestroy()
	{
		Destroy (playerUI);
	}
}
