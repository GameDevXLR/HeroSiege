using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledObjects {

    [SerializeField]
    private ObjectPoolItems item;
    [SerializeField]
    private List<GameObject> pooledObjects;
    private int index = 0;

    public PooledObjects(ObjectPoolItems objPooling)
    {
        pooledObjects = new List<GameObject>();
        item = objPooling;
        for (int i = 0; i < item.amountToPool; i++)
        {
            GameObject obj = GameObject.Instantiate(item.objectToPool) as GameObject;
            obj.SetActive(false);
            pooledObjects.Add(obj);
        }
    }

    public string getTagObj()
    {
        return item.objectToPool.tag;
    }

    public GameObject getObj()
    {

        if (item.shouldExtand)
        {
            index = UtilsArray.getFirstInactiveObject(pooledObjects);
            if(index == pooledObjects.Count)
            {
                GameObject obj = GameObject.Instantiate(item.objectToPool) as GameObject;
                pooledObjects.Add(obj);
            }
        }
        else if (index == pooledObjects.Count)
        {
            index = 0;
        }

        if(!pooledObjects[index].activeInHierarchy)
        {
            pooledObjects[index].SetActive(true);
        }
        return pooledObjects[index++];
    }    
}
