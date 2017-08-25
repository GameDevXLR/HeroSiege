using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


/// <summary>
/// ce script est pour nous aider pour tout ce qui est connection
/// </summary>
public class NetworkUtils : MonoBehaviour
{

    public static NetworkUtils Instance;
    public Dictionary<string, NetworkConnection> listPlayer;
    public List<NetworkConnection> listConn;
    public List<NetworkConnection> listConnTeam1;
    public List<NetworkConnection> listConnTeam2;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            listPlayer = new Dictionary<string, NetworkConnection>();
            listConn = new List<NetworkConnection>();
            listConnTeam1 = new List<NetworkConnection>();
            listConnTeam2 = new List<NetworkConnection>();
        }
        else
        {
            Destroy(this);
        }
    }

    public void addConn(NetworkConnection conn, int team = 0)
    {
        if (!listConn.Contains(conn))
            listConn.Add(conn);
        
        if (team == 1 && !listConnTeam1.Contains(conn)) { 
            listConnTeam1.Add(conn);
            if (listConnTeam2.Contains(conn))
            {
                listConnTeam2.Remove(conn);
            }
        }
        else if (team == 2 && !listConnTeam2.Contains(conn))
        {
            listConnTeam2.Add(conn);
            if (listConnTeam1.Contains(conn))
            {
                listConnTeam1.Remove(conn);
            }
        }
        Debug.Log("Connection : ");
        foreach (NetworkConnection co in listConn)
        {
            Debug.Log(co.ToString());
        }
        Debug.Log("Connection team 1 : ");
        foreach (NetworkConnection co in listConnTeam1)
        {
            Debug.Log(co.ToString());
        }
        Debug.Log("Connection team 2 : ");
        foreach (NetworkConnection co in listConnTeam2)
        {
            Debug.Log(co.ToString());
        }

    }

    
    




}
