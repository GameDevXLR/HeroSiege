﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.AI;

[NetworkSettings(channel = 2, sendInterval = 0.1f)]
public class PlayerCastTowerMage : NetworkBehaviour {


//invocation de la premiere tour.
	//c'est ici qu'on configure la tour en fait, c'est le sort du joueur quoi.
	//ca peut up et tout comme un sort normal blablabla...
	public GameObject actualTower;
	private GameObject previousTower;
	public Sprite spellImg;
	public AudioClip SpellCC;
	public AudioClip OOM;
	string spellDescription;
	public int spellCost = 80;
	public int spellDmg = 50;
	public float spellCD;
	public float spellDuration = 1.5f;
	public float spellRange = 20f;
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
			spell2Btn = GameObject.Find("Spell2Btn").GetComponent<Button>();
			spell2LvlUpBtn = GameObject.Find("Spell2LvlUpBtn").GetComponent<Button>();
			cdCountdown = spell2Btn.transform.Find ("CDCountdown");
			cdCountdown.gameObject.SetActive (false);

			spell2Btn.onClick.AddListener(CastThatSpell);
			spell2LvlUpBtn.onClick.AddListener(levelUp);
			//			int x = (int)spellDmg / 5;
			spellDescription = "Invoke a tower dealing "+ spellDmg.ToString()+" damage. Last "+spellDmg*6+" seconds. The more it attacks, the less it stay. Can be recharged.";
			if (PlayerPrefs.GetString ("LANGAGE") == "Fr") 
			{
				spellDescription = "Invoque une tour infligeant "+ spellDmg.ToString()+" dégâts. Reste "+spellDmg*6+" secondes max.Peut être rechargé.";

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
	public void CmdCastSpell(Vector3 pos)
	{


		RpcSoundSpell();
		GameObject go = Instantiate(spellObj, pos, spellTargeter.transform.rotation);
		go.GetComponent<TowerPetAutoA> ().towerOwner = gameObject;
		go.GetComponent<TowerPetAutoA> ().damage = spellDmg;
		go.GetComponent<TowerIGManager> ().maxHp = spellDmg * 2;
		go.GetComponent<TowerIGManager> ().currentHp = spellDmg * 2;
		go.GetComponent<TowerIGManager> ().regenHp = -1;
//		actualPet = go;
		NetworkServer.Spawn(go);


	}

	[ClientRpc]
	public void RpcSoundSpell()
	{
		GetComponent<AudioSource>().PlayOneShot(SpellCC);
	}

//	public void DestroyThePrevPet()
//	{
//		NetworkServer.Destroy (previousPet);
//	}
	//cette fonction est la car on veut vérifier en local déja si on peut lancer le sort avant de
	//demander le lancement du sort sur le serveur...normal.
	public void CastThatSpell()
	{
		if (GetComponent<PlayerIGManager>().isDead)
		{
			return;
		}
		StartCoroutine(ShowTargeter()); // on a besoin d'attendre la fin de frame pour pas que mouseUp soit détecter direct et que le sort se lance cash en cliquant sur l'icone de sort.
	}

	//si t'es un joueur; tu peux cast ce sort avec la touche Z : voir pour opti ca en fonction du clavier des gars.

	public void Update()
	{
		if (!isLocalPlayer)
		{
			return;
		}
		if (spell2LvlUpBtn.IsActive()
			&& !GameManager.instanceGM.isInTchat 
			&& Input.GetKey(CommandesController.Instance.getKeycode(CommandesEnum.up))
			&& Input.GetKeyUp(CommandesController.Instance.getKeycode(CommandesEnum.sort2)))
		{
			levelUp();
			return;
		}

		if (!GameManager.instanceGM.isInTchat 
			&& Input.GetKeyUp(CommandesController.Instance.getKeycode(CommandesEnum.sort2)) 
			&& !onCD)
		{
			CastThatSpell();
		}
		if (!isTargeting)
		{
			return;
		}
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out hit, 80f, layer_mask))
		{
			if (Input.GetMouseButtonUp(1))
			{
				isTargeting = false;
				spellRangeArea.SetActive(false);

				spellTargeter.transform.position = Vector3.zero;
				return;
			}
			if (Input.GetMouseButtonUp(0))
			{
				if (Vector3.Distance(hit.point, transform.position) > spellRange || GetComponent<GenericManaScript>().currentMp < spellCost || GetComponent<PlayerIGManager>().isDead)
				{
					isTargeting = false;
					spellRangeArea.SetActive(false);

					spellTargeter.transform.position = Vector3.zero;
					return;
				}
				castPosDesired = hit.point;
				spellTargeter.transform.position = Vector3.zero;
				CmdCastSpell(castPosDesired);
				GetComponent<GenericManaScript>().CmdLooseManaPoints(spellCost);
				isTargeting = false;
				spellRangeArea.SetActive(false);

				spellTargeter.transform.position = Vector3.zero;
				StartCoroutine(SpellOnCD());

				Camera.main.GetComponent<CameraShaker>().ShakeCamera(amountShake, durationShake);
				return;
			}
			spellTargeter.transform.position = hit.point;

		}
	}

	IEnumerator ShowTargeter()
	{
		yield return new WaitForEndOfFrame();
		if (GetComponent<PlayerIGManager>().isDead)
		{
			yield return null;
		}
		if (GetComponent<GenericManaScript>().currentMp >= spellCost && !onCD)
		{
			isTargeting = true;
			ReziseTheTargeters ();
			spellRangeArea.SetActive(true);


		}
		else
		{
			if (isTargeting == true)
			{
				isTargeting = false;
				spellRangeArea.SetActive(false);

				spellTargeter.transform.position = Vector3.zero;
			}
			GetComponent<AudioSource>().PlayOneShot(OOM);
			if (PlayerPrefs.GetString ("LANGAGE") == "Fr") {
				GameManager.instanceGM.messageManager.SendAnAlertMess ("Pas assez de Mana!", Color.red);

			} else {
				GameManager.instanceGM.messageManager.SendAnAlertMess ("Not enough Mana!", Color.red);
			}		}
	}

	IEnumerator SpellOnCD()
	{
		onCD = true;
		spell2Btn.interactable = false;
		StartCoroutine (ShowCDTimer());
		cdCountdown.gameObject.SetActive (true);
		int tmp = (int)(spellCD);
		cdCountdown.gameObject.GetComponentInChildren<Text> ().text = tmp.ToString ();
		yield return new WaitForSecondsRealtime(spellCD);
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
				timeSpent += 0.5f;
			}
			yield return new WaitForSecondsRealtime (0.5f);
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

		if (isLocalPlayer && GetComponent<PlayerLevelUpManager>().LooseASpecPtAsLocalPlayer(2))
		{

			upgradeSpell();
			//            int x = (int)spellDmg / 5;
			spellDescription = "Invoke a tower dealing "+ spellDmg.ToString()+" damage. Last "+spellDmg*6+" seconds. The more it attacks, the less it stay. Can be recharged.";
			if (PlayerPrefs.GetString ("LANGAGE") == "Fr") 
			{
				spellDescription = "Invoque une tour infligeant "+ spellDmg.ToString()+" dégâts. Reste "+spellDmg*6+" secondes max.Peut être rechargé.";

			}
			spell2Btn.transform.GetChild(0).GetComponentInChildren<Text>().text = spellDescription;
			spell2Btn.transform.GetChild(0).transform.Find("MpCost").GetComponentInChildren<Text>().text = spellCost.ToString();
			spell2Btn.transform.GetChild(0).transform.Find("CDTime").GetComponentInChildren<Text>().text = spellCD.ToString();
			//changer ici l'interface du joueur.
		}
		else if (GetComponent<PlayerLevelUpManager>().LooseASpecPt(2))
		{
			upgradeSpell();
		}
	}

	public void upgradeSpell()
	{
		spellLvl++;
		spellCost += 16;
		spellCD -= 3f;
		spellDmg += 4*spellLvl;
		if (onCD) 
		{
			timeSpent -= 3f;
		}
		//		spellDuration += 1f;
	}

	//suffit de linké ca a un bouton d'interface et boom
	public void levelUp()
	{
		CmdLevelUpTheSpell();
	}
	public void ReziseTheTargeters()
	{
		spellRangeArea.transform.GetChild (0).GetChild (0).localScale = new Vector3 (2.2f, 2.2f, 1f);
		spellRangeArea.transform.GetChild (0).localScale = new Vector3 (2.2f, 2.2f, 1f);

		spellTargeter.transform.GetChild (0).GetChild (0).localScale = new Vector3 (.3f, .3f, 1f);
		spellTargeter.transform.GetChild (0).GetChild (1).localScale = new Vector3 (.3f, .3f, 1f);
		spellTargeter.transform.GetChild (0).localScale = new Vector3 (.3f, .3f, 1f);


	}
}
