using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

public class PlayerEnnemyDetectionScript : NetworkBehaviour {
	//permet au perso du joueur de cibler automatiquement l'ennemi le plus proche.

//	public bool isTheLocalP;
	public bool autoTargetting;
	public PlayerAutoAttack autoAScript;
	public NavMeshAgent playerAgent;
	// Use this for initialization

	void Start () 
	{
		autoAScript = GetComponentInParent<PlayerAutoAttack> ();
		playerAgent = GetComponentInParent<NavMeshAgent> ();
//		if (GameManager.instanceGM.playerObj == gameObject.transform.parent.gameObject) 
//		{
//			autoTargetting = true;
//			isTheLocalP = true;
//		}
//
	}
	
	// Update is called once per frame
	void OnTriggerStay (Collider other)
	{
		if (GetComponentInParent<NetworkIdentity>().isServer) 
		{
			if (autoTargetting) 
			{
				if (playerAgent.velocity == Vector3.zero) 
				{
					if (other.gameObject.layer == 9) 
					{
						if (GetComponentInParent<PlayerIGManager>().heroChosen =="Ovate" && !other.gameObject.GetComponent<EnnemyIGManager> ().myEnemies.Contains (autoAScript.gameObject)) 
						{
							other.gameObject.GetComponent<EnnemyIGManager> ().myEnemies.Add (autoAScript.gameObject);
						}
						if (autoAScript.target == null) {
							if (!other.GetComponent<EnnemyIGManager> ().isDead) 
							{
								TellHeroHisDest (other.gameObject.GetComponent<NetworkIdentity> ().netId);
							}
						}
					}
				}
			}
		}
	}

	public void TellHeroHisDest(NetworkInstanceId id)
	{

		GetComponentInParent<PlayerClicToMove> ().SetThatTargetFromAggro(id);
	} 
}
