using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandesList : MonoBehaviour {

    public static CommandesList Instance;

    public List<CommandeClass> listKeys;
    public Dictionary<string, CommandeClass> dictCommandes;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        foreach(CommandeClass commande in listKeys)
        {
            dictCommandes[commande.playerPrefKey] = commande;
        }
    }



}
