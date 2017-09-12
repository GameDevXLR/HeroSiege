using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Networking;

public class ClicToSelectManager : MonoBehaviour 
{

	//gere le clic to select. pour l'instant ya que des shops faudra ajuster ca un jour.
	public LayerMask layer_mask;
    private GameObject playerObj;

	// Use this for initialization
	void Start () 
	{
        playerObj = GameManager.instanceGM.playerObj;

    }
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetMouseButtonUp (0) ) 
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
            Debug.Log("miaoiu");
			if (Physics.Raycast (ray, out hit, 50f, layer_mask)) 
			{
                Debug.Log("try");
                if (hit.collider.gameObject.GetComponent<ShopScript>())
                {
                    hit.collider.gameObject.GetComponent<ShopScript>().OpenYourMenu();
                }
				    
                else if (hit.collider.gameObject.GetComponent<CrystalManager>())
                {
                    playerObj.GetComponent<PlayerIGManager>().energy += hit.collider.gameObject.GetComponent<CrystalManager>().getEnergie(10);
                }
                
			}
		}
	}
}
