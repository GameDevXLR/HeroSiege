using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerLevelUpManager : MonoBehaviour 
{

	//ce script gere les options de lvlup d'un joueur.
	// il dit au joueur quoi faire en cas de up
	public int playerLvl=1; // le niveau du joueur. en local.
	public int specPts;//points de spécialisation / un de base.
	public int ultiSpecPts; // pts pour lvl up l'ulti!
	public GameObject spell1LvlUpBtn;
	public int spell1Lvl = 1;
	public GameObject spell2LvlUpBtn;
	public int spell2Lvl = 1;
	public GameObject spellUltLvlUpBtn;
	public int spellUltLvl = 1;
	public GameObject specPlusBtn;
	public int specPlusLvl = 0;

	// Use this for initialization
	void Start () 
	{
		
		spell1LvlUpBtn = GameObject.Find ("Spell1LvlUpBtn");
		spell2LvlUpBtn = GameObject.Find ("Spell2LvlUpBtn");
		spellUltLvlUpBtn = GameObject.Find ("Spell3LvlUpBtn");
		//rajouter ici les futurs sorts a faire up.
		specPlusBtn = GameObject.Find ("StatPlusBtn");
//		Invoke ("AvoidEarlyUltiUp", 0.3f);

	}
	public void AvoidEarlyUltiUp()
	{

			spellUltLvlUpBtn.SetActive (false);

	}
	public void GetAlevel()
	{
		specPts++;
		playerLvl++;
		if (playerLvl == 2) 
		{
			GameManager.instanceGM.ShowAGameTip ("When you level up, you get a specialization point that you can spend to increase the power of one of your ability, or up your overall basic statistics.");
			if (PlayerPrefs.GetString ("LANGAGE") == "Fr") 
			{
				GameManager.instanceGM.ShowAGameTip ("Quand vous gagnez un niveau, Vous obtenez un point de spécialisation que vous pouvez dépenser pour augmenter la puissance de l'une de vos compétences, ou pour augmenter vos statistiques de base.");

			}
		}
//		GetComponent<PlayerManager> ().GetALevel ();
		if (playerLvl == 3 || playerLvl == 6 || playerLvl == 9 || playerLvl == 12|| playerLvl == 15|| playerLvl == 18|| playerLvl == 21|| playerLvl == 24|| playerLvl == 27|| playerLvl == 30) 
		{
			ultiSpecPts++;
		}
		specPlusBtn.SetActive (true);
		if (spell1Lvl < 10) {
			spell1LvlUpBtn.SetActive (true);
//			spell1LvlUpBtn.GetComponent<Animator> ().Play ("BtnCompPts");
		}
		if (spell2Lvl < 10) {

			spell2LvlUpBtn.SetActive (true);
		}
			//		StartCoroutine (LevelUpProcess ());
		if (ultiSpecPts > 0 && spellUltLvl<10) 
		{
			spellUltLvlUpBtn.SetActive (true);
		}
	}

//	IEnumerator LevelUpProcess()
//	{
//		yield return new WaitForSeconds (0.66f);
//	}

	public void LooseASpecPt(int spell)
	{
        if (specPts > 0)
        {
            if (spell == 4)
            {
                specPlusLvl++;
                if (specPlusLvl == 1)
                {
                    GameManager.instanceGM.ShowAGameTip("Did you know that one of the 3 stat bonus given by this passive is doubled based on your hero type : Adc/Tank/Mage.");
                    if (PlayerPrefs.GetString("LANGAGE") == "Fr")
                    {
                        GameManager.instanceGM.ShowAGameTip("Saviez vous que l'un des trois bonus donné par ce passif est doublé en fonction du type de votre héro: Adc/Tank/Mage.");

                    }
                }
            }
            if (spell == 3)
            {
                ultiSpecPts--;
                spellUltLvl++;
                if (spellUltLvl == 2)
                {
                    GameManager.instanceGM.ShowAGameTip("Did you know that you can up your ultimate only once every 3 levels. And that all of your abilities can only be upgraded 10 times, except for the Statistic bonus.");
                    if (PlayerPrefs.GetString("LANGAGE") == "Fr")
                    {
                        GameManager.instanceGM.ShowAGameTip("Saviez vous que vous pouvez augmenter votre Ultime une fois tous les trois niveaux? Et que vos compétences ne peuvent être augmenter que 10 fois, sauf la compétence de bonus de stats.");

                    }
                }
                if (ultiSpecPts < 0)
                {
                    ultiSpecPts = 0;
                }
                if (ultiSpecPts == 0)
                {
                    spellUltLvlUpBtn.SetActive(false);
                }
            }
            if (spell == 1)
            {
                spell1Lvl++;
            }
            if (spell == 2)
            {
                spell2Lvl++;
            }
        }
        specPts--;
        if (specPts <= 0)
        {
            specPts = 0;
            specPlusBtn.SetActive(false);
            spell1LvlUpBtn.SetActive(false);
            spell2LvlUpBtn.SetActive(false);
            spellUltLvlUpBtn.SetActive(false);

        }
    }
}
