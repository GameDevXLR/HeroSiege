using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Networking;

public class ClicToSelectManager : MonoBehaviour 
{

	//gere le clic to select. pour l'instant ya que des shops faudra ajuster ca un jour.
	public LayerMask layer_mask;

	// Update is called once per frame
	void Update ()
	{
		if (Input.GetMouseButtonUp (0) ) 
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			if (Physics.Raycast (ray, out hit, 50f, layer_mask)) 
			{
                GameObject objCollide = hit.collider.gameObject;
                if (objCollide.GetComponent<ShopScript>())
                {
                    objCollide.GetComponent<ShopScript>().OpenYourMenu();
                }
			}
		}
	}
}
