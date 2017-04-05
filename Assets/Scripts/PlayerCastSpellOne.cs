using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[NetworkSettings (channel = 2, sendInterval = 0.1f)]
public class PlayerCastSpellOne : NetworkBehaviour 
{
	//premier sort: a mettre sur l'objet joueur.
	// sort de zone avec dégat over time aprés.
	//le sort fait spawn un prefab qui est configuré ici (dégats etc)
	//le prefab doit etre enregistrer par le networkmanagerObj
	//le sort peut up.
	public AudioClip OOM;
	public AudioClip Spell1;
	string spellDescription;
	public int spellCost = 30;
	public int spellDmg = 50;
	public float spellCD;
	public float spellDuration = 1.5f;
	public GameObject spellObj;
	public int spellLvl = 1;
	private bool onCD;
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
			int x =(int) spellDmg / 5;
			spellDescription = "Inflige " + spellDmg.ToString() + " dégats a l'impact puis " + x.ToString() + " dégats toutes les 0,5secondes pendant " + spellDuration.ToString() + " secondes. Cout: " + spellCost.ToString()+ " MP. CD: "+ spellCD.ToString();
			spell1Btn.transform.GetChild (0).GetComponentInChildren<Text> ().text = spellDescription;


		}
	}

	//lance le sort sur le serveur.
	//le spawn du préfab est un networkspawn : du coup il apparaitra sur tous les pc..il fera ses trucs sur le serveur
	//bien sur.
	[Command]
	public void CmdCastSpell()
	{
			GetComponent<AudioSource> ().PlayOneShot (Spell1);
			GameObject go = Instantiate (spellObj, transform.position, transform.localRotation);
			go.GetComponent<SpellAreaDamage> ().caster = gameObject;
			go.GetComponent<AlwaysMove> ().target = gameObject;
			go.GetComponent<SpellAreaDamage> ().spellDamage = spellDmg;
			go.GetComponent<SpellAreaDamage> ().duration = spellDuration;
			NetworkServer.Spawn (go);
			GetComponent<GenericManaScript> ().CmdLooseManaPoints (spellCost);

	}
	//cette fonction est la car on veut vérifier en local déja si on peut lancer le sort avant de
	//demander le lancement du sort sur le serveur...normal.
	public void CastThatSpell()
	{
		if (GetComponent<GenericLifeScript> ().isDead) 
		{
			return;
		}
		if (GetComponent<GenericManaScript> ().currentMp >= spellCost && !onCD) {
			CmdCastSpell ();
			StartCoroutine (SpellOnCD ());
		} else 
		{
			GameManager.instanceGM.messageManager.SendAnAlertMess ("Not enough Mana!", Color.red);
			GetComponent<AudioSource> ().PlayOneShot (OOM);
		}
	}

	//si t'es un joueur; tu peux cast ce sort avec la touche A : voir pour opti ca en fonction du clavier des gars.

	public void Update()
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
		spell1Btn.interactable = false;
		yield return new WaitForSeconds (spellCD);
		spell1Btn.interactable = true;
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
		spellCost += 10;
		spellCD -= 0.5f;
		spellDmg += 20;
		spellDuration += 0.5f;
		if (isLocalPlayer) 
		{
			GetComponent<PlayerLevelUpManager> ().LooseASpecPt (false);
			int x =(int) spellDmg / 5;

			spellDescription = "Inflige " + spellDmg.ToString() + " dégats a l'impact puis " + x.ToString() + " dégats toutes les 0,5secondes pendant " + spellDuration.ToString() + " secondes. Cout: " + spellCost.ToString() +" MP.CD: "+ spellCD.ToString();

			spell1Btn.transform.GetChild (0).GetComponentInChildren<Text> ().text = spellDescription;
			//changer ici l'interface du joueur.
		}
	}

	//suffit de linké ca a un bouton d'interface et boom
	public void levelUp()
	{
		CmdLevelUpTheSpell ();
	}
}
