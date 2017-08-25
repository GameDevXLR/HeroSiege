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

    public void sendMessage(string name, string message)
    {
        CmdSendMessageWithName(name, message);
        //GameManager.instanceGM.messageManager.SendAnAlertMess(message, Color.red);        
    }

    [Command]
    public void CmdSendMessage(string message)
    {
        RpcSendMessage(message);
    }

    [Command]
    public void CmdSendMessageWithName(string name,string message)
    {
        foreach (KeyValuePair<string, NetworkConnection> entry in NetworkUtils.Instance.listPlayer)
        {
            Debug.Log("entry : " + entry.Key + "value : " + entry.Value);
        }
        TargetSendMessage(NetworkUtils.Instance.listPlayer[name],message);
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




}
