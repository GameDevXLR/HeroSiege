using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
	[ServerCallback]
	public void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == Layers.Player) 
		{
			playersIn.Add (other.gameObject);
		}
		if (other.gameObject.layer == Layers.Ennemies) 
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
			if (other.gameObject.layer == Layers.Ennemies && playersIn.Count == 0) 
			{
				timeToCapture -= Time.smoothDeltaTime;
				if (timeToCapture <= 0f) 
				{
					timeToCapture = initialTimeToCapt;
					belongsTo = PointOwner.neutral;

				}

			}
			break;
		case PointOwner.team2:
			break;
		default:
			break;
		}
		
	}

	public void ChangeOwner(PointOwner newOwner)
	{
		belongsTo = newOwner;
		if (canBeOwnedBy == newOwner) 
		{
			GetComponentInChildren<ShopScript> ().isAccessible = true;
			GameManager.instanceGM.messageManager.SendAnAlertMess ("The camp has been captured.", Color.green);
		} else 
		{
			GetComponentInChildren<ShopScript> ().isAccessible = false;
			GetComponentInChildren<ShopScript> ().CloseYourMenu ();
			GameManager.instanceGM.messageManager.SendAnAlertMess ("The camp has been lost.", Color.red);

		}
	}
		
}
