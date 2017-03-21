using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;


public class PersonalNetworkManager : NetworkLobbyManager {

	public override void OnLobbyClientSceneChanged(NetworkConnection conn)
	{
		Debug.Log("OnLobbyClientSceneChanged connectionId : " + conn.connectionId);

		base.OnLobbyClientSceneChanged(conn);
	}

}
