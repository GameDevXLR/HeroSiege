using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerStatPlus : NetworkBehaviour {


	//premier sort: a mettre sur l'objet joueur.
	// sort de zone avec dégat over time aprés.
	//le sort fait spawn un prefab qui est configuré ici (dégats etc)
	//le prefab doit etre enregistrer par le networkmanagerObj
	//le sort peut up.
	public Sprite spellImg;
	public AudioClip OOM;
	public AudioClip Spell1;
	string spellDescription;
	[SyncVar]public int dmgBonus = 4;
	[SyncVar]public int hpBonus = 30;
	[SyncVar]public int mpBonus = 20;
	[SyncVar]public bool doubleHPBonus;
	[SyncVar]public bool doubleMPBonus;
	[SyncVar]public bool doubleDpsBonus;
	public int spellLvl = 1;
	private Button spell1Btn;
	private Button spell1LvlUpBtn;

	void Start()
	{
		if (doubleDpsBonus) 
		{
			dmgBonus *= 2;
		}
		if (doubleHPBonus) 
		{
			hpBonus *= 2;
		}		
		if (doubleMPBonus) 
		{
			mpBonus *= 2;
		}
		if (isLocalPlayer)
		{
			spell1Btn = GameObject.Find("StatPlusBtn").GetComponent<Button>();
			spell1LvlUpBtn = GameObject.Find("StatPlusBtn").GetComponent<Button>();
			spell1LvlUpBtn.onClick.AddListener(levelUp);
			spellDescription = "Boost your hero by adding "+hpBonus+" max hit point, "+mpBonus+"max mana and "+dmgBonus+" damage.";
			spell1Btn.transform.GetChild(0).GetComponentInChildren<Text>().text = spellDescription;
//			spell1Btn.GetComponent<Image> ().sprite = spellImg;
			if (!isServer) 
			{
				Invoke("ActuDescription",3f);
			}
		}
	}

	//lance le sort sur le serveur.
	//le spawn du préfab est un networkspawn : du coup il apparaitra sur tous les pc..il fera ses trucs sur le serveur
	//bien sur.
	//	[Command]
	//	public void CmdCastSpell()
	//	{
	//		GetComponent<AudioSource>().PlayOneShot(Spell1);
	//		GameObject go = Instantiate(spellObj, transform.position, transform.localRotation);
	//		go.GetComponent<SpellTankTauntArea>().caster = gameObject;
	//		//		go.GetComponent<AlwaysMove>().target = gameObject;
	//		go.GetComponent<SpellTankTauntArea>().spellDamage = spellDmg;
	//		go.GetComponent<SpellTankTauntArea>().duration = spellDuration;
	//		NetworkServer.Spawn(go);
	//		GetComponent<GenericManaScript>().CmdLooseManaPoints(spellCost);
	//
	//	}
	//cette fonction est la car on veut vérifier en local déja si on peut lancer le sort avant de
	//demander le lancement du sort sur le serveur...normal.
	//	public void CastThatSpell()
	//	{
	//		if (GetComponent<GenericLifeScript>().isDead)
	//		{
	//			return;
	//		}
	//		if (GetComponent<GenericManaScript>().currentMp >= spellCost && !onCD)
	//		{
	//			CmdCastSpell();
	//			Camera.main.GetComponent<CameraShaker>().ShakeCamera(amountShake, durationShake);
	//			StartCoroutine(SpellOnCD());
	//		}
	//		else
	//		{
	//			GameManager.instanceGM.messageManager.SendAnAlertMess("Not enough Mana!", Color.red);
	//			GetComponent<AudioSource>().PlayOneShot(OOM);
	//		}
	//	}

	//si t'es un joueur; tu peux cast ce sort avec la touche A : voir pour opti ca en fonction du clavier des gars.

	//	public void Update()
	//	{
	//		if (!isLocalPlayer)
	//		{
	//			return;
	//		}
	//
	//		if (Input.GetKeyUp(KeyCode.A) && !onCD)
	//		{
	//			CastThatSpell();
	//		}
	//
	//	}

	//	IEnumerator SpellOnCD()
	//	{
	//		onCD = true;
	//		StartCoroutine (ShowCDTimer());
	//		cdCountdown.gameObject.SetActive (true);
	//		int tmp = (int)(spellCD);
	//		cdCountdown.gameObject.GetComponentInChildren<Text> ().text = tmp.ToString ();
	//		spell1Btn.interactable = false;
	//		yield return new WaitForSeconds(spellCD);
	//		spell1Btn.interactable = true;
	//		cdCountdown.gameObject.SetActive (false);
	//		timeSpent = 0f;
	//		onCD = false;
	//	}


	//	IEnumerator ShowCDTimer()
	//	{
	//		while (onCD) 
	//		{
	//			int tmp =(int) (spellCD - timeSpent);
	//			if (tmp >= 0) 
	//			{
	//				cdCountdown.gameObject.GetComponentInChildren<Text> ().text = tmp.ToString ();
	//				timeSpent += 0.2f;
	//			}
	//			yield return new WaitForSeconds (0.2f);
	//		}
	//	}
	//si on clic sur level up; ca le dit au serveur.
	[Command]
	public void CmdLevelUpTheSpell()
	{
		UpTheStats ();
		RpcLvlUpSpell();
	}

	//le serveur dit a tous les clients y compris lui meme que
	//le sort de ce joueur est devenu plus puissant
	[ClientRpc]
	public void RpcLvlUpSpell()
	{
		spellLvl++;
		if (isLocalPlayer)
		{
			GetComponent<PlayerLevelUpManager>().LooseASpecPt(4);
			//changer ici l'interface du joueur.
		}
	}

	//suffit de linké ca a un bouton d'interface et boom
	public void levelUp()
	{
		CmdLevelUpTheSpell();
	}
	//faire ici tous les up de syncvar on est sur le serveur.
	public void UpTheStats()
	{
		GetComponent<PlayerAutoAttack> ().damage += dmgBonus;
		GetComponent<GenericManaScript> ().maxMp += mpBonus;
		GetComponent<PlayerIGManager> ().maxHp += hpBonus;
	}
	public void ActuDescription()
	{
		spellDescription = "Boost your hero by adding "+hpBonus+" max hit point, "+mpBonus+"max mana and "+dmgBonus+" damage.";
		spell1Btn.transform.GetChild(0).GetComponentInChildren<Text>().text = spellDescription;
	}
}
