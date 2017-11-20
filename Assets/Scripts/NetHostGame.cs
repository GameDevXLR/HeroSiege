using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
public class NetHostGame :  MonoBehaviour{

		[SerializeField]
		private uint roomSize = 10;
	public GameObject loadingCanvas;
	public Text loadingMessage;
	public Slider loadingBar;
	private bool isCreating;
	public Toggle isItOneLane;
	public string roomName;
	public AudioClip startPlaying;
	public AudioClip classicClic;
	public Image loadColoredBackground;


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
		if (roomName != "" && roomName != null && !GetComponent<NetJoinGame>().isLoadingRooms)
			{
				Debug.Log("Creating Room: " + roomName + " with room for " + roomSize + " players.");
//			LoadingScreenManager.LoadScene (2);
			StartCoroutine (PreventDoubleGame ());
			networkManager.matchMaker.CreateMatch (roomName, roomSize, true, "", "", "", 0, 0, networkManager.OnMatchCreate);
		}
		}
	void FixedUpdate()
	{
		if (isCreating) 
		{
			loadingBar.value += Random.Range (0.05f, 0.20f);
		}
	}

	IEnumerator FadeTo(float aValue, float aTime)
	{
		float alpha = loadColoredBackground.color.a;
		for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
		{
			Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha,aValue,t));
			loadColoredBackground.color = newColor;
			yield return null;
		}
	}
	public IEnumerator PreventDoubleGame()
	{
		isCreating = true;

		GetComponent<AudioSource> ().PlayOneShot (startPlaying);
		loadingCanvas.SetActive (true);
		StartCoroutine(FadeTo(1f,5f));
		yield return new WaitForSecondsRealtime (.5f);

		if (PlayerPrefs.GetString ("LANGAGE") == "Fr") 
		{
			loadingMessage.text = "Construction des constructions.";


		} else 
		{
			loadingMessage.text = "Building the buildings.";
		}
		loadingBar.value += Random.Range(0.1f, 0.5f);
		if (loadingBar.value >= loadingBar.maxValue) 
		{
			loadingBar.value = Random.Range(0.1f, 0.5f);
		}
		yield return new WaitForSecondsRealtime (.5f);
		if (PlayerPrefs.GetString ("LANGAGE") == "Fr") 
		{
			loadingMessage.text = "Construction des constructions..";

		} else 
		{
			loadingMessage.text = "Building the buildings..";
		}
		loadingBar.value += Random.Range(0.1f, 0.5f);
		if (loadingBar.value >= loadingBar.maxValue) 
		{
			loadingBar.value = Random.Range(0.1f, 0.5f);
		}
		yield return new WaitForSecondsRealtime (.5f);
		if (PlayerPrefs.GetString ("LANGAGE") == "Fr") 
		{
			loadingMessage.text = "Construction des constructions...";

		} else 
		{
			loadingMessage.text = "Building the buildings...";
		}
		loadingBar.value += Random.Range(0.1f, 0.5f);
		if (loadingBar.value >= loadingBar.maxValue) 
		{
			loadingBar.value = Random.Range(0.1f, 0.5f);
		}
		yield return new WaitForSecondsRealtime (.5f);
		if (PlayerPrefs.GetString ("LANGAGE") == "Fr") 
		{
			loadingMessage.text = "Dragons enlevés.";

		} else 
		{
			loadingMessage.text = "Removing dragons.";
		}
		loadingBar.value += Random.Range(0.1f, 0.5f);
		if (loadingBar.value >= loadingBar.maxValue) 
		{
			loadingBar.value = Random.Range(0.1f, 0.5f);
		}
		yield return new WaitForSecondsRealtime (2.5f);
		if (PlayerPrefs.GetString ("LANGAGE") == "Fr") 
		{
			loadingMessage.text = "Dragons enlevés..";

		} else 
		{
			loadingMessage.text = "Removing dragons..";
		}
		loadingBar.value += Random.Range(0.1f, 0.5f);
		if (loadingBar.value >= loadingBar.maxValue) 
		{
			loadingBar.value = Random.Range(0.1f, 0.5f);
		}
		yield return new WaitForSecondsRealtime (5f);
		if (PlayerPrefs.GetString ("LANGAGE") == "Fr") 
		{
			loadingMessage.text = "Dragons enlevés...";

		} else 
		{
			loadingMessage.text = "Removing dragons...";
		}
		loadingBar.value += Random.Range(0.2f, 0.7f);
		if (loadingBar.value >= loadingBar.maxValue) 
		{
			loadingBar.value = Random.Range(0.1f, 0.5f);
		}
		yield return new WaitForSecondsRealtime (3f);
		if (PlayerPrefs.GetString ("LANGAGE") == "Fr") 
		{
			loadingMessage.text = "Sac a dos prêt!";

		} else 
		{
			loadingMessage.text = "Packing stuffs.";
		}
		loadingBar.value += Random.Range(0.1f, 0.5f);
		if (loadingBar.value >= loadingBar.maxValue) 
		{
			loadingBar.value = Random.Range(0.1f, 0.5f);
		}
		yield return new WaitForSecondsRealtime (5f);
		if (PlayerPrefs.GetString ("LANGAGE") == "Fr") 
		{
			loadingMessage.text = "En attente de la fin de l'hiver.";

		} else 
		{
			loadingMessage.text = "Waiting for winter to end.";
		}
		loadingBar.value += Random.Range(0.1f, 0.5f);
		if (loadingBar.value >= loadingBar.maxValue) 
		{
			loadingBar.value = Random.Range(0.1f, 0.5f);
		}
		yield return new WaitForSecondsRealtime (15f);
		if (PlayerPrefs.GetString ("LANGAGE") == "Fr") 
		{
			loadingMessage.text = "Echec critique!";

		} else 
		{
			loadingMessage.text = "FAILURE!";
		}
		loadingBar.value += Random.Range(0.1f, 0.5f);
		if (loadingBar.value >= loadingBar.maxValue) 
		{
			loadingBar.value = Random.Range(0.1f, 0.5f);
		}	
		loadColoredBackground.CrossFadeAlpha (0f, 0.1f, true);
		loadingCanvas.SetActive (false);
		StartCoroutine(FadeTo(0f,5f));

		isCreating = false;
	}
	public void ToggleOneLaneTwoLanes()
	{

			networkManager.GetComponent<PlayerMenuSettings> ().isItOneLane = isItOneLane.isOn;

	}

	}

