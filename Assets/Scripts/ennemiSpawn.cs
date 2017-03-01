using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ennemiSpawn : MonoBehaviour {

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
		for (float i = 0; i < waves; i++) 
		{
			if (i == 0) 
			{
				yield return new WaitForSeconds (2);
			}
			GameObject newEnnemi = Instantiate (ennemi[level], inibTransform.position, inibTransform.rotation) as GameObject;
			actualWave++;
			yield return new WaitForSeconds (timeBetweenWaves);
		}
	}
	void Update()
	{
		if (actualWave == waves) 
		{
			actualWave = 0;
			level++;
			if (level > ennemi.Length) 
			{
				Debug.Log ("fin du spawn");
			}
		}
	}
}
