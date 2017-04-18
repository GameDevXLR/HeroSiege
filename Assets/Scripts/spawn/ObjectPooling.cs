using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;


[System.Serializable]
public class ObjectPoolItems {
	public GameObject objectToPool;
	public int amountToPool;
	public bool shouldExtand = true;
}

public class ObjectPooling : NetworkBehaviour
{
	public static ObjectPooling sharedInstance;
	public List<ObjectPoolItems> itemsToPool;
	public List<GameObject> pooledObject;
    public List<NetworkInstanceId> listID;


    void Start()
    {
        if (isServer)
        {
            Debug.Log("Start ObjectPooling");
            CmdCreatePool();
        }
    }

    [Command]
    void CmdCreatePool(){
        
		pooledObject = new List<GameObject> ();
		foreach (ObjectPoolItems item in itemsToPool) {
			for (int i = 0; i < item.amountToPool; i++) {
                GameObject obj = addObjectInListe (item.objectToPool);
                NetworkServer.Spawn(obj);
                Debug.Log(obj.GetComponent<NetworkIdentity>().netId);
                NetworkInstanceId id = obj.GetComponent<NetworkIdentity>().netId;
                //listID.Add(id);
                RpcAddNetworkID(id);
            }
		}
        RpcSetActiveObject(false);

    }

    public GameObject GetPooledObject(string tag){
		for(int i = 0; i < pooledObject.Count; i++){
			if (!pooledObject [i].activeInHierarchy && pooledObject[i].tag == tag) {
				return pooledObject [i];
			}
		}

		foreach(ObjectPoolItems item in itemsToPool){
			if (item.objectToPool.tag == tag) {
				if(item.shouldExtand){
					addObjectInListe (item.objectToPool);
					return pooledObject [pooledObject.Count - 1];
				}
			}
		}
			
		return null;
	}

    [ClientRpc]
    void RpcSetActiveObject(bool activate)
    {

        for (int i = 0; i < listID.Count; i++)
        {
            GameObject obj = ClientScene.FindLocalObject(listID[i]);
            obj.SetActive(activate);
        }
    }

    [ClientRpc]
    void RpcAddNetworkID(NetworkInstanceId id)
    {
         listID.Add(id);
    }

    
    public void ReceiveSetActiveObjectWithId(NetworkInstanceId id, bool activate)
    {
        CmdSetActivateObjectWithID(id, activate);
    }

    [Command]
    public void CmdSetActivateObjectWithID(NetworkInstanceId id, bool activate)
    {
        RpcSetActiveObjectWithID(id, activate);

    }

    [ClientRpc]
    void RpcSetActiveObjectWithID(NetworkInstanceId id, bool activate)
    {
        GameObject obj = ClientScene.FindLocalObject(id);
        obj.SetActive(activate);
    }


    private GameObject addObjectInListe (GameObject objectToPool){
		GameObject obj = (GameObject)Instantiate (objectToPool);
        pooledObject.Add (obj);
        return obj;
	}
}
