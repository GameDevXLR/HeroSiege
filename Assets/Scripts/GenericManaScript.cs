using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

//cette ligne permet de dire quel type de transfert est utilisé; le chann 0 est configurer pour etre séquencé et fiable.
//pas besoin d'actualiser souvent : l'interval est donc de 0.5s...suffisant largement. ca se verra pas pour le joueur.
[NetworkSettings(channel = 0, sendInterval = 0.5f)]
public class GenericManaScript : NetworkBehaviour 
{

	//ce script gere la mana de l'objet auquel il est attaché.
	//Il fonctionne en réseau et gere également l'interface lié a la mana de tous les objets.

	//variables clés
	[SyncVar]public int maxMp = 100;
	[SyncVar]public int currentMp = 80;
	public int regenMp;
	public int levelUpBonusMP;

	// variables pour le serveur
	public float timeBetweenTic = 1f;
	private float lastTic;

	//variables d'affichage UI
	public RectTransform manaBar;
	public RectTransform manaBarMain;
	public Text playerMPTxt;


	void Start () 
	{
		if (isServer) 
		{		
			lastTic = 0f;
		}
		if (isLocalPlayer) 
		{
			manaBarMain = GameObject.Find ("PlayerManaBarMain").GetComponent<RectTransform> ();
			playerMPTxt = GameObject.Find ("PlayerMpText").GetComponent<Text>();
			playerMPTxt.text = currentMp.ToString () + " / " + maxMp.ToString ();

		}
	}
	
	//La mise a jour des mp se fait sur le serveur qui retransmet a tous les clients grace a la fonction Rpc.
	//Et aussi grace aux syncVar. a noté que grace au tictime; on evite de la run trop souvent.
	[ServerCallback]
	void Update () 
	{
		if (currentMp > maxMp) 
		{
			currentMp = maxMp;
			RpcActualizeThatMana ();

		}
		if (currentMp < 0) 
		{
			currentMp = 0;
			RpcActualizeThatMana ();
		}
		if (Time.time > lastTic)	//si le temps entre 2 tics est écoulé : 
		{  
			lastTic = Time.time + timeBetweenTic;
			if (currentMp < maxMp) 
			{
				RegenerateMp ();
			}
		} 

	}

	//auto regen des mp tous les timeTic;
	public void RegenerateMp ()
	{
		if (GetComponent<GenericLifeScript> ().isDead) 
		{
			manaBar.GetComponentInParent<Canvas> ().enabled = false;
			return;
		}
		if (currentMp <= maxMp) 
		{
			currentMp += regenMp;
			RpcActualizeThatMana ();
		}
	}
	//a invoquer sur le serveur pour faire perdre des mp au joueur.
	[Command]
	public void CmdLooseManaPoints(int mana)
	{
		currentMp -= mana;
		RpcActualizeThatMana ();
	}

	//fait up le joueur; dit a tous les clients que le joueur a up.
	public void LevelUp()
	{
		maxMp += levelUpBonusMP;
		currentMp = maxMp;
		if (isServer) {
			RpcActualizeThatMana ();
		}
	}

	//permet d'actualiser la mana sur tous les clients du jeu et en fonction de
	//si c'est le localplayer ou pas; ca actualise l'interface aussi.
	[ClientRpc]
	public void RpcActualizeThatMana()
	{
		if(GetComponent<GenericLifeScript>().isDead)
			{
			manaBar.GetComponentInParent<Canvas> ().enabled = false;
			return;
			}
		
		float x = (float)currentMp / maxMp;
		manaBar.localScale = new Vector3 (x, 1f, 1f);
		if (isLocalPlayer) 
		{
			if (manaBarMain == null) 
			{
				manaBarMain = GameObject.Find ("PlayerManaBarMain").GetComponent<RectTransform> ();

			}
			if (playerMPTxt == null) 
			{
				playerMPTxt = GameObject.Find ("PlayerMpText").GetComponent<Text>();

			}
			manaBarMain.localScale = new Vector3 (x, 1f, 1f);
			playerMPTxt.text = currentMp.ToString () + " / " + maxMp.ToString ();

		}
		if (currentMp < maxMp) 
		{
			manaBar.GetComponentInParent<Canvas> ().enabled = true;
		} else 
		{
			manaBar.GetComponentInParent<Canvas> ().enabled = false;
		}
	}
}
