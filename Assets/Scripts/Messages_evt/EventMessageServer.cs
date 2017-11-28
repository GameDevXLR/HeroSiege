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
		if (degat != 0) {
			TargetSendDegatMessage (bully.GetComponent<NetworkIdentity> ().connectionToClient, victime, degat);
		}
	}
    
    public void receiveDegat(GameObject victime, int degat)
	{
		if (degat != 0) {
			TargetSendDegatMessage (victime.GetComponent<NetworkIdentity> ().connectionToClient, victime, degat);
		}
	}



    [TargetRpc]
    public void TargetSendDegatMessage(NetworkConnection target, GameObject victime, int degats)
    {

        GameObject eventMess = ObjectPooling.Instance.GetPooledObject(degatPrefab.tag);
        
        //eventMess.transform.SetParent(victime.transform, false);
        //eventMess.transform.localScale = new Vector3(1, 1, 1);
		int tmpDegatScale =(int) Mathf.Round(Mathf.Abs(degats)/20);
		if (tmpDegatScale < 5) 
		{
			tmpDegatScale = 5;
		}
		if (tmpDegatScale > 20) 
		{
			tmpDegatScale = 20;
		}
		eventMess.transform.position = new Vector3( victime.transform.position.x,victime.transform.position.y+2f,victime.transform.position.z) ;
		eventMess.transform.GetChild (0).GetComponent<RectTransform>().localScale = new Vector3 ( tmpDegatScale, tmpDegatScale, 1);
		eventMess.transform.GetChild(0).GetComponentInChildren<Text>().text =  degats.ToString();
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

    public void ReceiveDifficulty(int diffLvl)
    {
        RpcSetDifficulty(diffLvl);
    }

    [ClientRpc] 
    public void RpcSetDifficulty(int diffLvl)
    {
        GameManager.instanceGM.difficultyPanel.GetComponent<ChooseDifficultyScript>().SyncDifficulty(diffLvl);
    }
}

