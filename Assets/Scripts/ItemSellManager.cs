using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ItemSellManager : MonoBehaviour 
{

	//doit etre placer sur chaque objet d'inventaire revendable : 
	// toutes les fonctions placé dans UnityEvent seront exécuter.
	//il marche donc en duo avec un script disant quoi faire en cas de vente (le script de l'objet généralement a ca; il hérite)
	public UnityEvent toDoWhenSelling;
	public AudioClip Gold;
	// Use this for initialization
	public void SellBackItem()
	{
		toDoWhenSelling.Invoke ();
		GetComponent<AudioSource> ().PlayOneShot (Gold);
	}
}
