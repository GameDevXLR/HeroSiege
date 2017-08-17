using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ObjectPoolItems
{
    public GameObject objectToPool;
    public int amountToPool;
    public bool shouldExtand = true;
}