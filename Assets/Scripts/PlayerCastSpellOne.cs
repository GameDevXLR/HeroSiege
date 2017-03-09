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
	public int spellLvl = 1;
	[SyncVar]private bool onCD;
	private Button spell1Btn;
	private Button spell1LvlUpBtn;
//	private GameObject spell1DescriptionObj;

	void Start () 
	{
		if(isLocalPlayer)
		{
			spell1Btn = GameObject.Find ("Spell1Btn").GetComponent<Button> ();
			spell1LvlUpBtn = GameObject.Find ("Spell1LvlUpBtn").GetComponent<Button> ();
			spell1Btn.onClick.AddListener (CastThatSpell );
			spell1LvlUpBtn.onClick.AddListener (levelUp );



		}
	}
	
//	public void ShowSpellDescription()
//	{
//		spell1DescriptionObj.GetComponent<Text> ().text = "";
//		spell1DescriptionObj.SetActive (true);
//	}
//	public void HideSpellDescription()
//	{
//		spell1DescriptionObj.GetComponent<Text> ().text = "";
//		spell1DescriptionObj.SetActive (false);
//	}
	
	[Command]
	public void CmdCastSpell()
	{
		if (GetComponent<GenericManaScript> ().currentMp > spellCost) 
		{

			GameObject go = Instantiate (spellObj, transform.position, transform.localRotation);
			go.GetComponent<SpellAreaDamage> ().caster = gameObject;
			go.GetComponent<AlwaysMove> ().target = gameObject;
			go.GetComponent<SpellAreaDamage> ().spellDamage = spellDmg;
			go.GetComponent<SpellAreaDamage> ().duration = spellDuration;
			NetworkServer.Spawn (go);
			GetComponent<GenericManaScript> ().LooseManaPoints (spellCost);
		}
	}
	public void CastThatSpell()
	{
		CmdCastSpell ();
		StartCoroutine(SpellOnCD());

	}
	void Update()
	{
		if (!isLocalPlayer) 
		{
			return;
		}

			if (Input.GetKeyUp (KeyCode.A) && !onCD) 
			{
			CastThatSpell ();
			}

	}

	IEnumerator SpellOnCD()
	{
		onCD = true;
		spell1Btn.GetComponent<Image> ().enabled = false;
		yield return new WaitForSeconds (spellCD);
		spell1Btn.GetComponent<Image> ().enabled = true;
		onCD = false;
	}

	[Command]
	public void CmdLevelUpTheSpell()
	{
		RpcLvlUpSpell ();
	}
	[ClientRpc]
	public void RpcLvlUpSpell()
	{
		spellLvl++;
		spellCost += 10;
		spellCD -= 0.5f;
		spellDmg += 20;
		spellDuration += 0.5f;
		if (isLocalPlayer) 
		{
			GetComponent<PlayerLevelUpManager> ().LooseASpecPt ();
			//changer ici l'interface du joueur.
		}
	}
	public void levelUp()
	{
		CmdLevelUpTheSpell ();
	}
}
