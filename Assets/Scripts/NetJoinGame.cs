using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using System.Collections;

public class NetJoinGame : MonoBehaviour {

	List<GameObject> roomList = new List<GameObject>();
	public AudioClip refreshSound;

	[SerializeField]
	private Text status;

	[SerializeField]
	private GameObject roomListItemPrefab;

	[SerializeField]
	private Transform roomListParent;

	private NetworkManager networkManager;

	public bool isLoadingRooms;
	void Start ()
	{
		networkManager = ExampleNetworkManager.singleton;
		if (networkManager.matchMaker == null)
		{
			networkManager.StartMatchMaker();
		}

		RefreshRoomList();
	}

	public void RefreshRoomList ()
	{
		GetComponent<AudioSource> ().PlayOneShot (refreshSound);
		if (isLoadingRooms) {
			return;
		}
		ClearRoomList();
		if (networkManager.matchMaker == null)
		{
			networkManager.StartMatchMaker();
		}

		networkManager.matchMaker.ListMatches(0, 20, "", true, 0, 0, OnMatchList);
		status.text = "Loading...";
		if (PlayerPrefs.GetString ("LANGAGE") == "Fr") 
		{
			status.text = "Chargement...";
		}
	}

	public void OnMatchList (bool success, string extendedInfo, List<MatchInfoSnapshot> matchList)
	{
		if (status.text != null) {
			status.text = "";
		}
		if (!success || matchList == null)
		{
			status.text = "Couldn't get room list.";
			if (PlayerPrefs.GetString ("LANGAGE") == "Fr") 
			{
				status.text = "Impossible de récupérer la liste des salons.";
			}
			return;
		}

		foreach (MatchInfoSnapshot match in matchList)
		{
			GameObject _roomListItemGO = Instantiate(roomListItemPrefab);
			_roomListItemGO.transform.SetParent(roomListParent);
			_roomListItemGO.transform.localScale = Vector3.one;

			RoomListItem _roomListItem = _roomListItemGO.GetComponent<RoomListItem>();
			if (_roomListItem != null)
			{
				_roomListItem.Setup(match, JoinRoom);
			}


			// as well as setting up a callback function that will join the game.

			roomList.Add(_roomListItemGO);
		}

		if (roomList.Count == 0)
		{
			status.text = "No rooms at the moment.";
			if (PlayerPrefs.GetString ("LANGAGE") == "Fr") 
			{
				status.text = "Aucun salon pour le moment...";
			}
		}
	}

	void ClearRoomList()
	{
		if (roomList.Count > 0) {
			isLoadingRooms = true;
		}
		for (int i = 0; i < roomList.Count; i++)
		{
			Destroy(roomList[i]);
			if (i == roomList.Count - 2) {
				isLoadingRooms = false;
			}
		}

		roomList.Clear();
	}

	public void JoinRoom (MatchInfoSnapshot _match)
	{
		
		networkManager.matchMaker.JoinMatch(_match.networkId, "", "", "", 0, 0, networkManager.OnMatchJoined);
		StartCoroutine(WaitForJoin());

		StartCoroutine (GetComponent<NetHostGame> ().PreventDoubleGame ());
	}

	IEnumerator WaitForJoin ()
	{
		ClearRoomList();

		int countdown = 10;
		while (countdown > 0)
		{
			if (countdown == 10) 
			{
				status.text = "Establishing connection...";
				if (PlayerPrefs.GetString ("LANGAGE") == "Fr") 
				{
					status.text = "Etablissement de la connection...";
				}
			} 
			if(countdown == 8)
			{
				status.text = "Packing armor and weapon...";
				if (PlayerPrefs.GetString ("LANGAGE") == "Fr") 
				{
					status.text = "Polissage de l'armure...";
				}
			}
			if (countdown <= 5) 
			{
				status.text = "JOINING... (" + countdown + ")";
				if (PlayerPrefs.GetString ("LANGAGE") == "Fr") 
				{
					status.text = "Connection.. (" + countdown + ")";
				}
			}
//			if (countdown == 3) 
//			{
//				LoadingScreenManager.LoadScene (2);
//			}
			//décommenter ci dessus si on veut afficher l'écran de chargement pour ceux qui rejoignent.
			yield return new WaitForSecondsRealtime(1);

			countdown--;
		}

		// Failed to connect
		status.text = "Failed to connect.";
		if (PlayerPrefs.GetString ("LANGAGE") == "Fr") 
		{
			status.text = "Echec de la connection.";
		}
		yield return new WaitForSecondsRealtime(1);

		MatchInfo matchInfo = networkManager.matchInfo;
		if (matchInfo != null)
		{
			networkManager.matchMaker.DropConnection(matchInfo.networkId, matchInfo.nodeId, 0, networkManager.OnDropConnection);
			networkManager.StopHost();
		}

		RefreshRoomList();

	}

}