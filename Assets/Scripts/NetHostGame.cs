using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NetHostGame :  MonoBehaviour{

		[SerializeField]
		private uint roomSize = 10;

		private string roomName;

		private NetworkManager networkManager;

		void Start ()
		{
		roomName = "Default";
			networkManager = NetworkManager.singleton;
			if (networkManager.matchMaker == null)
			{
				networkManager.StartMatchMaker();
			}
		}

		public void SetRoomName (string _name)
		{
			roomName = _name;
		}

		public void CreateRoom ()
		{
			if (roomName != "" && roomName != null)
			{
				Debug.Log("Creating Room: " + roomName + " with room for " + roomSize + " players.");
				networkManager.matchMaker.CreateMatch (roomName, roomSize, true, "", "", "", 0, 0, networkManager.OnMatchCreate);
			}
		}

	}

