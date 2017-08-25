using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TeamManageServer : NetworkBehaviour
{
    public void sendTeam(int team)
    {
        CmdTeamToServer(team );
        //GameManager.instanceGM.messageManager.SendAnAlertMess(message, Color.red);        
    }

    [Command]
    public void CmdTeamToServer(int team)
    {
        NetworkUtils.Instance.addConn(GetComponent<NetworkIdentity>().connectionToClient, team);
    }
}
