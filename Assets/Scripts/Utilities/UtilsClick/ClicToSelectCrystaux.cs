using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Networking;

public class ClicToSelectABuild : MonoBehaviour 
{

	//gere le clic to select. pour l'instant ya que des shops faudra ajuster ca un jour.
	public LayerMask layer_mask;

	//// Use this for initialization
	//void Start () 
	//{
		
	//}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetMouseButtonUp (0) ) 
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			if (Physics.Raycast (ray, out hit, 50f, layer_mask)) 
			{	
				hit.collider.gameObject.GetComponent<ShopScript> ().OpenYourMenu ();
			}
		}
	}
}
