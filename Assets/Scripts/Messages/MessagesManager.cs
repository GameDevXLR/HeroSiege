﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MessagesManager : MonoBehaviour 
{

	//ce script permet de faire pop un message en local dans le "messagePanel".
	public GameObject messPrefab;
	public Transform messagesBox;
    public Scrollbar scroll;

    public bool inTchat = false;
    public InputField inputField;
    public Text placeholder;
    public string placeHolderFr;
    public string placeHolderEn;
    public Text ButtonText;
    public string ButtonStrFr;
    public string ButtonStrEn;


    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return))
        {

            if (inTchat)
            {
                sendMessage();
            }
            else
            {
                inputField.Select();
            }
        }
    }

    public void SendAnAlertMess(string message, Color color)
	{
        //GameObject go = Instantiate (messPrefab, messagesBox, false);
        GameObject go = ObjectPooling.Instance.GetPooledObject(messPrefab.tag);
        go.transform.SetParent(messagesBox, false);
        go.transform.localScale = new Vector3(1, 1, 1);
		go.GetComponentInChildren<Text> ().text = message;
		go.GetComponentInChildren<Text> ().color = color;



        Invoke("resetScroll", 0.1f);
		go.transform.SetAsLastSibling ();
	}


    public void resetScroll()
    {
        scroll.value = 0;
    }



    public void sendMessage()
    {
        
        string premessage = UtilsString.EraseRetourChariot(inputField.text);
        
        if (premessage[0].Equals('\\'))
        {
            checkMessageSpe(premessage);
        }
        else if ( premessage.Trim().Length != 0)
        {
            string message = GameManager.instanceGM.playerObj.GetComponent<PlayerManager>().playerNickname + " : " + premessage;
            GameManager.instanceGM.playerObj.GetComponent<MessagesManagerServer>().sendMessage(message);
            
        }
        inputField.text = "";
        EventSystem.current.SetSelectedGameObject(null);

    }

    
    public void setIntchat(bool isInTchat)
    {
        inTchat = isInTchat;
        GameManager.instanceGM.isInTchat = inTchat;
    }

    public void checkMessageSpe(string message)
    {
        string[] tab = message.Split(' ');

        if (tab[0].Equals("\\msg"))
        {
            Debug.Log("envoi d'un message");
            string sendMessage = GameManager.instanceGM.playerObj.GetComponent<PlayerManager>().playerNickname + " : " + message;
            GameManager.instanceGM.playerObj.GetComponent<MessagesManagerServer>().sendMessage(tab[1],sendMessage);
        }
        else
        {
            Debug.Log("raté ma poule try again");
        }

    }
}
