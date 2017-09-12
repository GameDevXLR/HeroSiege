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
        eventMess.transform.SetParent(victime.transform, false);
        eventMess.transform.localScale = new Vector3(1, 1, 1);
        eventMess.GetComponentInChildren<Text>().text = "+" + degats;
        if(degats > 0)
        {
            eventMess.GetComponentInChildren<Text>().color = Color.green;
        }
        else
        {
            eventMess.GetComponentInChildren<Text>().color = Color.red;
        }
        eventMess.SetActive(true);
        eventMess.GetComponent<Animator>().enabled = true;
        eventMess.GetComponent<Canvas>().enabled = true;https://www.ecosia.org/search?tt=vivaldi&q=unet
        eventMess.GetComponent<InactivateAnimatorCanvas>().inactiveWithTime();
        eventMess.GetComponent<InactivateAndMoveByTime>().InactivateWithlifeTime();
    }
}

