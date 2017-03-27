﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[NetworkSettings (channel = 2, sendInterval = 0.1f)]
public class PlayerCastCCSpell : NetworkBehaviour 
	{
		//deuxieme sort: a mettre sur l'objet joueur.
		// sort de zone avec possibilité de target la ou on veut CC.
		//le sort fait spawn un prefab qui est configuré ici (dégats etc/ durée du CC)
		//le prefab doit etre enregistrer par le networkmanagerObj
		//le sort peut up.

		string spellDescription;
		public int spellCost = 80;
		public int spellDmg = 50;
		public float spellCD;
		public float spellDuration = 1.5f;
	public float spellRange = 25f;
		public GameObject spellObj;
		public int spellLvl = 1;
		private bool onCD;
		private Button spell2Btn;
		private Button spell2LvlUpBtn;
	private Vector3 castPosDesired;
	public GameObject spellTargeter;
	public bool isTargeting; // savoir si le joueur cible pour lancer le sort. 
	public LayerMask layer_mask;

		//	private GameObject spell1DescriptionObj;

		void Start () 
		{
			if(isLocalPlayer)
			{
				spell2Btn = GameObject.Find ("Spell2Btn").GetComponent<Button> ();
				spell2LvlUpBtn = GameObject.Find ("Spell2LvlUpBtn").GetComponent<Button> ();
				spell2Btn.onClick.AddListener (CastThatSpell );
				spell2LvlUpBtn.onClick.AddListener (levelUp );
				int x =(int) spellDmg / 5;
			spellDescription = "Stun et Inflige "+ x.ToString() + " dégats toutes les 0,5secondes pendant " + spellDuration.ToString() + " secondes. Cout: " + spellCost.ToString()+ " MP.CD: " + spellCD.ToString ();
				spell2Btn.transform.GetChild (0).GetComponentInChildren<Text> ().text = spellDescription;
			}
			spellTargeter = GameObject.Find ("AreaTargeter");

		}
		//lance le sort sur le serveur.
		//le spawn du préfab est un networkspawn : du coup il apparaitra sur tous les pc..il fera ses trucs sur le serveur
		//bien sur.
		[Command]
	public void CmdCastSpell(Vector3 pos)
		{
		GameObject go = Instantiate (spellObj, pos, spellTargeter.transform.rotation);
		go.GetComponent<SpellCCAreaScript> ().caster = gameObject;
		go.GetComponent<SpellCCAreaScript> ().spellDamage = spellDmg;
		go.GetComponent<SpellCCAreaScript> ().duration = spellDuration;
			NetworkServer.Spawn (go);

		}
		//cette fonction est la car on veut vérifier en local déja si on peut lancer le sort avant de
		//demander le lancement du sort sur le serveur...normal.
		public void CastThatSpell()
		{
		StartCoroutine (ShowTargeter ()); // on a besoin d'attendre la fin de frame pour pas que mouseUp soit détecter direct et que le sort se lance cash en cliquant sur l'icone de sort.
		}

		//si t'es un joueur; tu peux cast ce sort avec la touche Z : voir pour opti ca en fonction du clavier des gars.

		public void Update()
	{
		if (!isLocalPlayer) {
			return;
		}

		if (Input.GetKeyUp (KeyCode.Z) && !onCD) 
		{
			CastThatSpell ();
		}
		if (!isTargeting) 
		{
			return;
		}
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		if (Physics.Raycast (ray, out hit, 80f, layer_mask)) 
		{	
			if(Input.GetMouseButtonUp(1))
				{
				isTargeting = false;
				spellTargeter.transform.position = Vector3.zero;
				return;
				}
			if (Input.GetMouseButtonUp (0)) {
				if (Vector3.Distance (hit.point, transform.position) > spellRange || GetComponent<GenericManaScript> ().currentMp < spellCost) {
					Debug.Log (Vector3.Distance (hit.point, transform.position));
					isTargeting = false;
					spellTargeter.transform.position = Vector3.zero;
					return;
				}
				castPosDesired = hit.point;
				spellTargeter.transform.position = Vector3.zero;
				CmdCastSpell (castPosDesired);
				GetComponent<GenericManaScript> ().CmdLooseManaPoints (spellCost);
				isTargeting = false;
				spellTargeter.transform.position = Vector3.zero;
				StartCoroutine (SpellOnCD ());
				return;
			}
				spellTargeter.transform.position = hit.point;
			
		}
	}

	IEnumerator ShowTargeter()
	{
		yield return new WaitForEndOfFrame ();
		if (GetComponent<GenericLifeScript> ().isDead) 
		{
			yield return null;
		}
		if (GetComponent<GenericManaScript> ().currentMp >= spellCost && !onCD) 
		{
			isTargeting = true;
		} else 
		{
			if (isTargeting == true) 
			{
				isTargeting = false;
				spellTargeter.transform.position = Vector3.zero;
			}
			GameManager.instanceGM.messageManager.SendAnAlertMess ("Not enough Mana!", Color.red);
		}
	}

		IEnumerator SpellOnCD()
		{
			onCD = true;
		spell2Btn.interactable = false;
			yield return new WaitForSeconds (spellCD);
		spell2Btn.interactable = true;
			onCD = false;
		}

		//si on clic sur level up; ca le dit au serveur.
		[Command]
		public void CmdLevelUpTheSpell()
		{
			RpcLvlUpSpell ();
		}

		//le serveur dit a tous les clients y compris lui meme que
		//le sort de ce joueur est devenu plus puissant
		[ClientRpc]
		public void RpcLvlUpSpell()
		{
			spellLvl++;
			spellCost += 5;
			spellCD -= 0.5f;
			spellDmg += 100;
			spellDuration += 0.5f;
			if (isLocalPlayer) 
			{
			GetComponent<PlayerLevelUpManager> ().LooseASpecPt (true);
				int x =(int) spellDmg / 5;
			spellDescription = "Stun et Inflige " + x.ToString () + " dégats toutes les 0,5secondes pendant " + spellDuration.ToString () + " secondes. Cout: " + spellCost.ToString () + " MP. CD: " + spellCD.ToString ();
				spell2Btn.transform.GetChild (0).GetComponentInChildren<Text> ().text = spellDescription;
				//changer ici l'interface du joueur.
			}
		}

		//suffit de linké ca a un bouton d'interface et boom
		public void levelUp()
		{
			CmdLevelUpTheSpell ();
		}
	}

