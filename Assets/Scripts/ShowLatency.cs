using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ShowLatency : NetworkBehaviour {

	private NetworkClient nClient;
	private int latency;
	private Text latencyDisplay;
	// Use this for initialization
//	public override void OnStartLocalPlayer ()
	void Start()
	{
		if (isLocalPlayer)
		{
			nClient = GameObject.Find ("NetworkManagerObj").GetComponent<NetworkManager> ().client;
			latencyDisplay = GameObject.Find ("LatencyTxt").GetComponent<Text> ();
		}
	}

	
	// Update is called once per frame
	void Update () 
	{
		if (isLocalPlayer) 
		{
			ShowLat ();
		}
	}

	void ShowLat()
	{
		latency = nClient.GetRTT ();
		latencyDisplay.text = latency.ToString ();
	}
}
