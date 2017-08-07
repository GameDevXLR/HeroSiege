using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CommandeClass {

    public string playerPrefKey; // key of the playerPref

    public string frenchName; 
    public string englishName;

    public KeyCode key; // key of the keyboard
    private KeyCode keyBase; // key of the keyboard (if reset, not really importante for now but cost nothing)

    
    
    // set key of the keyboard
    // @param keycode
    public void setKey(KeyCode key)
    {
        this.key = key;
    }

    // set key of the keyboard
    // @param string
    public void setKey(string key)
    {
        setKey((KeyCode)System.Enum.Parse(typeof(KeyCode), key));
    }

    // set key of the playerPref
    // @param keycode
    public void setplayerPrefKey(KeyCode key)
    {
        setplayerPrefKey(key.ToString());
    }

    // set key of the playerPref
    // @param string
    public void setplayerPrefKey(string key)
    {
        if (playerPrefKey == null)
        {
            if(frenchName != null && !PlayerPrefs.HasKey(frenchName))
            {
                playerPrefKey = frenchName;
            }
            else if (englishName != null && !PlayerPrefs.HasKey(englishName))
            {
                playerPrefKey = englishName;
            }
            else
            {
                do
                {
                    playerPrefKey = "Commande" + Random.Range(0,20000).ToString();
                }
                while (PlayerPrefs.HasKey(playerPrefKey));
            }
            
        }
    }

    // save the key in the playerPref
    public void saveKey()
    {
        PlayerPrefs.SetString(playerPrefKey, key.ToString());
    }
}
