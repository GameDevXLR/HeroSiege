using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AutoCastCatapulteOnPlayers : NetworkBehaviour 
{
	public bool teamOfCataIs1;
	public int spellDmg;
	public GameObject spellPrefab;

	float previousFire;
	public float timeBetweenFire = 10f;

	// Use this for initialization
	void Start () 
	{
		if (GetComponent<CaptureThePoint> ().canBeOwnedBy == CaptureThePoint.PointOwner.team2) 
		{
			teamOfCataIs1 = false;
		}
		
	}
	
	void Update () 
	{
		if (!isServer) 
		{
			return;
		}
		if (Time.time > 30 && Time.time > previousFire + timeBetweenFire) 
		{
			previousFire = Time.time + Random.Range(0f, 20f);
			if (GetComponent<CaptureThePoint> ().belongsTo == CaptureThePoint.PointOwner.neutral) 
			{
				FireOnAPlayer ();
			}
		}
		
	}

	public void FireOnAPlayer()
	{
		if (teamOfCataIs1) 
		{
			int x = Random.Range (0, GameManager.instanceGM.team1ID.Count);
			Vector3 targPos =  ClientScene.FindLocalObject (GameManager.instanceGM.team1ID [x]).transform.position;
			FireOnTarget (targPos);
		} else 
		{
			if (GameManager.instanceGM.soloGame) 
			{
				return;
			}
			int x = Random.Range (0, GameManager.instanceGM.team2ID.Count);
			Vector3 targPos =  ClientScene.FindLocalObject (GameManager.instanceGM.team2ID [x]).transform.position;
			FireOnTarget (targPos);

		}
		
	}
	public void FireOnTarget(Vector3 pos)
	{
		GameObject go = Instantiate(spellPrefab, pos, Quaternion.identity);
//		go.GetComponent<SpellCatapulteAuto>().caster = gameObject;
		go.GetComponent<SpellCatapulteAuto>().spellDamage = spellDmg;
		NetworkServer.Spawn(go);
	}
}
