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
     

	void Start(){
        if (isServer)
        {
            Debug.Log("Start ObjectPooling");
            CmdCreatePool();
        }
            
        
    }


	public List<ObjectPoolItems> itemsToPool;
	public List<GameObject> pooledObject;
    
    
    public List<NetworkInstanceId> listID;
    
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
        RpcSetActiveObject();
       



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
    void RpcSetActiveObject()
    {
        //this.listID = listID;
        Debug.Log("miaou");
        for (int i = 0; i < listID.Count; i++)
        {
            GameObject ship = ClientScene.FindLocalObject(listID[i]);

            ship.SetActive(false);

        }

    }

    [ClientRpc]
    void RpcAddNetworkID(NetworkInstanceId id)
    {
        
         listID.Add(id);
        
    }



    private GameObject addObjectInListe (GameObject objectToPool){
		GameObject obj = (GameObject)Instantiate (objectToPool);
        //obj.SetActive(false);
        pooledObject.Add (obj);
        return obj;
	}
}
