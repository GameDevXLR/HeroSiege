using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerCastSpellOne : NetworkBehaviour 
{
	public GameObject spellObj;
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
		GameObject go = Instantiate (spellObj, transform.position, transform.localRotation);
		go.GetComponent<SpellAreaDamage> ().caster = gameObject;
		go.GetComponent<AlwaysMove> ().target = gameObject;
		NetworkServer.Spawn (go);
	}

	void Update()
	{
		if (!isLocalPlayer) 
		{
			return;
		}
		if(Input.GetKeyUp(KeyCode.A))
			{
				CmdCastSpell();
			}
	}
}
