using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InactivateAnimatorCanvas : MonoBehaviour {


    public float time;

	public void inactiveWithTime()
    {
        Invoke("inactivate", time);
    }

    public void inactivate()
    {
        gameObject.GetComponent<Animator>().enabled = false;
        gameObject.GetComponent<Canvas>().enabled = false;
    }
}
