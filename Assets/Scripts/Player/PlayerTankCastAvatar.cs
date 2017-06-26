using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerTankCastAvatar : NetworkBehaviour 
{
	public Sprite spellImg;
	public AudioClip SpellCC;
	public AudioClip OOM;
	string spellDescription;
	public int spellCost = 50;
	public int spellDmg = 20;
	public float spellCD;
	public float spellDuration = 30f;
	public float spellRange = 25f;
	public GameObject spellObj;
	public int spellLvl = 1;
	private bool onCD;
	private float timeSpent;
	public Transform cdCountdown;
	private Button spell2Btn;
	private Button spell2LvlUpBtn;
	private Vector3 castPosDesired;
	public GameObject spellTargeter;
	public GameObject spellRangeArea;
	public bool isTargeting; // savoir si le joueur cible pour lancer le sort. 
	public LayerMask layer_mask;
	public float durationShake = 10;
	public float amountShake = 10;
	//	private GameObject spell1DescriptionObj;

	void Start()
	{
		spellRangeArea.SetActive(false);
		if (isLocalPlayer)
		{
			spell2Btn = GameObject.Find("Spell3Btn").GetComponent<Button>();
			spell2LvlUpBtn = GameObject.Find("Spell3LvlUpBtn").GetComponent<Button>();
			GetComponent<PlayerLevelUpManager> ().AvoidEarlyUltiUp ();
			cdCountdown = spell2Btn.transform.Find ("CDCountdown");
			cdCountdown.gameObject.SetActive (false);

			spell2Btn.onClick.AddListener(CastThatSpell);
			spell2LvlUpBtn.onClick.AddListener(levelUp);
			int x = (int)spellDmg*10;
			spellDescription = "Strenghten your hero, he deals "+spellDmg+ " more per attack and get " + x.ToString () + " health for " + spellDuration.ToString () + " seconds.";           
			if (PlayerPrefs.GetString ("LANGAGE") == "Fr") 
			{
				spellDescription = "Endurci votre héro, il inflige "+spellDmg+ " dégâts de plus par attaque et recoit un bonus de " + x.ToString () + " pv pendant " + spellDuration.ToString () + " secondes.";            

			}
			spell2Btn.transform.GetChild(0).GetComponentInChildren<Text>().text = spellDescription;
			spell2Btn.transform.GetChild(0).transform.Find ("MpCost").GetComponentInChildren<Text> ().text = spellCost.ToString();
			spell2Btn.transform.GetChild(0).transform.Find ("CDTime").GetComponentInChildren<Text> ().text = spellCD.ToString();
			spell2Btn.GetComponent<Image> ().sprite = spellImg;

		}
		spellTargeter = GameObject.Find("AreaTargeter");

	}
	//lance le sort sur le serveur.
	//le spawn du préfab est un networkspawn : du coup il apparaitra sur tous les pc..il fera ses trucs sur le serveur
	//bien sur.
	[Command]
	public void CmdCastSpell()
	{
		RpcCastSpell ();
		GetComponent<GenericManaScript>().CmdLooseManaPoints(spellCost);

	}

	[ClientRpc]
	public void RpcCastSpell()
	{
		GetComponent<AudioSource>().PlayOneShot(SpellCC);
		StartCoroutine (AvatarProcedure (spellDuration));
	}

	IEnumerator AvatarProcedure(float dur)
	{
		GetComponent<PlayerIGManager>().deadAnimChildMesh.transform.localScale = new Vector3 (2f, 2f, 2f);
		if (isServer) 
		{
			GetComponent<PlayerIGManager> ().maxHp += spellDmg * 10;
			GetComponent<PlayerIGManager> ().currentHp += spellDmg * 10;
			GetComponent<PlayerAutoAttack> ().damage += spellDmg ;
		}
		yield return new WaitForSeconds (dur);
		GetComponent<PlayerIGManager>().deadAnimChildMesh.transform.localScale = Vector3.one;
		if (isServer) 
		{
			GetComponent<PlayerIGManager> ().maxHp -= spellDmg * 10;
//			GetComponent<GenericLifeScript> ().currentHp -= spellDmg * 10;
			GetComponent<PlayerAutoAttack> ().damage -= spellDmg ;
		}
	}

	//cette fonction est la car on veut vérifier en local déja si on peut lancer le sort avant de
	//demander le lancement du sort sur le serveur...normal.
	public void CastThatSpell()
	{
		if (GetComponent<PlayerIGManager>().isDead)
		{
			return;
		}
		if (GetComponent<GenericManaScript> ().currentMp < spellCost) 
		{
			return;
		}
		StartCoroutine(SpellOnCD()); // on a besoin d'attendre la fin de frame pour pas que mouseUp soit détecter direct et que le sort se lance cash en cliquant sur l'icone de sort.
		CmdCastSpell();
	}
	//si t'es un joueur; tu peux cast ce sort avec la touche Z : voir pour opti ca en fonction du clavier des gars.

	public void Update()
	{
		if (!isLocalPlayer)
		{
			return;
		}

		if (Input.GetKeyUp(KeyCode.E) && !onCD)
		{
			CastThatSpell();
		}

	}


	IEnumerator SpellOnCD()
	{
		onCD = true;
		spell2Btn.interactable = false;
		StartCoroutine (ShowCDTimer());
		cdCountdown.gameObject.SetActive (true);
		int tmp = (int)(spellCD);
		cdCountdown.gameObject.GetComponentInChildren<Text> ().text = tmp.ToString ();
		yield return new WaitForSeconds(spellCD);
		spell2Btn.interactable = true;
		cdCountdown.gameObject.SetActive (false);
		timeSpent = 0f;
		onCD = false;
	}
	IEnumerator ShowCDTimer()
	{
		while (onCD) 
		{
			int tmp =(int) (spellCD - timeSpent);
			if (tmp >= 0) 
			{
				cdCountdown.gameObject.GetComponentInChildren<Text> ().text = tmp.ToString ();
				timeSpent += 0.2f;
			}
			yield return new WaitForSecondsRealtime (0.2f);
		}
	}
	//si on clic sur level up; ca le dit au serveur.
	[Command]
	public void CmdLevelUpTheSpell()
	{
		RpcLvlUpSpell();
	}

	//le serveur dit a tous les clients y compris lui meme que
	//le sort de ce joueur est devenu plus puissant
	[ClientRpc]
	public void RpcLvlUpSpell()
	{
		spellLvl++;
		spellCost += 16;
		spellCD -= 3f;
		spellDmg += 16;
		spellDuration += 1f;
		if (isLocalPlayer)
		{
			GetComponent<PlayerLevelUpManager>().LooseASpecPt(3);
			int x = (int)spellDmg *10;
			spellDescription = "Strenghten your hero, he deals "+spellDmg+ " more per attack and get " + x.ToString () + " health for " + spellDuration.ToString () + " seconds.";            
			if (PlayerPrefs.GetString ("LANGAGE") == "Fr") 
			{
				spellDescription = "Endurci votre héro, il inflige "+spellDmg+ " dégâts de plus par attaque et recoit un bonus de " + x.ToString () + " pv pendant " + spellDuration.ToString () + " secondes.";            

			}
			spell2Btn.transform.GetChild(0).GetComponentInChildren<Text>().text = spellDescription;
			spell2Btn.transform.GetChild(0).transform.Find ("MpCost").GetComponentInChildren<Text> ().text = spellCost.ToString();
			spell2Btn.transform.GetChild(0).transform.Find ("CDTime").GetComponentInChildren<Text> ().text = spellCD.ToString();
			//changer ici l'interface du joueur.
		}
	}

	//suffit de linké ca a un bouton d'interface et boom
	public void levelUp()
	{
		CmdLevelUpTheSpell();
	}


}
