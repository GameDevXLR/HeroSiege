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
     

	void Awake(){
		sharedInstance = this;
	}


	public List<ObjectPoolItems> itemsToPool;
	public List<GameObject> pooledObject;
    public List<NetworkInstanceId> listID;
    
	void Start(){
		pooledObject = new List<GameObject> ();
		foreach (ObjectPoolItems item in itemsToPool) {
			for (int i = 0; i < item.amountToPool; i++) {
                GameObject obj = addObjectInListe (item.objectToPool);
                NetworkServer.Spawn(obj);
                listID.Add(obj.GetComponent<NetworkIdentity>().netId);

            }

		}

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


	private GameObject addObjectInListe (GameObject objectToPool){
		GameObject obj = (GameObject)Instantiate (objectToPool);
		obj.SetActive (false);
		pooledObject.Add (obj);
        return obj;
	}
}
