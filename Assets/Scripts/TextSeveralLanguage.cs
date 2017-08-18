using UnityEngine;
using System.Collections;
using UnityEngine.UI; // Required when Using UI elements.
using System.Collections.Generic;

public class TextSeveralLanguage : MonoBehaviour {

    public List<Text> FrenchText;
    public List<Text> EnglishText;
    
  
    private void Start()
    {
		if (PlayerPrefs.GetString("LANGAGE", "Eng") == Languages.French )
        {
            Invoke("TradFr", 0.1f);
        }

        else
        {
            Invoke("TradEng", 0.1f);
        }
    }

    public void TradFr()
    {
        foreach (Text text in FrenchText)
        {
            text.enabled = true;
        }

        foreach (Text text in EnglishText)
        {
            text.enabled = false;
        }
    }
    public void TradEng()
    {
        foreach (Text text in FrenchText)
        {
            text.enabled = false;
        }

        foreach (Text text in EnglishText)
        {
            text.enabled = true;
        }
    }
    public void SwitchLangage(string Language)
    {

        if (Language == Languages.French) //fr
        {
            PlayerPrefs.SetString("LANGAGE", Languages.French);
            TradFr();
        }
        else
        {
            PlayerPrefs.SetString("LANGAGE", Languages.English);
            TradEng();
        }
    }

}
