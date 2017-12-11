using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobGetCollider : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		GetComponent<ParticleSystem> ().trigger.SetCollider (0, GameManager.instanceGM.playerObj.GetComponent<MeshCollider>());
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
