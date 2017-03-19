using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowOnHover : MonoBehaviour 
{
	//ce script se place sur un panel UI : il fait que si on le survol; il affiche le premier
	//child de ce panel (un descriptif en gros).
	//le parent doit evidemment avoir un enfant a montrer...sinon ca marchera pas XD
	//il faut rajouter au panel un "event trigger" component de unity et linké les 2 fonctions ci dessous


	public Transform panelToShow;

	// Use this for initialization
	void Start () {
		panelToShow = transform.GetChild(0);	
	}
	
	public  void ShowHover(){
		panelToShow.gameObject.SetActive (true);
		
	}

	public  void HideOnEndHover()
	{
		panelToShow.gameObject.SetActive (false);

	}
}
