using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
public class PlayerCastCatapulte : NetworkBehaviour 
{
	
		//deuxieme sort: a mettre sur l'objet joueur.
		// sort de zone avec possibilité de target la ou on veut CC.
		//le sort fait spawn un prefab qui est configuré ici (dégats etc/ durée du CC)
		//le prefab doit etre enregistrer par le networkmanagerObj
		//le sort peut up.
	public bool isUsingCata;
	public int cataChargePrice = 10;
	[SyncVar]public int cataCharges = 1;
		public AudioClip SpellCata;
		public AudioClip OOM;
	public AudioClip boughtACharge;
		string spellDescription;
	[SyncVar]public int spellCost = 1;
		public int spellDmg = 100;
		public float spellCD = 10f;
//		public float spellDuration = 1.5f;
//		public float spellRange = 25f;
		public GameObject spellObj;
		public int spellLvl = 1;
		private bool onCD;
		private float timeSpent;
		public Transform cdCountdown;
	public Button spellCataBtn;
	public Button spellCataLoadBtn;
		private Vector3 castPosDesired;
		public GameObject spellTargeter;
//		public GameObject spellRangeArea;
		public bool isTargeting; // savoir si le joueur cible pour lancer le sort. 
		public LayerMask layer_mask;
		public float durationShake = 10;
		public float amountShake = 10;
		public int startDmg;

	public GameObject cataObj;
		//	private GameObject spell1DescriptionObj;

		void Start()
		{
//			spellRangeArea.SetActive(false);
			if (isLocalPlayer)
			{
			startDmg = spellDmg;
				spellCataBtn = GameObject.Find("SpellCataBtn").GetComponent<Button>();
				spellCataLoadBtn = GameObject.Find("SpellCataLoadBtn").GetComponent<Button>();
				cdCountdown = spellCataBtn.transform.Find ("CDCountdown");
				cdCountdown.gameObject.SetActive (false);
				spellCataBtn.onClick.AddListener(CastThatSpell);
				spellCataLoadBtn.onClick.AddListener(LoadTheCata);
				spellDescription = "Deal " + spellDmg.ToString() + " to everyone on impact. Spend "+ cataChargePrice.ToString() +" to buy a charge (Require a catapulte nearby).";   
				spellCataBtn.transform.GetChild(0).GetComponentInChildren<Text>().text = spellDescription;
				spellCataBtn.transform.GetChild(0).transform.Find ("MpCost").GetComponentInChildren<Text> ().text = cataCharges.ToString();
				spellCataBtn.transform.GetChild(0).transform.Find ("CDTime").GetComponentInChildren<Text> ().text = spellCD.ToString();

			}
			spellTargeter = GameObject.Find("AreaTargeter");

		}
	public void ActualizeCataDmg()
	{
		spellDescription = "Deal " + spellDmg.ToString() + " to everyone on impact. Spend "+ cataChargePrice.ToString() +" to buy a charge (Require a catapulte nearby).";
		spellCataBtn.transform.GetChild(0).GetComponentInChildren<Text>().text = spellDescription;

	}


		//lance le sort sur le serveur.
		//le spawn du préfab est un networkspawn : du coup il apparaitra sur tous les pc..il fera ses trucs sur le serveur
		//bien sur.
		[Command]
		public void CmdCastSpell(Vector3 pos)
		{
		cataCharges -= spellCost;
		GetComponent<AudioSource>().PlayOneShot(SpellCata);
		GameObject go = Instantiate(spellObj, pos, spellTargeter.transform.rotation);
		go.GetComponent<SpellCatapulteArea>().caster = gameObject;
		go.GetComponent<SpellCatapulteArea>().spellDamage = spellDmg;
//		go.GetComponent<SpellCatapulteArea>().duration = spellDuration;
		NetworkServer.Spawn(go);

		}

		//cette fonction est la car on veut vérifier en local déja si on peut lancer le sort avant de
		//demander le lancement du sort sur le serveur...normal.
		public void CastThatSpell()
		{
			if (GetComponent<GenericLifeScript>().isDead)
			{
				return;
			}
			StartCoroutine(ShowTargeter()); // on a besoin d'attendre la fin de frame pour pas que mouseUp soit détecter direct et que le sort se lance cash en cliquant sur l'icone de sort.
		}

		//si t'es un joueur; tu peux cast ce sort avec la touche R : voir pour opti ca en fonction du clavier des gars.

		public void Update()
		{
		if (!isLocalPlayer|| !isUsingCata)
			{
				return;
			}

		if (Input.GetKeyUp(KeyCode.R) && !onCD)
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
//					spellRangeArea.SetActive(false);

					spellTargeter.transform.position = Vector3.zero;
					return;
				}
				if (Input.GetMouseButtonUp(0))
				{
				if ( cataCharges < spellCost || GetComponent<GenericLifeScript>().isDead)
					{
						isTargeting = false;
//						spellRangeArea.SetActive(false);

						spellTargeter.transform.position = Vector3.zero;
						return;
					}
					castPosDesired = hit.point;
					spellTargeter.transform.position = Vector3.zero;
					CmdCastSpell(castPosDesired);
//					cataCharges -= spellCost;
					isTargeting = false;
//					spellRangeArea.SetActive(false);

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
			if (GetComponent<GenericLifeScript>().isDead)
			{
				yield return null;
			}
		if (cataCharges >= spellCost && !onCD)
			{
				isTargeting = true;
			ReziseTheTargeters ();


			}
			else
			{
				if (isTargeting == true)
				{
					isTargeting = false;
//					spellRangeArea.SetActive(false);

					spellTargeter.transform.position = Vector3.zero;
				}
				GetComponent<AudioSource>().PlayOneShot(OOM);
				GameManager.instanceGM.messageManager.SendAnAlertMess("Not enough Charges!", Color.red);
			}
		}

		IEnumerator SpellOnCD()
		{
			onCD = true;
			spellCataBtn.interactable = false;
			StartCoroutine (ShowCDTimer());
			cdCountdown.gameObject.SetActive (true);
			int tmp = (int)(spellCD);
			cdCountdown.gameObject.GetComponentInChildren<Text> ().text = tmp.ToString ();
			yield return new WaitForSeconds(spellCD);
			spellCataBtn.interactable = true;
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
				yield return new WaitForSeconds (0.2f);
			}
		}
		//si on clic sur level up; ca le dit au serveur.
		[Command]
		public void CmdLoadTheCata()
		{
		cataObj.GetComponent<CatapulteObjectScript>().chargesLoaded++;
		cataCharges++;
		GetComponent<PlayerGoldScript> ().ActualGold -= cataChargePrice;
//			RpcLvlUpSpell();
		}
//
//		//le serveur dit a tous les clients y compris lui meme que
//		//le sort de ce joueur est devenu plus puissant
//		[ClientRpc]
//		public void RpcLvlUpSpell()
//		{
//		
////			spellLvl++;
////			spellCost += 8;
////			spellCD -= 2f;
////			spellDmg += 75;
////			spellDuration += 1f;
//			if (isLocalPlayer)
//			{
//				GetComponent<PlayerLevelUpManager>().LooseASpecPt(true);
//				spellDescription =  "Deal " + spellDmg.ToString() + "to everyone on impact."; 
//				spellCataBtn.transform.GetChild(0).GetComponentInChildren<Text>().text = spellDescription;
//				spellCataBtn.transform.GetChild(0).transform.Find ("MpCost").GetComponentInChildren<Text> ().text = spellCost.ToString();
//				spellCataBtn.transform.GetChild(0).transform.Find ("CDTime").GetComponentInChildren<Text> ().text = spellCD.ToString();
//				//changer ici l'interface du joueur.
//			}
//		}

		//suffit de linké ca a un bouton d'interface et boom
		public void LoadTheCata()
		{
		if (GetComponent<PlayerGoldScript> ().ActualGold < cataChargePrice) 
		{
			return;
		}
		GetComponent<AudioSource> ().PlayOneShot (boughtACharge);
			CmdLoadTheCata();
		spellCataBtn.transform.GetChild(0).transform.Find ("MpCost").GetComponentInChildren<Text> ().text = cataCharges.ToString();

		StartCoroutine ( CataLoadProcess());
		}

	IEnumerator CataLoadProcess()
	{
		spellCataLoadBtn.interactable = false;
		yield return new WaitForSeconds (1f);
		spellCataLoadBtn.interactable = true;
	}

	[ClientRpc]
	public void RpcActivateCata(int newCharges)
	{
		if (isLocalPlayer) 
		{
			isUsingCata = true;
			spellCataBtn.interactable = true;
			spellCataLoadBtn.interactable = true;
		
			spellCataBtn.transform.GetChild(0).transform.Find ("MpCost").GetComponentInChildren<Text> ().text = newCharges.ToString();

		}
		if (isServer) 
		{
			cataCharges = newCharges;
			
		}
	}
	[ClientRpc]
	public void RpcDesactivateCata()
	{
		if (isLocalPlayer) 
		{
			isUsingCata = false;
			spellCataBtn.interactable = false;
			spellCataLoadBtn.interactable = false;
			spellCataBtn.transform.GetChild(0).transform.Find ("MpCost").GetComponentInChildren<Text> ().text = cataCharges.ToString();

		}

	}
	public void ReziseTheTargeters()
	{
		spellTargeter.transform.GetChild (0).GetChild (0).localScale = new Vector3 (1f,1f, 1f);
		spellTargeter.transform.GetChild (0).GetChild (1).localScale = new Vector3 (1f,1f, 1f);
		spellTargeter.transform.GetChild (0).localScale = new Vector3 (1f,1f, 1f);


	}
}
