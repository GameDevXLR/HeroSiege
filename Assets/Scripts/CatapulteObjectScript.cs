using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CatapulteObjectScript : NetworkBehaviour
{

	[SyncVar (hook = "ShowTheAnimCata")]public int chargesLoaded;
	public float actualCD;
	public float spellCD;
	public bool isBeingUsed;
	public GameObject userOfCata;


	[ServerCallback]
	public void OnTriggerEnter(Collider other)
	{
		if (isBeingUsed) 
		{
			return;
		}
		if (other.gameObject.layer == 8 && other.gameObject.tag == "Player") // double check pour etre bien sur pour plus tard...
		{ 
			isBeingUsed = true;
			userOfCata = other.gameObject;
			ActivatePlayerBtn ();
		}
	}
	[ServerCallback]
	public void OnTriggerExit(Collider other)
	{
		if (other.gameObject == userOfCata) 
		{
			DesactivatePlayerBtn ();
		}
	}

	public void ActivatePlayerBtn ()
	{
		userOfCata.GetComponent<PlayerCastCatapulte> ().RpcActivateCata (chargesLoaded);
		userOfCata.GetComponent<PlayerCastCatapulte> ().cataObj = gameObject;

	}

	public void DesactivatePlayerBtn()
	{
		isBeingUsed = false;
		chargesLoaded = userOfCata.GetComponent<PlayerCastCatapulte> ().cataCharges;
		userOfCata.GetComponent<PlayerCastCatapulte> ().RpcDesactivateCata ();
	}

	public void ShowTheAnimCata(int ch)
	{
		chargesLoaded = ch;
//		GetComponentInChildren<Animator> ().Play ("TirCatapulte"); // voir pour faire jouer ici l'anime cata.
	}
}
