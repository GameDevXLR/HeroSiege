using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerManager : NetworkBehaviour 
{
	[SyncVar(hook = "ActualizeKillCount")]public int killCount;
	[SyncVar(hook = "ActuPlayerLvl")]public int playerLvl = 1; // le playerxpscript augmente ca quand c'est bon.
	public Text playerLvlDisplay;
//	[SyncVar(hook= "ActualizeMyNN")]
	public string playerNickname;
	public Text playerNNTxt;
	public Text playerDeathsTxt;
	public Text playerKillsTxt;
	public Text playerLifeTxt; // en gros : 200/250 par exemple.
	public Text playerManaTxt;
	public Transform playerLifeBar; // a rescale en fonction des pv. lié au script Genericlifescript.
	public Transform playerManaBar; //a noté que c'est que pour les alliés.
	public Image avatarImg;
	public Image deadAvatarImg;
	public GameObject pingPrefab;
	public GameObject UIPrefab;
	public GameObject playerUI;
	public Transform playersStatsView;
//    public GameObject minimap;
	[Header("Selec hero screen")]
	public GameObject playerSelecPrefab;
	public GameObject playerSelecUI;
	public Image playerSelecHeroChosenImg;
	public Transform team1SelecScreenPanel;
	public Transform team2SelecScreenPanel;
	public Sprite myTeamSprite;
	public Image myTeamIcon;

	public void Start()
	{
		playersStatsView = GameObject.Find ("PlayersStatsView").transform;
		team1SelecScreenPanel = GameObject.Find ("BlueTeamSelecPanel").transform;
		team2SelecScreenPanel = GameObject.Find ("RedTeamSelecPanel").transform;
//		minimap = GameObject.Find("minimap");
		if (!isLocalPlayer) 
		{
			StartCoroutine (DelayTheMainSelecHeroUI ());
//			playerUI.transform.GetChild (0).GetComponent<Text> ().text = playerNickname;
			//systeme de nom provisoire juste pour distinguer : en attendant le menu avant jeu.
		}else
		{
			StartCoroutine (DelayTheSelecHeroUI ());
			playerKillsTxt = GameObject.Find ("KillsCountLog").GetComponent<Text> ();
			playerDeathsTxt = GameObject.Find ("DeathsCountLog").GetComponent<Text> ();
			playerLvlDisplay = GameObject.Find ("PlayerLevel").GetComponent<Text> ();
			avatarImg = GameObject.Find ("PlayerAvatarImg").GetComponent<Image> ();
		}
		ActualizeKillCount (killCount);

	}
	IEnumerator DelayTheMainSelecHeroUI()
	{
		yield return new WaitForSeconds (2f);
		SpawnPlayerUI ();

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
		myTeamIcon = playerUI.transform.Find ("allyTeam").GetComponent<Image> ();
		playerDeathsTxt = playerUI.transform.Find ("AllyDeathsTxt").GetComponent<Text> ();
		playerLvlDisplay = playerUI.transform.Find ("AllyLvl").GetComponent<Text> ();
		avatarImg = playerUI.transform.Find ("allyAvatar").GetComponent<Image> ();
		deadAvatarImg = playerUI.transform.Find ("allyDeadAvatar").GetComponent<Image> ();
		playerLifeTxt.text = "250 / 250";
		playerManaTxt.text = "100 / 100";
		playerKillsTxt.text = "0";
		StartCoroutine (DelayTheUIActu ());
	}

	IEnumerator DelayTheSelecHeroUI()
	{
		yield return new WaitForSeconds (2f);
		SpawnPlayerSelecUI ();

	}

	public void SpawnPlayerSelecUI()
	{
		if (GetComponent<PlayerXPScript> ().isTeam1) 
		{
			playerSelecUI = Instantiate (playerSelecPrefab, team1SelecScreenPanel, true);
		} else 
		{
			playerSelecUI = Instantiate (playerSelecPrefab, team2SelecScreenPanel, true);

		}
		playerSelecHeroChosenImg = playerSelecUI.transform.Find("Avatar").GetComponent<Image>();
		playerSelecUI.GetComponentInChildren<Text> ().text = playerNickname;
		playerSelecUI.transform.localScale = new Vector3 (1f, 1f, 1f);
	}

	IEnumerator DelayTheUIActu()
	{
		yield return new WaitForSeconds (1.5f);
		myTeamIcon.sprite = myTeamSprite;
		SpawnPlayerSelecUI ();
	}

	public void ActualizeKillCount(int kills)
	{
		killCount = kills;
		if (playerKillsTxt) {
			playerKillsTxt.text = killCount.ToString ();
		}
	}

	public void ActuPlayerLvl(int lvl)
	{
		playerLvl = lvl;
		if (!isLocalPlayer) 
		{
			playerLvlDisplay.text = lvl.ToString ();
		}
	}

	public void OnDestroy()
	{
		Destroy (playerUI);
	}

    // ping
    public void recevePingPosition(Vector3 pingPos)
    {
        CmdSendAPing(pingPos);
    }

    [Command]
    public void CmdSendAPing(Vector3 pingPos)
    {
		GameObject ping = Instantiate(pingPrefab, pingPos, Quaternion.Euler(new Vector3(90f,0f,0f))) as GameObject; 
        NetworkServer.Spawn(ping);

    }

    
}
