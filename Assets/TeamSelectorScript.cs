﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class TeamSelectorScript : NetworkBehaviour 
{

	public int teamNbr; // DOIT ETRE COMPLETER : détermine a quel team appartiendra l'objet qui entrera en collision.

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player") 
		{

			GameManager.instanceGM.AddPlayerToTeam (teamNbr, other.GetComponent<NetworkIdentity>().netId);
			if (other.gameObject == GameManager.instanceGM.playerObj) {
				if (teamNbr == 1) 
				{
					GameManager.instanceGM.isTeam1 = true;
					other.gameObject.GetComponent<GenericLifeScript>().respawnPoint = GameObject.Find ("PlayerRespawnPointT1");

				} else 
				{
					GameManager.instanceGM.isTeam2 = true;
					other.gameObject.GetComponent<GenericLifeScript>().respawnPoint = GameObject.Find ("PlayerRespawnPointT2");

				}
			}
		}
	}

}
