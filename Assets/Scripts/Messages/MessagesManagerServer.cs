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

    [Command]
    public void CmdSendMessage(string message)
    {
        RpcSendMessage(message);
    }

    [ClientRpc]
    public void RpcSendMessage(string message)
    {
        GameManager.instanceGM.messageManager.SendAnAlertMess(message, Color.blue);
    }


}
