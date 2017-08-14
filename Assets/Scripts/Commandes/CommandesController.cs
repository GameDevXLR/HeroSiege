﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandesController : MonoBehaviour {
    public static CommandesController Instance;

    public List<CommandeClass> listKeys;
    public Dictionary<string, CommandeClass> dictCommandes;
    

    private void Awake()
    {
        //PlayerPrefs.DeleteAll(); // just for the test if I want to test without the name
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

    public KeyCode getKeycode(string commande)
    {
        return dictCommandes[commande].getKey();
    }
}
