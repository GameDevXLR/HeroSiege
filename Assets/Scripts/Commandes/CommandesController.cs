using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandesController : MonoBehaviour {
    public static CommandesController Instance;

    public List<CommandeClass> listKeys;
    public Dictionary<string, CommandeClass> dictCommandes;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

        }
        dictCommandes = new Dictionary<string, CommandeClass>();
        foreach (CommandeClass commande in listKeys)
        {
            commande.actuKey();
            dictCommandes[commande.playerPrefKey] = commande;
        }
        
    }
}
