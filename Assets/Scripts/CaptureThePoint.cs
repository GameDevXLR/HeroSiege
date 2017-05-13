using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HyperLuminalGames;
using UnityEngine.Networking;

public class CaptureThePoint : NetworkBehaviour 



{
	public enum PointOwner
	{
		team1,
		team2,
		neutral
	}


	[SyncVar(hook="ChangeOwner")]public PointOwner belongsTo;
	public PointOwner canBeOwnedBy = PointOwner.team1;
	public List<GameObject> playersIn;
	public List<GameObject> enemiesIn;
	public float timeToCapture;
	private float timeCaptureStart;
	private float initialTimeToCapt;
	public AudioClip Capture;
	[SyncVar(hook = "SyncOutpostTimer")]int tmpTime;
	public bool haveGivenTip0;

	public bool haveGivenTip;
	// Use this for initialization
	void Start () 
	{		
		GetComponentInChildren<ShopScript> ().isAccessible = false;
		initialTimeToCapt = timeToCapture;

	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
	public void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == Layers.Player) 
		{
			if (isServer) {
				playersIn.Add (other.gameObject);
			}
			if (other.GetComponent<NetworkIdentity> ().isLocalPlayer) 
			{
				if (!haveGivenTip0) 
				{
					GameManager.instanceGM.ShowAGameTip ("kill all enemies in the outpost area to be able to capture it.");
					haveGivenTip0 = true;
				}
			}
		}
		if (other.gameObject.layer == Layers.Ennemies && isServer) 
		{
			enemiesIn.Add (other.gameObject);
		}
		
	}
	[ServerCallback]
	public void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer == Layers.Player) 
		{
			playersIn.Remove (other.gameObject);
		}
		if (other.gameObject.layer == Layers.Ennemies) 
		{
			enemiesIn.Remove (other.gameObject);
		}
		
	}



	[ServerCallback]
	public void OnTriggerStay(Collider other)
	{
		if (timeToCapture < 10f && timeToCapture > 0f) 
		{
			//a opti : ca run 2 fois sur le serveur et le changement de variable / recalcul est trop fréquent mais bon...ya pas 1000 fois le script.
			tmpTime = (int)timeToCapture;
			GetComponent<Location> ().Display_2_Text = tmpTime.ToString ();
		} else 
		{
			GetComponent<Location> ().Display_2_Text = "";
		}
		switch (belongsTo) 
		{
		case PointOwner.neutral:
			if (enemiesIn.Count > 0) 
			{
				enemiesIn.RemoveAll ((GameObject obj) => obj == null);
			}
			if (other.gameObject.layer == Layers.Player && enemiesIn.Count == 0) 
			{
				timeToCapture -= Time.smoothDeltaTime;
				if (timeToCapture <= 0f) 
				{
					timeToCapture = initialTimeToCapt;
					belongsTo = canBeOwnedBy;
				}

			}
			break;
		case PointOwner.team1:
			if (other.gameObject.layer == Layers.Ennemies) 
			{
				if (playersIn.Count > 0) 
				{
					playersIn.ForEach (CheckIfPlayersAreAlive);
				}
				if(playersIn.Count == 0)
				{
					timeToCapture -= Time.smoothDeltaTime;
					if (timeToCapture <= 0f) 
					{
						timeToCapture = initialTimeToCapt;
						belongsTo = PointOwner.neutral;
					}
				}

			}
			break;
		case PointOwner.team2:
			if (other.gameObject.layer == Layers.Ennemies) 
			{
				if (playersIn.Count > 0) 
				{
					playersIn.ForEach (CheckIfPlayersAreAlive);
				}
				if(playersIn.Count == 0)
				{
					timeToCapture -= Time.smoothDeltaTime;
					if (timeToCapture <= 0f) 
					{
						timeToCapture = initialTimeToCapt;
						belongsTo = PointOwner.neutral;
					}
				}

			}
			break;
		default:
			break;
		}
		
	}

	public void CheckIfPlayersAreAlive(GameObject player)
	{
		if (player.GetComponent<GenericLifeScript> ().isDead) 
		{
			playersIn.Remove (player);
		}
		
	}
		
	public void ChangeOwner(PointOwner newOwner)
	{
		GameManager.instanceGM.ActualizeLocSystem ();
		belongsTo = newOwner;
		if (canBeOwnedBy == newOwner) 
		{
			GetComponent<AudioSource> ().PlayOneShot (Capture);
			GetComponent<Location> ().IconColour = Color.green;
			GetComponentInChildren<ShopScript> ().isAccessible = true;
			transform.GetChild (0).GetComponent<Location> ().enabled = true;
			if (GameManager.instanceGM.isTeam1) 
			{
				if (belongsTo == PointOwner.team1) 
				{
					GameManager.instanceGM.messageManager.SendAnAlertMess ("Our outpost has been captured.", Color.green);
				} else 
				{
					GameManager.instanceGM.messageManager.SendAnAlertMess ("The enemy outpost has been captured.", Color.red);
				}
			} else 
			{
				if (belongsTo == PointOwner.team1) 
				{
					GameManager.instanceGM.messageManager.SendAnAlertMess ("The enemy outpost has been captured.", Color.red);
				} else 
				{
					GameManager.instanceGM.messageManager.SendAnAlertMess ("Our outpost has been captured.", Color.green);
				}
			}
		}else 
		{
			GetComponent<Location> ().IconColour = Color.yellow;
			transform.GetChild (0).GetComponent<Location> ().enabled = false;
			GetComponentInChildren<ShopScript> ().isAccessible = false;
			GetComponentInChildren<ShopScript> ().CloseYourMenu ();
			if (GameManager.instanceGM.isTeam1) 
			{
				if (belongsTo == PointOwner.team1) 
				{
					GameManager.instanceGM.messageManager.SendAnAlertMess ("Our outpost has been lost.", Color.red);
				} else 
				{
					GameManager.instanceGM.messageManager.SendAnAlertMess ("The enemy outpost has been lost!", Color.green);
				}
			} else 
			{
				if (belongsTo == PointOwner.team1) 
				{
					GameManager.instanceGM.messageManager.SendAnAlertMess ("The enemy outpost has been lost", Color.green);
				} else 
				{
					GameManager.instanceGM.messageManager.SendAnAlertMess ("Our outpost has been lost!", Color.red);
				}
			}
		}
	}

	public void SyncOutpostTimer(int t)
	{
		if (!haveGivenTip) 
		{
			GameManager.instanceGM.ShowAGameTip ("Capture the outpost to get acess to his shop and be able to increase the strenght of your daily boss");
			haveGivenTip = true;
		}
		if (!isServer) {
			tmpTime = t;
			if (t < 10f && t > 0f) {
				GetComponent<Location> ().Display_2_Text = tmpTime.ToString ();
			} else {
				GetComponent<Location> ().Display_2_Text = "";
			}
		}
	}
		
}
