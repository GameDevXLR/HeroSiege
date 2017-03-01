using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ennemiSpawn : MonoBehaviour {

	public GameObject ennemi;
	public Transform inibTransform;
	public float waves;

	private GameObject newEnnemi;

	void Start () 
	{
		StartCoroutine (spawn ());
	}
	

	IEnumerator spawn()
	{
		for (float i = 0; i < waves; i++) 
		{
			GameObject newEnnemi = Instantiate (ennemi, inibTransform.position, inibTransform.rotation) as GameObject;
			yield return new WaitForSeconds (5);
		}
	}
}
