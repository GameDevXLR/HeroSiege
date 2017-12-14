﻿using UnityEngine;
using System.Collections;
using NATTraversal;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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
		loadingCanvas = GameObject.Find ("LoadingCanvas");
		loadColoredBackground = loadingCanvas.transform.Find ("FondLoadingColored").GetComponent<Image>();
		loadingMessage = loadingCanvas.transform.Find ("LoadingPanel").transform.Find ("LoadText").GetComponent<Text> ();
		loadingBar = loadingCanvas.transform.Find ("LoadingPanel").transform.Find ("LoadingBar").GetComponent<Slider> ();

		loadingCanvas.GetComponent<Canvas> ().enabled = true;
		loadingCanvas.SetActive (false);

		networkManager = (NetworkManager) NATTraversal.NetworkManager.singleton;
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
//			networkManager.matchMaker.CreateMatch (roomName, roomSize, true, "", "", "", 0, 0, networkManager.OnMatchCreate);
			networkManager.StartHostAll(roomName, roomSize, true,"", 0,0,0, networkManager.OnMatchCreate);

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
			loadingMessage.text = "Utilisez clic droit pour vous déplacer, clic gauche pour attaquer.";


		} else 
		{
			loadingMessage.text = "Use right clic to move, left clic to attack.";
		}
		loadingBar.value += Random.Range(0.1f, 0.5f);
		if (loadingBar.value >= loadingBar.maxValue) 
		{
			loadingBar.value = Random.Range(0.1f, 0.5f);
		}
		yield return new WaitForSecondsRealtime (.5f);
		if (PlayerPrefs.GetString ("LANGAGE") == "Fr") 
		{
			loadingMessage.text = "Les catapultes doivent être recapturé tous les matins.";

		} else 
		{
			loadingMessage.text = "Catapults need to be capture every morning.";
		}
		loadingBar.value += Random.Range(0.1f, 0.5f);
		if (loadingBar.value >= loadingBar.maxValue) 
		{
			loadingBar.value = Random.Range(0.1f, 0.5f);
		}
		yield return new WaitForSecondsRealtime (.5f);
		if (PlayerPrefs.GetString ("LANGAGE") == "Fr") 
		{
			loadingMessage.text = "Elles tirent sur la team adverse si capturés.";

		} else 
		{
			loadingMessage.text = "They will fire on the enemy team if captured.";
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
			loadingMessage.text = "Nourrissez les bêtes depuis le magasin de l'avant-poste.";

		} else 
		{
			loadingMessage.text = "Feed the beasts in the outpost shop.";
		}
		loadingBar.value += Random.Range(0.1f, 0.5f);
		if (loadingBar.value >= loadingBar.maxValue) 
		{
			loadingBar.value = Random.Range(0.1f, 0.5f);
		}
		yield return new WaitForSecondsRealtime (5f);
		if (PlayerPrefs.GetString ("LANGAGE") == "Fr") 
		{
			loadingMessage.text = "Attention! N'allez pas trop loin dans l'arêne sans préparations.";

		} else 
		{
			loadingMessage.text = "Beware ! Don't go too deep in the arena too early...";
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
		yield return new WaitForSecondsRealtime (30f);
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

		StartCoroutine(FadeTo(0f,5f));
		loadingCanvas.SetActive (false);

		isCreating = false;
	}
	public void ToggleOneLaneTwoLanes()
	{

			networkManager.GetComponent<PlayerMenuSettings> ().isItOneLane = isItOneLane.isOn;

	}

	}

