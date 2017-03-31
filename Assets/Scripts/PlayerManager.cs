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
	public GameObject UIPrefab;
	public GameObject playerUI;
	public Transform playersStatsView;

	public void Start()
	{
		playersStatsView = GameObject.Find ("PlayersStatsView").transform;
		SpawnPlayerUI ();
		ActualizeKillCount (killCount);

		//systeme de nom provisoire juste pour distinguer : en attendant le menu avant jeu.
		playerUI.transform.GetChild (0).GetComponent<Text> ().text = playerNickname;
	}

	public void SpawnPlayerUI()
	{

		playerUI = Instantiate (UIPrefab, playersStatsView, true);
		playerUI.transform.localScale = Vector3.one;
		
	}

	public void ActualizeKillCount(int kills)
	{
		killCount = kills;
		playerUI.transform.GetChild (1).GetComponent<Text> ().text = killCount.ToString ();
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
