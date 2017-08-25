using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TeamManageServer : NetworkBehaviour
{
    public void sendTeam(string name, int team)
    {
        CmdTeamToServer(name, team);
        //GameManager.instanceGM.messageManager.SendAnAlertMess(message, Color.red);        
    }

    [Command]
    public void CmdTeamToServer(string name, int team)
    {
        NetworkConnection conn = GetComponent<NetworkIdentity>().connectionToClient;
        NetworkUtils.Instance.addConn(conn, team);
        
    }
}
