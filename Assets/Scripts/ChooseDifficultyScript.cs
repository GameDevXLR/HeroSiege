using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.EventSystems;
using UnityEngine.Events;


public class ChooseDifficultyScript : MonoBehaviour,  IEventSystemHandler
{

	public UnityEvent GameMode;
//	public UnityEvent SelectedHero;
	//permet de choisir la difficulté de la partie.
	//seul le joueur Host pourra avoir acces a ce menu / il est désactiver au démarrage sur les autres joueurs.
	// en fonction de la difficultée choisi : les inibs sont activer.
	//une fois la difficulté choisi; le gamemanger lancera la partie.
	//le gamemanager garde trace de la difficulté choisie.
	public bool isSolo;
	public GameObject difficultyPanel;
	public GameObject inib1;
	public GameObject inib2;
	public GameObject inib3;
	public GameObject inib1B;
	public GameObject inib2B;
	public GameObject inib3B;
    public GameObject spawManager; // pour le pool 

	[Header("Difficulty select buttons")]
	public Button normalBtn;
	public Button hardBtn;
	public Button nightmareBtn;
	public Button madnessBtn;

	public Button startGameBtn;

	public Sprite normalImg;
	public Sprite hardImg;
	public Sprite nightmareImg;
	public Sprite madnessImg;

	public Sprite normalHighlightImg;
	public Sprite hardHighlightImg;
	public Sprite nightmareHighlightImg;
	public Sprite madnessHighlightImg;


    public enum difficultySettings
    {
        normal,
        hard,
        nightmare,
        madness
    }
    
    public int diffLvl;



    public difficultySettings gameMode;
    


    void Start()
    {
        if (GameMode == null)
        {
            GameMode = new UnityEvent();
        }

        GameMode.AddListener(NormalModeExe);
        StartCoroutine(StartProcedure());
        
    }



	public void SyncDifficulty (int diff)
	{
		normalBtn.animator.SetTrigger ("Normal");
		hardBtn.animator.SetTrigger ("Normal");
		nightmareBtn.animator.SetTrigger ("Normal");
		madnessBtn.animator.SetTrigger ("Normal");
		normalBtn.image.sprite = normalImg;
		hardBtn.image.sprite = hardImg;
		nightmareBtn.image.sprite = nightmareImg;
		madnessBtn.image.sprite = madnessImg;

		diffLvl = diff;
		switch (diff) 
		{
		case 1:
			normalBtn.animator.SetTrigger ("Highlighted");
			normalBtn.image.sprite = normalHighlightImg;
			break;
		case 2:
			hardBtn.animator.SetTrigger ("Highlighted");
			hardBtn.image.sprite = hardHighlightImg;
			break;
		case 3:
			nightmareBtn.animator.SetTrigger ("Highlighted");
			nightmareBtn.image.sprite = nightmareHighlightImg;
			break;
		case 4:
			madnessBtn.animator.SetTrigger ("Highlighted");
			madnessBtn.image.sprite = madnessHighlightImg;
			break;
			
		default:
			break;
		}
	}

	
	public void NormalMode()
	{
        GameManager.instanceGM.playerObj.GetComponent<EventMessageServer>().ReceiveDifficulty(1);
		GameMode.RemoveAllListeners ();
		GameMode.AddListener (NormalModeExe);
//		Invoke ("NormalModeExe", 0.2f);
	}
	public void NormalModeExe()
	{
		GameManager.instanceGM.gameDifficulty = 1;
		gameMode = difficultySettings.normal;
		if (PlayerPrefs.GetString ("LANGAGE") == "Fr") 
		{
			GameManager.instanceGM.messageManager.SendAnAlertMess ("Mode Normal activé.", Color.green);

		} else 
		{
			GameManager.instanceGM.messageManager.SendAnAlertMess ("Normal mode activated.", Color.green);
		}
		inib1.GetComponent<SpawnManager>().enabled = true;
		if (isSolo) 
		{
			return;
		}
		inib1B.GetComponent<SpawnManager>().enabled = true;


	}
	public void HardMode()
	{
        GameManager.instanceGM.playerObj.GetComponent<EventMessageServer>().ReceiveDifficulty(2);
        GameMode.RemoveAllListeners ();
		GameMode.AddListener (HardModeExe);
	}
	public void HardModeExe()
	{
		GameManager.instanceGM.gameDifficulty = 2;
		gameMode = difficultySettings.hard;
		inib1.GetComponent<SpawnManager>().enabled = true;
		if (isSolo) 
		{
			return;
		}
		inib1B.GetComponent<SpawnManager>().enabled = true;

	}

	public void NightmareMode()
	{
        GameManager.instanceGM.playerObj.GetComponent<EventMessageServer>().ReceiveDifficulty(3);
        GameMode.RemoveAllListeners ();
		GameMode.AddListener (NightmareModeExe);
	}
	public void NightmareModeExe()
	{
		GameManager.instanceGM.gameDifficulty = 3;
        gameMode = difficultySettings.nightmare;
		inib2.GetComponent<SpawnManager>().enabled = true;
		inib3.GetComponent<SpawnManager>().enabled = true;
		if (isSolo) 
		{
			return;
		}
		inib2B.GetComponent<SpawnManager>().enabled = true;
		inib3B.GetComponent<SpawnManager>().enabled = true;

	}

	public void MadnessMode()
	{
        GameManager.instanceGM.playerObj.GetComponent<EventMessageServer>().ReceiveDifficulty(4);
        GameMode.RemoveAllListeners ();
		GameMode.AddListener (MadnessModeExe);
	}
	public void MadnessModeExe()
	{
		GameManager.instanceGM.gameDifficulty = 4;
		if (PlayerPrefs.GetString ("LANGAGE") == "Fr") {
			GameManager.instanceGM.messageManager.SendAnAlertMess ("Mode Madness activé. Prenez garde!", Color.green);

		} else {
			GameManager.instanceGM.messageManager.SendAnAlertMess ("Madness?! Run! You fool...", Color.red);
		}
		gameMode = difficultySettings.madness;

		inib1.GetComponent<SpawnManager>().enabled = true;
		inib2.GetComponent<SpawnManager>().enabled = true;
		inib3.GetComponent<SpawnManager>().enabled = true;
		if (isSolo) 
		{
			return;
		}
		inib1B.GetComponent<SpawnManager>().enabled = true;
		inib2B.GetComponent<SpawnManager>().enabled = true;
		inib3B.GetComponent<SpawnManager>().enabled = true;


	}

	public void GenericStart()
	{
		if (GameManager.instanceGM.IsItSolo ()) 
		{
			isSolo = true;
		}
        GameManager.instanceGM.receiveStartTheGame();
		GameObject.Find ("MainSun").GetComponent<DayNightCycle> ().speed = -1.2f;
//		gameObject.GetComponent<RectTransform>().localScale = Vector3.zero;
		GameMode.Invoke ();
	}




	IEnumerator StartProcedure()
	{
		yield return new WaitForSeconds (0.05f);
        //setInib();
		spawManager = GameObject.Find ("SpawnManager");		
	}

    public void setInteractableBtn(bool interactable)
    {
        normalBtn.interactable = interactable;
        hardBtn.interactable = interactable;
        nightmareBtn.interactable = interactable;
        madnessBtn.interactable = interactable;
        startGameBtn.interactable = interactable;
        startGameBtn.gameObject.SetActive(interactable);
    }
    

    public void setInib()
    {
        GameManager.instanceGM.setInib(inib1.GetComponent<SpawnManager>(), inib2.GetComponent<SpawnManager>(), inib3.GetComponent<SpawnManager>(),
            inib1B.GetComponent<SpawnManager>(), inib2B.GetComponent<SpawnManager>(), inib3B.GetComponent<SpawnManager>());
    }

	public void StartGameButtonAvailable()
	{
		
	}
	
}
