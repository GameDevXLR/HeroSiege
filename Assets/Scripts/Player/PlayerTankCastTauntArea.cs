using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerTankCastTauntArea : NetworkBehaviour 
{


	//premier sort: a mettre sur l'objet joueur.
	// sort de zone avec dégat over time aprés.
	//le sort fait spawn un prefab qui est configuré ici (dégats etc)
	//le prefab doit etre enregistrer par le networkmanagerObj
	//le sort peut up.
	public Sprite spellImg;
	public AudioClip OOM;
	public AudioClip Spell1;
	string spellDescription;
	public int spellCost = 20;
	public int spellDmg = 10;
	public float spellCD;
	public float timeSpent;
	public Transform cdCountdown;
	public float spellDuration = 3f;
	public GameObject spellObj;
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
			spell1Btn.onClick.AddListener(CastThatSpell);
			spell1LvlUpBtn.onClick.AddListener(levelUp);
			int x = (int)spellDmg;
			spellDescription = "Force all enemies around to target you for " + spellDuration.ToString() + " seconds,they loose "+x+" damage.";
			if (PlayerPrefs.GetString ("LANGAGE") == "Fr") 
			{
				spellDescription = "Force tous les ennemis autour de vous a vous attaquer pendant " + spellDuration.ToString() + " secondes. Ils perdent "+x+" dégâts d'attaque.";

			}
			spell1Btn.transform.GetChild(0).GetComponentInChildren<Text>().text = spellDescription;
			spell1Btn.transform.GetChild(0).transform.Find ("MpCost").GetComponentInChildren<Text> ().text = spellCost.ToString();
			spell1Btn.transform.GetChild(0).transform.Find ("CDTime").GetComponentInChildren<Text> ().text = spellCD.ToString();
			spell1Btn.GetComponent<Image> ().sprite = spellImg;

		}
	}

	//lance le sort sur le serveur.
	//le spawn du préfab est un networkspawn : du coup il apparaitra sur tous les pc..il fera ses trucs sur le serveur
	//bien sur.
	[Command]
	public void CmdCastSpell()
	{
        RpcSoundSpell();
		GameObject go = Instantiate(spellObj, transform.position, transform.localRotation);
		go.GetComponent<SpellTankTauntArea>().caster = gameObject;
//		go.GetComponent<AlwaysMove>().target = gameObject;
		go.GetComponent<SpellTankTauntArea>().spellDamage = spellDmg;
		go.GetComponent<SpellTankTauntArea>().duration = spellDuration;
		NetworkServer.Spawn(go);
		GetComponent<GenericManaScript>().CmdLooseManaPoints(spellCost);

	}

    [ClientRpc]
    public void RpcSoundSpell()
    {
        GetComponent<AudioSource>().PlayOneShot(Spell1);
    }

    //cette fonction est la car on veut vérifier en local déja si on peut lancer le sort avant de
    //demander le lancement du sort sur le serveur...normal.
    public void CastThatSpell()
	{
		if (GetComponent<PlayerIGManager>().isDead)
		{
			return;
		}
		if (GetComponent<GenericManaScript>().currentMp >= spellCost && !onCD)
		{
			CmdCastSpell();
			Camera.main.GetComponent<CameraShaker>().ShakeCamera(amountShake, durationShake);
			StartCoroutine(SpellOnCD());
		}
		else
		{
			GetComponent<AudioSource>().PlayOneShot(OOM);
			if (PlayerPrefs.GetString ("LANGAGE") == "Fr") {
				GameManager.instanceGM.messageManager.SendAnAlertMess ("Pas assez de Mana!", Color.red);

			} else {
				GameManager.instanceGM.messageManager.SendAnAlertMess ("Not enough Mana!", Color.red);
			}		}
	}

	//si t'es un joueur; tu peux cast ce sort avec la touche A : voir pour opti ca en fonction du clavier des gars.

	public void Update()
	{
		if (!isLocalPlayer)
		{
			return;
		}

        if (spell1LvlUpBtn.IsActive()
           && Input.GetKey(CommandesController.Instance.getKeycode(CommandesEnum.up))
           && Input.GetKeyUp(CommandesController.Instance.getKeycode(CommandesEnum.sort1)))
        {
            levelUp();
            return;
        }

        if (Input.GetKeyUp(CommandesController.Instance.getKeycode(CommandesEnum.sort1)) && !onCD)
		{
			CastThatSpell();
		}

	}

	IEnumerator SpellOnCD()
	{
		onCD = true;
		StartCoroutine (ShowCDTimer());
		cdCountdown.gameObject.SetActive (true);
		int tmp = (int)(spellCD);
		cdCountdown.gameObject.GetComponentInChildren<Text> ().text = tmp.ToString ();
		spell1Btn.interactable = false;
		yield return new WaitForSecondsRealtime(spellCD);
		spell1Btn.interactable = true;
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
	
        if (isLocalPlayer && GetComponent<PlayerLevelUpManager>().LooseASpecPtAsLocalPlayer(1))
        {

            upgradeSpell();
            int x = (int)spellDmg;
            spellDescription = "Force all enemies around to target you for " + spellDuration.ToString() + " seconds,they loose " + x + " damage.";
            if (PlayerPrefs.GetString("LANGAGE") == "Fr")
            {
                spellDescription = "Force tous les ennemis autour de vous a vous attaquer pendant " + spellDuration.ToString() + " secondes. Ils perdent " + x + " dégâts d'attaque.";

            }
            spell1Btn.transform.GetChild(0).GetComponentInChildren<Text>().text = spellDescription;
            spell1Btn.transform.GetChild(0).transform.Find("MpCost").GetComponentInChildren<Text>().text = spellCost.ToString();
            spell1Btn.transform.GetChild(0).transform.Find("CDTime").GetComponentInChildren<Text>().text = spellCD.ToString();
            //			spell1Btn.transform.GetChild (1).transform.GetComponent<Animator> ().SetBool ("Enable", true);
            //			spell1Btn.transform.GetChild (1).transform.GetComponent<Animator> ().Play("BtnCompPts");
            //changer ici l'interface du joueur.
        }
        else if (GetComponent<PlayerLevelUpManager>().LooseASpecPt(1))
        {
            upgradeSpell();
        }
    }

    public void upgradeSpell()
    {
        spellLvl++;
        spellCost += 5;
        spellCD -= 1f;
        spellDmg += 6;
        spellDuration += 1f;
    }

    //suffit de linké ca a un bouton d'interface et boom
    public void levelUp()
	{
		CmdLevelUpTheSpell();
	}
}
