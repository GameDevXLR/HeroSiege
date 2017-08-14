using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalGameManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GameManager.instanceGM.playerObj.GetComponent<SetLocalAutorities>().CmdSetAuthority(gameObject);
	}


	
}
