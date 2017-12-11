using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobGetCollider : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		int i = 0;

		if (GetComponentInParent<MinionsPathFindingScript> ().isTeam1) 
		{
			foreach (GameObject player in GameManager.instanceGM.team1Players) 
			{
				GetComponent<ParticleSystem> ().trigger.SetCollider (i++, player.GetComponent<MeshCollider> ());
			}
		} 

		else 
		{
			foreach (GameObject player in GameManager.instanceGM.team2Players) 
			{
				GetComponent<ParticleSystem> ().trigger.SetCollider (i++, player.GetComponent<MeshCollider> ());
			}
			
		}

	}
	
}