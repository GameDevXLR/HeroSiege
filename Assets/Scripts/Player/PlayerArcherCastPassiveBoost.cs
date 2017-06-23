using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerArcherCastPassiveBoost : NetworkBehaviour {


	//premier sort: a mettre sur l'objet joueur.
	// sort de zone avec dégat over time aprés.
	//le sort fait spawn un prefab qui est configuré ici (dégats etc)
	//le prefab doit etre enregistrer par le networkmanagerObj
	//le sort peut up.
	public Sprite spellImg;
	public AudioClip OOM;
	public AudioClip Spell1;
	string spellDescription;
	public int spellCost = 0;
	public int stunChances = 2;
	public int dodgeChances = 3;
	public int levelupStunChanceBonus = 1;
	public int levelupDodgeChanceBonus = 1;

//	public float spellCD;
	public float timeSpent;
	public Transform cdCountdown;
//	public float spellDuration = 3f;
//	public GameObject spellObj;
	public int spellLvl = 1;
	private bool onCD;
	private Button spell1Btn;
	private Button spell1LvlUpBtn;
	public float durationShake = 5;
	public float amountShake = 0;

	//	private GameObject spell1DescriptionObj;

	void Start()
	{
		if (isLocalPlayer)
		{
			spell1Btn = GameObject.Find("Spell1Btn").GetComponent<Button>();
			spell1LvlUpBtn = GameObject.Find("Spell1LvlUpBtn").GetComponent<Button>();
			cdCountdown = spell1Btn.transform.Find ("CDCountdown");
			cdCountdown.gameObject.SetActive (false);
//			spell1Btn.onClick.AddListener(CastThatSpell);
			spell1LvlUpBtn.onClick.AddListener(levelUp);
			spellDescription = "Give you " + stunChances + "% chances to stun and deal "+ spellLvl/2+" times more damages and "+ dodgeChances+"% dodge chances.";
			if (PlayerPrefs.GetString ("LANGAGE") == "Fr") 
			{
				spellDescription = "Vous donne " + stunChances + "% de chances de stun et d'infliger "+ spellLvl/2+" fois plus de dégats et ajoute "+ dodgeChances+"% chances d'esquiver.";

			}
			spell1Btn.transform.GetChild(0).GetComponentInChildren<Text>().text = spellDescription;
			spell1Btn.transform.GetChild(0).transform.Find ("MpCost").GetComponentInChildren<Text> ().text = "0";
			spell1Btn.transform.GetChild(0).transform.Find ("CDTime").GetComponentInChildren<Text> ().text = "0";
			spell1Btn.GetComponent<Image> ().sprite = spellImg;

		}
		if (isServer) 
		{
			GetComponent<PlayerAutoAttack> ().critChance += stunChances;
			GetComponent<PlayerAutoAttack> ().critFactor++;
			GetComponent<PlayerIGManager> ().dodge += dodgeChances;
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
		
		
        if (isLocalPlayer && GetComponent<PlayerLevelUpManager>().LooseASpecPtAsLocalPlayer(1))
        {
            upgradeSpell();
            spellDescription = "Give you " + stunChances + "% chances to stun and deal " + spellLvl / 2 + " times more damages and " + dodgeChances + "% dodge chances.";
            if (PlayerPrefs.GetString("LANGAGE") == "Fr")
            {
                spellDescription = "Vous donne " + stunChances + "% de chances de stun et d'infliger " + spellLvl / 2 + " fois plus de dégats et ajoute " + dodgeChances + "% chances d'esquiver.";

            }
            spell1Btn.transform.GetChild(0).GetComponentInChildren<Text>().text = spellDescription;
            //			spell1Btn.transform.GetChild (1).transform.GetComponent<Animator> ().SetBool ("Enable", true);
            //			spell1Btn.transform.GetChild (1).transform.GetComponent<Animator> ().Play("BtnCompPts");
            //changer ici l'interface du joueur.
        }
        else if (GetComponent<PlayerLevelUpManager>().LooseASpecPt(3))
        {
            upgradeSpell();
        }
    }

    public void upgradeSpell()
    {
        spellLvl++;
        stunChances += 1;
        dodgeChances += 1;
    }


    //suffit de linké ca a un bouton d'interface et boom
    public void levelUp()
	{
		CmdLevelUpTheSpell();
	}
	//faire ici tous les up de syncvar on est sur le serveur.
	public void UpTheStats()
	{
		GetComponent<PlayerAutoAttack> ().critChance += levelupStunChanceBonus;
		if (spellLvl == 2 || spellLvl == 4 || spellLvl == 6 || spellLvl == 8 || spellLvl == 10) 
		{ 
			GetComponent<PlayerAutoAttack> ().critFactor++;
		}
		GetComponent<PlayerIGManager> ().dodge += levelupDodgeChanceBonus;
	}
}
