using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InactivateByTime : MonoBehaviour
{

    public float lifetime;
    // Use this for initialization
    public void InactivateWithlifeTime()
    {
        Invoke("inactivate", lifetime);
    }

    public void inactivate()
    {
        gameObject.SetActive(false);
    }


}

