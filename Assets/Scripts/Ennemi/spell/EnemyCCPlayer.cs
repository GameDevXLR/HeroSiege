using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCCPlayer : MonoBehaviour 
{
	public float cCDuration = 1f;
	public void CCTheTarget(GameObject targ)
	{
		targ.GetComponent<PlayerStatusHandler> ().MakeHimCC (cCDuration);
	}
}
