using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EnemySlowPlayer : NetworkBehaviour 
{
	public void SlowTheTarget(GameObject targ, float dur)
	{
		targ.GetComponent<PlayerStatusHandler> ().MakeHimSlow (dur);
	}
}
