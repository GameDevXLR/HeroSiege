using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MessagesManagerServer : NetworkBehaviour
{

    public void sendMessage(string message)
    {
        CmdSendMessage(message);
        //GameManager.instanceGM.messageManager.SendAnAlertMess(message, Color.red);        
    }

    public void sendMessage(string name, string sendername,string message)
    {
        CmdSendMessageWithName(name, sendername,message);
        //GameManager.instanceGM.messageManager.SendAnAlertMess(message, Color.red);        
    }

    [Command]
    public void CmdSendMessage(string message)
    {
        RpcSendMessage(message);
    }

    [Command]
    public void CmdSendMessageWithName(string name, string sendername, string message)
    {
        if (NetworkUtils.Instance.listPlayer.ContainsKey(name))
        {
            TargetSendMessage(NetworkUtils.Instance.listPlayer[name], sendername + " : " + message);
        }
        else
        {
            TargetSendMessageBadReveiver(NetworkUtils.Instance.listPlayer[sendername], name );
        }

    }

    [ClientRpc]
    public void RpcSendMessage(string message)
    {
        GameManager.instanceGM.messageManager.SendAnAlertMess(message, Color.blue);
    }

    [TargetRpc]
    public void TargetSendMessage(NetworkConnection target, string message)
    {
        GameManager.instanceGM.messageManager.SendAnAlertMess(message, Color.yellow);
    }

    [TargetRpc]
    public void TargetSendMessageBadReveiver(NetworkConnection target, string name)
    {
        string sendMessage = "";
        if (PlayerPrefs.GetString("LANGAGE") == "Fr")
        {
            sendMessage = "Désolé ce pseudonyme est inconnu : " + name;
        }
        else
        {
            sendMessage = "Sorry, this pseudo is unknown: " + name;
        }
        GameManager.instanceGM.messageManager.SendAnAlertMess(sendMessage, Color.red);
    }




}
