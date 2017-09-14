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
    public Scrollbar scroll;

    public bool inTchat = false;
    public InputField inputField;
    public Text placeholder;
    public string placeHolderFr;
    public string placeHolderEn;
    public Text ButtonText;
    public string ButtonStrFr;
    public string ButtonStrEn;
    private int count = 0;
    public bool canTalk = true;
    public int maxMessByTime = 10;
    public float time = 30f;


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
        go.GetComponentInChildren<Text>().text = message;
        go.GetComponentInChildren<Text>().color = color;
        Invoke("resetScroll", 0.1f);
        go.transform.SetAsLastSibling();
    }


    public void resetScroll()
    {
        scroll.value = 0;
    }



    public void sendMessage()
    {
        if (canTalk)
        {
            if (count == 0)
            {
                StartCoroutine(setCanTalkToTrue(time));
            }
            count++;
            if (count > maxMessByTime)
            {
                canTalk = false;

                string sendmessage = "";
                if (PlayerPrefs.GetString("LANGAGE") == "Fr")
                {
                    sendmessage = "Tu ne peux envoyer que " + maxMessByTime + " / " + time;
                }
                else
                {
                    sendmessage = "You can only send " + maxMessByTime + " / " + time;
                }
                SendAnAlertMess(sendmessage, Color.red);
            }
            else
            {
                string premessage = UtilsString.EraseRetourChariot(inputField.text);

                if (premessage.Trim().Length != 0)
                {
                    if (premessage[0].Equals('\\'))
                    {
                        checkMessageSpe(premessage);
                    }
                    else
                    {
                        string message = GameManager.instanceGM.playerObj.GetComponent<PlayerManager>().playerNickname + " : " + premessage;
                        GameManager.instanceGM.playerObj.GetComponent<MessagesManagerServer>().sendMessage(message);

                    }
                }


            }
            inputField.text = "";
            EventSystem.current.SetSelectedGameObject(null);

        }
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
            if (tab.Length > 2)
            {
                string premessage = string.Join(" ", UtilsArray.getSubArray(tab, 2, tab.Length - 2));
                if (premessage.Trim().Length != 0)
                {
                    string senderName = GameManager.instanceGM.playerObj.GetComponent<PlayerManager>().playerNickname;
                    GameManager.instanceGM.playerObj.GetComponent<MessagesManagerServer>().sendMessage(tab[1], senderName, premessage);
                }

            }
        }
        else if (tab[0].Equals("\\cheat1"))
        {
            GameManager.instanceGM.playerObj.GetComponent<PlayerIGManager>().maxHp = 200000;
            GameManager.instanceGM.playerObj.GetComponent<PlayerIGManager>().currentHp = 200000;
            GameManager.instanceGM.playerObj.GetComponent<PlayerAutoAttack>().damage = 2000;

        }
        else
        {
            string sendmessage;
            if (PlayerPrefs.GetString("LANGAGE") == "Fr")
            {
                sendmessage = "Désolé je ne connais pas ce code : " + tab[0];
            }
            else
            {
                sendmessage = "Sorry, this code is not known: " + tab[0];
            }
            SendAnAlertMess(sendmessage, Color.red);
        }

    }

    IEnumerator setCanTalkToTrue(float delay)
    {
        yield return new WaitForSeconds(delay);
        canTalk = true;
        count = 0;

    }
}
