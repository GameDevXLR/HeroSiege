using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CommandesGui : MonoBehaviour {

    public GameObject boutonPrefab;
    public GameObject boutonCurrent;
    public GameObject parent;
    public string playerPrefCurrent;
    public Dictionary<string, GameObject> dictCommande;
    public Dictionary<string, Text> dictDescr;
    Event keyEvent;
    KeyCode newKey;

    bool waitingForKey = false;



    // Use this for initialization
    void Start () {
        dictCommande = new Dictionary<string, GameObject>();
        dictDescr = new Dictionary<string, Text>();
        foreach (CommandeClass commande in CommandesController.Instance.listKeys)
        {
            GameObject buttonCommande = Instantiate(boutonPrefab);

            buttonCommande.transform.SetParent(parent.transform, false);
            if (PlayerPrefs.GetString("LANGAGE") == "Fr")
            {
                buttonCommande.transform.GetChild(0).GetComponent<Text>().text = commande.frenchName;
            }
            else
            {
                buttonCommande.transform.GetChild(0).GetComponent<Text>().text = commande.englishName;
            }

            GameObject bouton = buttonCommande.transform.GetChild(1).gameObject;

            dictCommande[commande.playerPrefKey] = bouton;
            dictDescr[commande.playerPrefKey] = buttonCommande.transform.GetChild(0).GetComponent<Text>();

            bouton.transform.GetChild(0).GetComponent<Text>().text = commande.getKey().ToString();
            bouton.GetComponent<Button>().onClick.AddListener(delegate { sendMessage(commande.playerPrefKey); });
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void sendMessage(string message)
    {
        boutonCurrent = dictCommande[message];
        playerPrefCurrent = message;
        if (!waitingForKey)
            StartCoroutine(AssignKey());
    }


    private void OnGUI()
    {
        /*
         * keyEvent dictates what key our user presses
        * bt using Event.current to detect the current
        * event
        */

        keyEvent = Event.current;
        //Executes if a button gets pressed and

        //the user presses a key
        
        if (keyEvent.isKey && waitingForKey)
        {
            newKey = keyEvent.keyCode; //Assigns newKey to the key user presses

            waitingForKey = false;
        }
    }



    public IEnumerator AssignKey()
    {
        waitingForKey = true;
        yield return WaitForKey(); //Executes ensdlessly until user presses a key

        CommandesController.Instance.dictCommandes[playerPrefCurrent].setKey(newKey) ; //Set forward to new keycode
        CommandesController.Instance.dictCommandes[playerPrefCurrent].saveKey();
        boutonCurrent.transform.GetChild(0).GetComponent<Text>().text = CommandesController.Instance.dictCommandes[playerPrefCurrent].getKey().ToString(); //Set button text to new key
        
        yield return null;
    }

    //Used for controlling the flow of our below Coroutine

    IEnumerator WaitForKey()
    {
        Debug.Log("WaitForKey");
        while (!keyEvent.isKey)
            yield return null;
    }

    public void switchLanguage(string lang)
    {
        foreach(KeyValuePair<string, Text> kvl in dictDescr)
        {
            if (lang == "Fr")
                kvl.Value.text = CommandesController.Instance.dictCommandes[kvl.Key].frenchName;
            else
                kvl.Value.text = CommandesController.Instance.dictCommandes[kvl.Key].englishName;
        }
    }

}
