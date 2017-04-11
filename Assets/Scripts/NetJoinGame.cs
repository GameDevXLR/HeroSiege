using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using System.Collections;

public class NetJoinGame : MonoBehaviour {

	List<GameObject> roomList = new List<GameObject>();

	[SerializeField]
	private Text status;

	[SerializeField]
	private GameObject roomListItemPrefab;

	[SerializeField]
	private Transform roomListParent;

	private NetworkManager networkManager;

	void Start ()
	{
		networkManager = NetworkManager.singleton;
		if (networkManager.matchMaker == null)
		{
			networkManager.StartMatchMaker();
		}

		RefreshRoomList();
	}

	public void RefreshRoomList ()
	{
		ClearRoomList();

		if (networkManager.matchMaker == null)
		{
			networkManager.StartMatchMaker();
		}

		networkManager.matchMaker.ListMatches(0, 20, "", true, 0, 0, OnMatchList);
		status.text = "Loading...";
	}

	public void OnMatchList (bool success, string extendedInfo, List<MatchInfoSnapshot> matchList)
	{
		if (status.text != null) {
			status.text = "";
		}
		if (!success || matchList == null)
		{
			status.text = "Couldn't get room list.";
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
		}
	}

	void ClearRoomList()
	{
		for (int i = 0; i < roomList.Count; i++)
		{
			Destroy(roomList[i]);
		}

		roomList.Clear();
	}

	public void JoinRoom (MatchInfoSnapshot _match)
	{
		
		networkManager.matchMaker.JoinMatch(_match.networkId, "", "", "", 0, 0, networkManager.OnMatchJoined);
		StartCoroutine(WaitForJoin());
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
			} 
			if(countdown == 8)
			{
				status.text = "Packing armor and weapon...";
			}
			if (countdown <= 5) 
			{
				status.text = "JOINING... (" + countdown + ")";
			}
//			if (countdown == 3) 
//			{
//				LoadingScreenManager.LoadScene (2);
//			}
			//décommenter ci dessus si on veut afficher l'écran de chargement pour ceux qui rejoignent.
			yield return new WaitForSeconds(1);

			countdown--;
		}

		// Failed to connect
		status.text = "Failed to connect.";
		yield return new WaitForSeconds(1);

		MatchInfo matchInfo = networkManager.matchInfo;
		if (matchInfo != null)
		{
			networkManager.matchMaker.DropConnection(matchInfo.networkId, matchInfo.nodeId, 0, networkManager.OnDropConnection);
			networkManager.StopHost();
		}

		RefreshRoomList();

	}

}