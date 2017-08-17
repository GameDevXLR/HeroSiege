using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledObjects : MonoBehaviour {

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
            GameObject obj = Instantiate(item.objectToPool);
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
        
        if (index == pooledObjects.Count)
        {
            if (item.shouldExtand)
                pooledObjects.Add(Instantiate(item.objectToPool));
            else
            {
                index = 0;
            }
        }
        else if(!pooledObjects[index].activeInHierarchy)
        {
            pooledObjects[index].SetActive(true);
        }
        return pooledObjects[index++];
    }

    
}
