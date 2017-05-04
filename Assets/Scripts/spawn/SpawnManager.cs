using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SpawnManager : NetworkBehaviour 
{

	//nouveau systeme de spawn plus propre.


	private float lastTic; //dernier proc en temps global.
	public float timeBetweenTic; //temps entre 2 mobs d'une meme vague.
	public int level = 0; // niveau de la vague. (détermine le type de mob)
	public bool isTeam1; // cocher si c'est un ennemi de la team1. sinon ce sera team 2 auto.
	public GameObject[] ennemi; //array des mobs spawnable.
	public Transform spawnpoint; //point de spawn du mob.
	public Transform targetDestination;
	public int numberOfMobs; // nombre de mobs dans la vague.
	private int actualNbr; //C est le mob numero combien de la vague ? 
	public float TimeBetweenMobs; //temps entre 2 mobs.

	[SyncVar (hook = "ActuNbrOfWaves")]public int totalWaves; // le numéro de la vague d'ennemis. nombre total de vagues depuis le début.
	private GameObject newEnnemi;
	public int difficultyFactor; // permet de gérer la difficulté du jeu. cette variable est rechercher sur le GameManager au lancement du script.

	void Start () 
	{
		difficultyFactor = GameManager.instanceGM.gameDifficulty;
//        Debug.Log("start : SpawManager");
	}
	

	void Update () 
	{
		if (!isServer) 
		{
			return;			
		}
		if (Time.time > lastTic) 
		{
			lastTic = Time.time + timeBetweenTic;
			actualNbr++;
			SpawnAMob ();
			if (level == 2) 
			{
				SpawnAMob ();
			}
			if (actualNbr == numberOfMobs) 
			{
				LevelUpTheWaves ();
				lastTic += 5f; //ajouter ici le temps entre 2 vagues de 2 niveaux différents.
			}
		}
	}
	public void SpawnAMob()
	{
		newEnnemi = Instantiate (ennemi [level], spawnpoint.position, spawnpoint.rotation) as GameObject;
		int tmpFactor = totalWaves * 10 * difficultyFactor;
		newEnnemi.GetComponent<GenericLifeScript> ().maxHp += tmpFactor;
		newEnnemi.GetComponent<GenericLifeScript> ().currentHp += tmpFactor;
		newEnnemi.GetComponent<GenericLifeScript> ().xpGiven += tmpFactor/difficultyFactor;
		newEnnemi.GetComponent<GenericLifeScript> ().goldGiven += tmpFactor/(5*difficultyFactor);
		newEnnemi.GetComponent<MinionsPathFindingScript>().isTeam1 = isTeam1;
		newEnnemi.GetComponent<MinionsPathFindingScript> ().target = targetDestination;
		if (level == 2) //si c'est la vague Boss (mob3) bennn faire péter hein...
		{
			newEnnemi.GetComponent<GenericLifeScript> ().maxHp += tmpFactor;
			newEnnemi.GetComponent<GenericLifeScript> ().currentHp += tmpFactor;
			newEnnemi.GetComponent<GenericLifeScript> ().xpGiven += tmpFactor/difficultyFactor;
			newEnnemi.GetComponent<GenericLifeScript> ().goldGiven += tmpFactor/(5*difficultyFactor);
		}
		NetworkServer.Spawn (newEnnemi);

	}

	//que se passe t-il quand on change de vague d'ennemi ? 
	//le nombre total de mob dans la vague augmente de 1 pour le moment quand on a fait défilé tous les types d'ennemis.
	public void LevelUpTheWaves()
	{
		totalWaves++;
		actualNbr = 0;
		level++;
		if (level == ennemi.Length) 
		{
			level = 0;
			numberOfMobs++;
		}
	}

	public void ActuNbrOfWaves (int nbW)
	{
		numberOfMobs = nbW;
		GameManager.instanceGM.nbrWavesText.text = nbW.ToString ();
	}
}
