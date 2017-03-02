using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ennemiSpawn : MonoBehaviour {

	// ce script gere le spawn des ennemis : il contient une variable "level" qui détermine le niveau de la vague et donc le mob associé.
	// la variable "waves" correspond au nombre total de monstres qui spawn par level.
	// la coroutine s'arrete si le level est supérieur au nombre d'objets a faire spawn (nombre de gameobjects dans l'array ennemi)...

	public GameObject[] ennemi;
	public Transform inibTransform;
	public float waves;
	private int actualWave;
	public float timeBetweenWaves;
	public int level = 0;

	private GameObject newEnnemi;

	void Start () 
	{
		StartCoroutine (spawn ());
	}
	

	IEnumerator spawn()
	{
		if (level < ennemi.Length) {
			for (float i = 0; i < waves; i++) {
				if (i == 0) {
					yield return new WaitForSeconds (2);
				}
				GameObject newEnnemi = Instantiate (ennemi [level], inibTransform.position, inibTransform.rotation) as GameObject;
				actualWave++;

				yield return new WaitForSeconds (timeBetweenWaves);
			}
		}
	}
	void Update()
	{
		if (actualWave == waves) 
		{
			actualWave = 0;
			level++;
			if (level >= ennemi.Length) 
			{
				return;
			}
			StartCoroutine (spawn ());
		}
	}
}
