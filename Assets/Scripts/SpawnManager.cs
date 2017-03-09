﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SpawnManager : NetworkBehaviour 
{

	//nouveau systeme de spawn plus propre.


	private float lastTic; //dernier proc en temps global.
	public float timeBetweenTic; //temps entre 2 mobs d'une meme vague.
	public int level = 0; // niveau de la vague. (détermine le type de mob)

	public GameObject[] ennemi; //array des mobs spawnable.
	public Transform spawnpoint; //point de spawn du mob.
	public int numberOfMobs; // nombre de mobs dans la vague.
	private int actualNbr; //C est le mob numero combien de la vague ? 
	public float TimeBetweenMobs; //temps entre 2 mobs.

	public int totalWaves; // le numéro de la vague d'ennemis.
	private GameObject newEnnemi;
	public int difficultyFactor; // permet de gérer la difficulté du jeu. cette variable est rechercher sur le GameManager au lancement du script.

	void Start () 
	{
		difficultyFactor = GameManager.instanceGM.gameDifficulty;
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
		newEnnemi.GetComponent<GenericLifeScript> ().xpGiven += tmpFactor;
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

}
