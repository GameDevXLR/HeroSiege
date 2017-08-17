using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;


public class ObjectPooling : NetworkBehaviour
{
	public static ObjectPooling Instance;
	public List<ObjectPoolItems> itemsToPool;
	public List<PooledObjects> pooledObject;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            CreatePool();
        }
        else
        {
            Destroy(this);
        }
    }
    
    void CreatePool(){
        
		pooledObject = new List<PooledObjects> ();
        int i = 0;
		foreach (ObjectPoolItems item in itemsToPool) {
            pooledObject.Add(new PooledObjects(item));
		}
    }

    public GameObject GetPooledObject(string tag){
        int i = 0;
        while (i < pooledObject.Count && pooledObject[i].getTagObj() != tag)
        {
            i++;
        }

        if (i == pooledObject.Count)
            return null;

        return pooledObject[i].getObj();
	}

}
