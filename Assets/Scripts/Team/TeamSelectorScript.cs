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

	GameObject spawnPointT1;
	GameObject spawnPointT2;
	void Start()
	{
		spawnPointT1 = GameObject.Find ("PlayerRespawnPointT1");
		spawnPointT2 = GameObject.Find ("PlayerRespawnPointT2");
	}
	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player") 
		{
			GameManager.instanceGM.AddPlayerToTeam (teamNbr, other.gameObject);
			//en gros si t'es le local player:
			if (other.gameObject == GameManager.instanceGM.playerObj) 
			{
				if (teamNbr == 1) 
				{
					other.gameObject.GetComponent<PlayerXPScript> ().isTeam1 = true;
					GameManager.instanceGM.isTeam1 = true;
                    GameManager.instanceGM.isTeam2 = false;
                    other.gameObject.GetComponent<TeamManageServer>().sendTeam(other.gameObject.GetComponent<PlayerManager>().playerNickname, 1);
                    //NetworkUtils.Instance.addConn(other.GetComponent<NetworkIdentity>().connectionToClient, 1);

					other.gameObject.GetComponent<PlayerIGManager>().respawnPoint = spawnPointT1;
					return;

				} else 
				{
					other.gameObject.GetComponent<PlayerXPScript>().isTeam1 = false;
                    GameManager.instanceGM.isTeam2 = true;
                    GameManager.instanceGM.isTeam1 = false;
                    other.gameObject.GetComponent<TeamManageServer>().sendTeam(other.gameObject.GetComponent<PlayerManager>().playerNickname,2);
                    
					other.gameObject.GetComponent<PlayerIGManager>().respawnPoint = spawnPointT2;
					return;
				}
			}
			//si t'es pas le local player mais un autre joueur:
			if (teamNbr == 1) 
			{
				other.gameObject.GetComponent<PlayerXPScript> ().isTeam1 = true;
				other.gameObject.GetComponent<PlayerIGManager>().respawnPoint = spawnPointT1;
				other.gameObject.GetComponent<PlayerManager> ().myTeamSprite = team1Icon;
				return;

			} else 
			{
				other.gameObject.GetComponent<PlayerXPScript>().isTeam1 = false;
				other.gameObject.GetComponent<PlayerIGManager>().respawnPoint = spawnPointT1;
				other.gameObject.GetComponent<PlayerManager> ().myTeamSprite = team2Icon;

            }
		}		
	}

}
