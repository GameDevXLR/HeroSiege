using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EnnemyGoldScript : NetworkBehaviour {

    int gold;
    public GameObject goldPrefab;

    [TargetRpc]
    public void TargetGold(NetworkConnection target, int gold)
    {
        string message = GameManager.instanceGM.playerObj.GetComponent<PlayerManager>().playerNickname + " : " + gold + " gold" ;
        GameManager.instanceGM.playerObj.GetComponent<MessagesManagerServer>().sendMessage(message);
    }
}
