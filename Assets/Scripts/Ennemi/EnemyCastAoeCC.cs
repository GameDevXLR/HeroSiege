using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EnemyCastAoeCC : NetworkBehaviour 
{

	public GameObject AoeCCPrefab;
	public float spellCCDuration = 2f;

	public int charges;
	public int recquiredCharges = 5;

	public void AddACharge()
	{
		charges++;
		if (charges == recquiredCharges) 
		{
			CastAoeCC ();
			charges = 0;
		}
		
	}

	public void CastAoeCC()
	{
		GameObject go = Instantiate(AoeCCPrefab, transform.position, transform.rotation);
		go.GetComponent<EnemyAoeCCSpell>().caster = gameObject;
		go.GetComponent<EnemyAoeCCSpell>().duration = spellCCDuration;
		NetworkServer.Spawn(go);

	}

}
