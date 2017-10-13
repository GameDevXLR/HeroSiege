using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;

public class PlayerGoldScript : NetworkBehaviour 
{
	//petit script qui gere l'or d'un joueur. rien de fou : synchro sur réseau.
	public Text goldDisplay;
	public float timeBetweenGoldDrop;
	private float lastGoldDrop;
	public int goldPerDrop;
	[SyncVar]public bool isDropping;
	[SyncVar(hook ="GoldActualize" )]public int ActualGold;
    public GameObject goldCanvasPrefab;


    public void GetGold(int gold)
	{
		if (!isServer) 
		{
			return;
		}
		ActualGold += gold;
	}

	void Start()
	{
		if (isLocalPlayer) 
		{
			goldDisplay = GameObject.Find ("PlayerGold").GetComponent<Text> ();
		}
		if (isServer) 
		{
			lastGoldDrop = Time.time + timeBetweenGoldDrop;
		}
	}
	public void Update()
	{
		if (!isServer || !isDropping) 
		{
			return;
		}
		if (Time.time > lastGoldDrop) 
		{
			lastGoldDrop = Time.time + timeBetweenGoldDrop;
			ActualGold += goldPerDrop;
		}
	}

	public void GoldActualize(int goldygold)
	{
		ActualGold = goldygold;
		if (isLocalPlayer) 
		{
			if (goldDisplay == null) 
			{
				goldDisplay = GameObject.Find ("PlayerGold").GetComponent<Text> ();

			}
			goldDisplay.text = ActualGold.ToString ();
		}
	}


   ////////////////// Fonction côté serveur //////////////////////
   /// <summary>
   /// Actualise les golds reçu des ennmies du mob tués par le killer
   /// </summary>
   /// <param name="ennemi"></param>
   /// <param name="killer"></param>
    public void receiveGoldFromEnnemy(EnnemyIGManager ennemi, GameObject killer)
    {
        int goldGiven;
        int goldKiller;

        if (ennemi.myEnemies.Count > 0)
        {
            double goldTemp = ennemi.goldGiven / (ennemi.myEnemies.Count * 0.9);
            goldGiven = (int) Math.Ceiling(goldTemp);
            goldTemp = ennemi.goldGiven / (ennemi.myEnemies.Count * 0.1);
            goldKiller = (int)Math.Ceiling(goldTemp);

            for (int i = 0; i < ennemi.myEnemies.Count; i++)
            {
                GameObject ennemiKiller = ennemi.myEnemies[i];
                if (ennemiKiller == killer)
                {
                    ennemiKiller.GetComponent<PlayerGoldScript>().GetGold(goldGiven + goldKiller);
                    TargetSendGoldMessage(ennemiKiller.GetComponent<NetworkIdentity>().connectionToClient, ennemi.gameObject, goldGiven + goldKiller);
                }
                else
                {
                    ennemiKiller.GetComponent<PlayerGoldScript>().GetGold(goldGiven);
                    TargetSendGoldMessage(ennemiKiller.GetComponent<NetworkIdentity>().connectionToClient, ennemi.gameObject, goldGiven);
                }

            }
            ennemi.myEnemies.Clear();
        }
        
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ennemi"></param>
    public void receiveGoldFromEnnemy(EnnemyIGManager ennemi)
    {
        int gold;
        if (ennemi.myEnemies.Count > 0)
        {
            double goldTemp = ennemi.goldGiven / (ennemi.myEnemies.Count * 0.9);
            gold = (int)Math.Ceiling(goldTemp);

            for (int i = 0; i < ennemi.myEnemies.Count; i++)
            {
                GameObject ennemiKiller = ennemi.myEnemies[i];
                ennemiKiller.GetComponent<PlayerGoldScript>().GetGold(gold);
                TargetSendGoldMessage(ennemiKiller.GetComponent<NetworkIdentity>().connectionToClient, ennemi.gameObject, gold);
            }
            ennemi.myEnemies.Clear();
        }
        
    }



    [TargetRpc]
    public void TargetSendGoldMessage(NetworkConnection target, GameObject mob,int gold)
    {
        GameObject go = ObjectPooling.Instance.GetPooledObject(goldCanvasPrefab.tag);
        //go.transform.SetParent(mob.transform, false);
        //go.transform.localScale = new Vector3(1, 1, 1);
        go.transform.position = mob.transform.position;
        //go.transform.SetParent(null, false);
        go.transform.GetChild(0).GetComponentInChildren<Text>().text = "+" + gold;
        go.SetActive(true);
        go.GetComponentInChildren<Animator>().enabled = true;
        go.GetComponentInChildren<Canvas>().enabled = true;
        go.GetComponentInChildren<InactivateAnimatorCanvas>().inactiveWithTime();
        go.GetComponentInChildren<InactivateByTime>().InactivateWithlifeTime();
    }
}
