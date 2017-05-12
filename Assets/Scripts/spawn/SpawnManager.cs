using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SpawnManager : NetworkBehaviour 
{

	//nouveau systeme de spawn plus propre.
	int coPlayers;

	private float lastTic; //dernier proc en temps global.
	public float timeBetweenTic; //temps entre 2 mobs d'une meme vague.
	public int level = 0; // niveau de la vague. (détermine le type de mob)
	public bool isTeam1; // cocher si c'est un ennemi de la team1. sinon ce sera team 2 auto.
	public GameObject[] ennemi; //array des mobs spawnable.
	public GameObject squelettePrefab;
	public GameObject squeletteElitePrefab;
	public GameObject zombiePrefab;
	public GameObject zombieElitePrefab;
	public GameObject minibossPrefab;
	public GameObject minibossElitePrefab;

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
		coPlayers = NetworkServer.connections.Count;
		if (coPlayers > 1) 
		{
			difficultyFactor *= (coPlayers / 2);
		}
//        Debug.Log("start : SpawManager");
	}
	

//	void Update () 
//	{
//		if (!isServer) 
//		{
//			return;			
//		}
//
//		if (Time.time > lastTic) 
//		{
//			lastTic = Time.time + timeBetweenTic;
//			actualNbr++;
//			SpawnAMob ();
//			if (level == 2) 
//			{
//				SpawnAMob ();
//			}
//			if (actualNbr == numberOfMobs) 
//			{
//				LevelUpTheWaves ();
//				lastTic += 5f; //ajouter ici le temps entre 2 vagues de 2 niveaux différents.
//			}
//		}
//	}
//	public void SpawnAMob()
//	{
//		newEnnemi = Instantiate (ennemi [level], spawnpoint.position, spawnpoint.rotation) as GameObject;
//		int tmpFactor = totalWaves * 10 * difficultyFactor;
//		newEnnemi.GetComponent<GenericLifeScript> ().maxHp += tmpFactor;
//		newEnnemi.GetComponent<GenericLifeScript> ().currentHp += tmpFactor;
//		newEnnemi.GetComponent<GenericLifeScript> ().xpGiven += tmpFactor/difficultyFactor;
//		newEnnemi.GetComponent<GenericLifeScript> ().goldGiven += tmpFactor/(5*difficultyFactor);
//		newEnnemi.GetComponent<MinionsPathFindingScript>().isTeam1 = isTeam1;
//		newEnnemi.GetComponent<MinionsPathFindingScript> ().target = targetDestination;
//		if (level == 2) //si c'est la vague Boss (mob3) bennn faire péter hein...
//		{
//			newEnnemi.GetComponent<GenericLifeScript> ().maxHp += tmpFactor;
//			newEnnemi.GetComponent<GenericLifeScript> ().currentHp += tmpFactor;
//			newEnnemi.GetComponent<GenericLifeScript> ().xpGiven += tmpFactor/difficultyFactor;
//			newEnnemi.GetComponent<GenericLifeScript> ().goldGiven += tmpFactor/(5*difficultyFactor);
//		}
//		NetworkServer.Spawn (newEnnemi);
//
//	}

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

	public void StartSpawning(int day)
	{

		switch (day) 
		{
		case 1:
			StartCoroutine(SpawnProcedure(5,0,0,0,0,0));
			break;
		case 2:
			StartCoroutine(SpawnProcedure(0,3,0,0,0,0));

			break;
		case 3:
			StartCoroutine(SpawnProcedure(0,2,0,3,0,0));

			break;
		case 4:
			StartCoroutine(SpawnProcedure(0,0,0,3,2,0));

			break;
		case 5:
			StartCoroutine(SpawnProcedure(0,0,1,0,0,0));

			break;
		case 6:
			StartCoroutine(SpawnProcedure(0,0,1,4,0,0));

			break;
		case 7:
			StartCoroutine(SpawnProcedure(0,0,1,3,2,0));

			break;
		case 8:
			StartCoroutine(SpawnProcedure(0,0,0,4,2,1));

			break;
		case 9:
			StartCoroutine(SpawnProcedure(0,0,0,5,3,1));

			break;
		case 10:
			StartCoroutine(SpawnProcedure(0,0,0,0,5,2));

			break;
		default:
			StartCoroutine(SpawnProcedure(0,0,0,5,3,3));

			//pour tous les autres jours faire ca ici.
			break;
		}
	}

	IEnumerator SpawnProcedure(int squel, int zomb, int miniB, int squelEli, int zombEli, int miniBEli)
	{
		while (true) 
		{
			yield return new WaitForSeconds (11.5f); // changer ca si la durée d'une nuit varie.
			totalWaves++;
			if (squel != 0) {
				for (int i = 0; i < squel; i++) {
					newEnnemi = Instantiate (squelettePrefab, spawnpoint.position, spawnpoint.rotation) as GameObject;
					int tmpFactor = totalWaves * 10 * difficultyFactor;

					newEnnemi.GetComponent<GenericLifeScript> ().maxHp += tmpFactor;
					newEnnemi.GetComponent<GenericLifeScript> ().currentHp += tmpFactor;
					newEnnemi.GetComponent<GenericLifeScript> ().goldGiven += tmpFactor / (5 * difficultyFactor);
					newEnnemi.GetComponent<EnemyAutoAttackScript> ().damage += tmpFactor / 10;
					newEnnemi.GetComponent<MinionsPathFindingScript> ().isTeam1 = isTeam1;
					newEnnemi.GetComponent<MinionsPathFindingScript> ().target = targetDestination;
					NetworkServer.Spawn (newEnnemi);
					yield return new WaitForSeconds (0.2f);
				}
			}
			yield return new WaitForSeconds (0.2f);
			if (zomb != 0) {
				for (int i = 0; i < zomb; i++) {
					newEnnemi = Instantiate (zombiePrefab, spawnpoint.position, spawnpoint.rotation) as GameObject;
					int tmpFactor = totalWaves * 10 * difficultyFactor;
					newEnnemi.GetComponent<GenericLifeScript> ().maxHp += tmpFactor;
					newEnnemi.GetComponent<GenericLifeScript> ().currentHp += tmpFactor;
					newEnnemi.GetComponent<GenericLifeScript> ().goldGiven += tmpFactor / (5 * difficultyFactor);
					newEnnemi.GetComponent<EnemyAutoAttackScript> ().damage += tmpFactor / 10;

					newEnnemi.GetComponent<MinionsPathFindingScript> ().isTeam1 = isTeam1;
					newEnnemi.GetComponent<EnemyAutoAttackScript> ().damage += tmpFactor / 10;
					newEnnemi.GetComponent<MinionsPathFindingScript> ().target = targetDestination;
					NetworkServer.Spawn (newEnnemi);
					yield return new WaitForSeconds (0.2f);
				}
			}
			yield return new WaitForSeconds (0.2f);
			if (miniB != 0) {
				for (int i = 0; i < miniB; i++) {
					newEnnemi = Instantiate (minibossPrefab, spawnpoint.position, spawnpoint.rotation) as GameObject;
					int tmpFactor = totalWaves * 10 * difficultyFactor;
					newEnnemi.GetComponent<GenericLifeScript> ().maxHp += tmpFactor*2;
					newEnnemi.GetComponent<GenericLifeScript> ().currentHp += tmpFactor*2;
					newEnnemi.GetComponent<GenericLifeScript> ().goldGiven += tmpFactor / (2 * difficultyFactor);
					newEnnemi.GetComponent<EnemyAutoAttackScript> ().damage += tmpFactor / 2;
					newEnnemi.GetComponent<MinionsPathFindingScript> ().isTeam1 = isTeam1;
					newEnnemi.GetComponent<MinionsPathFindingScript> ().target = targetDestination;
					NetworkServer.Spawn (newEnnemi);
					yield return new WaitForSeconds (0.2f);
				}
			}
			yield return new WaitForSeconds (0.2f);
			if (squelEli != 0) {
				for (int i = 0; i < squelEli; i++) {
					newEnnemi = Instantiate (squeletteElitePrefab, spawnpoint.position, spawnpoint.rotation) as GameObject;
					int tmpFactor = totalWaves * 10 * difficultyFactor;
					newEnnemi.GetComponent<GenericLifeScript> ().maxHp += tmpFactor*4;
					newEnnemi.GetComponent<GenericLifeScript> ().currentHp += tmpFactor*4;
					newEnnemi.GetComponent<GenericLifeScript> ().goldGiven += tmpFactor / (2 * difficultyFactor);
					newEnnemi.GetComponent<EnemyAutoAttackScript> ().damage += tmpFactor / 5;
					newEnnemi.GetComponent<GenericLifeScript> ().xpGiven += 275;
					newEnnemi.transform.Find ("Mob1Prefab").transform.localScale = new Vector3 (2, 2, 2);
					newEnnemi.GetComponent<GenericLifeScript> ().armorScore += 20;

					newEnnemi.GetComponent<MinionsPathFindingScript> ().isTeam1 = isTeam1;
					newEnnemi.GetComponent<MinionsPathFindingScript> ().target = targetDestination;
					NetworkServer.Spawn (newEnnemi);
					yield return new WaitForSeconds (0.2f);
				}
			}
			yield return new WaitForSeconds (0.2f);
			if (zombEli != 0) {
				for (int i = 0; i < zombEli; i++) {
					newEnnemi = Instantiate (zombieElitePrefab, spawnpoint.position, spawnpoint.rotation) as GameObject;
					int tmpFactor = totalWaves * 10 * difficultyFactor;
					newEnnemi.GetComponent<GenericLifeScript> ().maxHp += tmpFactor*4;
					newEnnemi.GetComponent<GenericLifeScript> ().currentHp += tmpFactor*4;
					newEnnemi.GetComponent<GenericLifeScript> ().goldGiven += tmpFactor / (2 * difficultyFactor);
					newEnnemi.GetComponent<EnemyAutoAttackScript> ().damage += tmpFactor / 5;
					newEnnemi.GetComponent<GenericLifeScript> ().xpGiven += 250;
					newEnnemi.transform.Find ("Mob2Prefab").transform.localScale = new Vector3 (2, 2, 2);
					newEnnemi.GetComponent<GenericLifeScript> ().dodge += 7;

					newEnnemi.GetComponent<MinionsPathFindingScript> ().isTeam1 = isTeam1;
					newEnnemi.GetComponent<MinionsPathFindingScript> ().target = targetDestination;
					NetworkServer.Spawn (newEnnemi);
					yield return new WaitForSeconds (0.2f);
				}
			}
			yield return new WaitForSeconds (0.2f);
			if (miniBEli != 0) 
			{
				for (int i = 0; i < miniBEli; i++) {
					newEnnemi = Instantiate (minibossElitePrefab, spawnpoint.position, spawnpoint.rotation) as GameObject;
					int tmpFactor = totalWaves * 10 * difficultyFactor;
					newEnnemi.GetComponent<GenericLifeScript> ().maxHp += tmpFactor*5;
					newEnnemi.GetComponent<GenericLifeScript> ().currentHp += tmpFactor*5;
					newEnnemi.GetComponent<GenericLifeScript> ().xpGiven += 750;
					newEnnemi.GetComponent<GenericLifeScript> ().goldGiven += tmpFactor / (1 * difficultyFactor);
					newEnnemi.GetComponent<EnemyAutoAttackScript> ().damage += tmpFactor;
					newEnnemi.transform.Find ("Mob3Prefab").transform.localScale = new Vector3 (2, 2, 2);
					newEnnemi.GetComponent<GenericLifeScript> ().armorScore += 20;

					newEnnemi.GetComponent<MinionsPathFindingScript> ().isTeam1 = isTeam1;
					newEnnemi.GetComponent<MinionsPathFindingScript> ().target = targetDestination;
					NetworkServer.Spawn (newEnnemi);
					yield return new WaitForSeconds (0.2f);
				}
			}
		}
	}

}
