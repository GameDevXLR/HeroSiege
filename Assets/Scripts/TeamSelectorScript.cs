using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class TeamSelectorScript : NetworkBehaviour 
{

	//ce script va sur un collider trigger qui doit etre placé la ou spawn les joueurs.
	//en fonction du TeamNbr; le joueur qui collide est assigné a l'équipe correspondante.
	//une fois la partie démarrée (difficulté choisie): l'objet est détruit.

	public int teamNbr; // DOIT ETRE COMPLETER : détermine a quel team appartiendra l'objet qui entrera en collision.
	public Sprite team1Icon;
	public Sprite team2Icon;
	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player") 
		{

			GameManager.instanceGM.AddPlayerToTeam (teamNbr, other.GetComponent<NetworkIdentity>().netId);
			if (other.gameObject == GameManager.instanceGM.playerObj) 
			{
				if (teamNbr == 1) 
				{
					GameManager.instanceGM.isTeam1 = true;
					other.gameObject.GetComponent<PlayerXPScript> ().isTeam1 = true;

					other.gameObject.GetComponent<GenericLifeScript>().respawnPoint = GameObject.Find ("PlayerRespawnPointT1");
					return;

				} else 
				{
					GameManager.instanceGM.isTeam2 = true;
					other.gameObject.GetComponent<GenericLifeScript>().respawnPoint = GameObject.Find ("PlayerRespawnPointT2");
					return;
				}
			}
			if (teamNbr == 1) 
			{
				other.gameObject.GetComponent<GenericLifeScript>().respawnPoint = GameObject.Find ("PlayerRespawnPointT1");
				other.gameObject.GetComponent<PlayerXPScript> ().isTeam1 = true;
				other.gameObject.GetComponent<PlayerManager> ().myTeamSprite = team1Icon;
				return;

			} else 
			{
				other.gameObject.GetComponent<GenericLifeScript>().respawnPoint = GameObject.Find ("PlayerRespawnPointT2");
				other.gameObject.GetComponent<PlayerManager> ().myTeamSprite = team2Icon;

			}
		}		
	}

}
