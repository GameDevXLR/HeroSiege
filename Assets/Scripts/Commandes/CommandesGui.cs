using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CommandesGui : MonoBehaviour {

    public GameObject boutonPrefab;
    public GameObject parent;
    public Dictionary<string, GameObject> dictCommande;



	// Use this for initialization
	void Start () {
        dictCommande = new Dictionary<string, GameObject>();
        foreach (CommandeClass commande in CommandesController.Instance.listKeys)
        {
            GameObject buttonCommande = Instantiate(boutonPrefab);

            buttonCommande.transform.SetParent(parent.transform, false);
            buttonCommande.transform.GetChild(0).GetComponent<Text>().text = commande.englishName;
            Debug.Log(commande.playerPrefKey);
            
            dictCommande[commande.playerPrefKey] = buttonCommande;

            GameObject bouton = buttonCommande.transform.GetChild(1).gameObject;
            bouton.transform.GetChild(0).GetComponent<Text>().text = commande.key.ToString();
            bouton.GetComponent<Button>().onClick.AddListener(delegate { sendMessage(commande.playerPrefKey); });
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void sendMessage(string message)
    {
        Debug.Log("message : " + message);
    }
}
