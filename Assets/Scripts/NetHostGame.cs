using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
public class NetHostGame :  MonoBehaviour{

		[SerializeField]
		private uint roomSize = 10;

	private bool isCreating;
	public Toggle isItOneLane;
	public string roomName;
	public AudioClip startPlaying;
	public AudioClip classicClic;


		private NetworkManager networkManager;

		void Start ()
		{
//		roomName = "Default";
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
		if (isCreating) 
		{
			return;
		}
			if (roomName != "" && roomName != null)
			{
				Debug.Log("Creating Room: " + roomName + " with room for " + roomSize + " players.");
//			LoadingScreenManager.LoadScene (2);
			StartCoroutine (PreventDoubleGame ());
			networkManager.matchMaker.CreateMatch (roomName, roomSize, true, "", "", "", 0, 0, networkManager.OnMatchCreate);
		}
		}

	IEnumerator PreventDoubleGame()
	{
		isCreating = true;
		GetComponent<AudioSource> ().PlayOneShot (startPlaying);
		yield return new WaitForSeconds (1f);
		isCreating = false;
	}
	public void ToggleOneLaneTwoLanes()
	{

			networkManager.GetComponent<PlayerMenuSettings> ().isItOneLane = isItOneLane.isOn;

	}

	}

