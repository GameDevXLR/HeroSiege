using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MessagesManager : MonoBehaviour 
{

	//ce script permet de faire pop un message en local dans le "messagePanel".
	public GameObject messPrefab;
	public Transform messagesBox;

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
		GameObject go = Instantiate (messPrefab, messagesBox, false);
		go.GetComponentInChildren<Text> ().text = message;
		go.GetComponentInChildren<Text> ().color = color;
		//go.transform.SetAsLastSibling ();
	}


    public void sendMessage()
    {
        GameManager.instanceGM.playerObj.GetComponent<MessagesManagerServer>().sendMessage(inputField.text);
        inputField.text = "";
        EventSystem.current.SetSelectedGameObject(null);
    }

    



    public void setIntchat(bool isInTchat)
    {
        inTchat = isInTchat;
        GameManager.instanceGM.isInTchat = inTchat;
    }
}
