using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class InactivateByTime : MonoBehaviour
{

    public float lifetime;
	public GameObject daddyTr;
    // Use this for initialization
    public void InactivateWithlifeTime()
    {
        Invoke("inactivate", lifetime);
    }

    public void inactivate()
    {
		daddyTr.GetComponent<NavMeshAgent> ().enabled = false;
		daddyTr.transform.position = Vector3.zero;
		GetComponent<Canvas> ().enabled = false;
		GetComponent<Animator> ().enabled = false;
    }


}

