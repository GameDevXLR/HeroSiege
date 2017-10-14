using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class EventMessageServer : NetworkBehaviour
{
    public GameObject degatPrefab;

    public void receiveDegat(GameObject victime, GameObject bully,int degat)
    {
        TargetSendDegatMessage(bully.GetComponent<NetworkIdentity>().connectionToClient, victime, degat);
    }
    
    public void receiveDegat(GameObject victime, int degat)
    {
        TargetSendDegatMessage(victime.GetComponent<NetworkIdentity>().connectionToClient, victime,degat);
    }



    [TargetRpc]
    public void TargetSendDegatMessage(NetworkConnection target, GameObject victime, int degats)
    {
        GameObject eventMess = ObjectPooling.Instance.GetPooledObject(degatPrefab.tag);
        
        //eventMess.transform.SetParent(victime.transform, false);
        //eventMess.transform.localScale = new Vector3(1, 1, 1);
        eventMess.transform.position = victime.transform.position;
        eventMess.transform.GetChild(0).GetComponentInChildren<Text>().text = "+" + degats;
        if(degats > 0)
        {
            eventMess.transform.GetChild(0).GetComponentInChildren<Text>().color = Color.green;
        }
        else
        {
            eventMess.transform.GetChild(0).GetComponentInChildren<Text>().color = Color.red;
        }
        eventMess.SetActive(true);
        
        eventMess.GetComponentInChildren<Animator>().enabled = true;
        eventMess.GetComponentInChildren<Canvas>().enabled = true;
        eventMess.GetComponentInChildren<InactivateAnimatorCanvas>().inactiveWithTime();
        eventMess.GetComponentInChildren<InactivateByTime>().InactivateWithlifeTime();
        eventMess.GetComponent<InactivateByTime>().InactivateWithlifeTime();
    }
}

