using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SetLocalAutorities : NetworkBehaviour
{



    private NetworkIdentity objNetId;
    // Use this for initialization
    

    [Command]
    public void CmdSetAuthority(GameObject obj)
    {
        NetworkIdentity nIns = obj.GetComponent<NetworkIdentity>();
//        GameObject client = NetworkServer.FindLocalObject(nIns.netId);
//        NetworkIdentity ni = client.GetComponent<NetworkIdentity>();
        nIns.AssignClientAuthority(connectionToClient);
        Debug.Log("miaou");

    }
}
