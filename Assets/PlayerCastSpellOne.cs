using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerCastSpellOne : NetworkBehaviour 
{
	public int spellCost = 30;
	public int spellDmg = 50;
	public float spellCD;
	public float spellDuration = 1.5f;
	public GameObject spellObj;
	[SyncVar]private bool onCD;
	// Use this for initialization
	void Start () 
	{
		if(isLocalPlayer)
		{
		GameObject.Find ("Spell1Btn").GetComponent<Button> ().onClick.AddListener (CmdCastSpell );
		}
	}
	
	[Command]
	public void CmdCastSpell()
	{
		if (GetComponent<GenericManaScript> ().currentMp > spellCost) 
		{
			StartCoroutine(SpellOnCD());

			GameObject go = Instantiate (spellObj, transform.position, transform.localRotation);
			go.GetComponent<SpellAreaDamage> ().caster = gameObject;
			go.GetComponent<AlwaysMove> ().target = gameObject;
			go.GetComponent<SpellAreaDamage> ().spellDamage = spellDmg;
			go.GetComponent<SpellAreaDamage> ().duration = spellDuration;
			NetworkServer.Spawn (go);
			GetComponent<GenericManaScript> ().LooseManaPoints (spellCost);
		}
	}

	void Update()
	{
		if (!isLocalPlayer) {
			return;
		}

			if (Input.GetKeyUp (KeyCode.A) && !onCD) 
			{
				CmdCastSpell ();
			}

	}

	IEnumerator SpellOnCD()
	{
		onCD = true;
		yield return new WaitForSeconds (spellCD);
		onCD = false;
	}
}
