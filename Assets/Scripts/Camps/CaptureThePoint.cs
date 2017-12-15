using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HyperLuminalGames;
using UnityEngine.Networking;

public class CaptureThePoint : NetworkBehaviour
{
    public enum PointOwner
    {
        team1,
        team2,
        neutral
    }

    [Header("TypeOfPoint")]
    public bool isOutpost = true;
    public bool isCatapulte;

    [SyncVar(hook = "ChangeOwner")] public PointOwner belongsTo;
    public PointOwner canBeOwnedBy = PointOwner.team1;
    public List<GameObject> playersIn;
    public List<GameObject> enemiesIn;
    public float timeToCapture;
    private float timeCaptureStart;
    private float initialTimeToCapt;
    public AudioClip Capture;
    [SyncVar(hook = "SyncOutpostTimer")] int tmpTime;
    public bool haveGivenTip0;
    public string tip0Fr = "Tuez tous les ennemis sur l'avant-poste pour le capturer.";
    public string tip0En = "Kill all enemies on the outpost area to be able to capture it.";

    public bool haveGivenTip;
    public string tipFr = "Capturez l'avant-poste pour avoir accés au magasin associé et pouvoir augmenter la force de votre boss journalier.";
    public string tipEn = "Capture the outpost to get acess to his shop and be able to increase the strenght of your daily boss.";

    public string alertMessSentPositiveEng = "Our outpost has been captured.";
    public string alertMessSentNegativeEng = "The enemy outpost has been captured.";
    public string alertMessSentPositiveFr = "Notre avant-poste a été capturé.";
    public string alertMessSentNegativeFr = "L'avant-poste ennemi a été capturé.";

    public string alertMessSentPositiveLossEng = "The enemy outpost has been lost!";
    public string alertMessSentNegativeLossEng = "Our outpost has been lost.";
    public string alertMessSentPositiveLossFr = "L'avant-poste ennemi est perdu.";
    public string alertMessSentNegativeLossFr = "Notre avant-poste est perdu.";

    public Color ownedColor;
    public Color notOwnedColor;
    // Use this for initialization
    void Start()
    {
        if (isOutpost)
        {
            GetComponentInChildren<ShopScript>().isAccessible = false;
        }
        initialTimeToCapt = timeToCapture;

    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == Layers.Player && other.gameObject.tag == "Player")
        {
            if (isServer)
            {
                playersIn.Add(other.gameObject);
            }
            if (other.GetComponent<NetworkIdentity>().isLocalPlayer)
            {
                if (!haveGivenTip0)
                {
                    GameManager.instanceGM.ShowAGameTip(tip0En);
                    if (PlayerPrefs.GetString("LANGAGE") == "Fr")
                    {
                        GameManager.instanceGM.ShowAGameTip(tip0Fr);

                    }
                    haveGivenTip0 = true;
                }
            }
        }
		if (isServer && other.gameObject.layer == Layers.Ennemies )
        {
            enemiesIn.Add(other.gameObject);
        }

    }
    [ServerCallback]
    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == Layers.Player)
        {
            playersIn.Remove(other.gameObject);
            if (playersIn.Count == 0 && belongsTo == PointOwner.neutral)
            {
                stopCapture();
            }
        }
        if (other.gameObject.layer == Layers.Ennemies)
        {
            enemiesIn.Remove(other.gameObject);
            if (enemiesIn.Count == 0 && belongsTo != PointOwner.neutral)
            {
                stopCapture();
            }
        }
    }


    public virtual void stopCapture()
    {
        timeToCapture = initialTimeToCapt;
    }


    [ServerCallback]
    public void OnTriggerStay(Collider other)
    {
		if (timeToCapture < initialTimeToCapt && timeToCapture > 0f)
        {
            //a opti : ca run 2 fois sur le serveur et le changement de variable / recalcul est trop fréquent mais bon...ya pas 1000 fois le script.
            tmpTime = (int)timeToCapture;
            GetComponent<Location>().Display_2_Text = tmpTime.ToString();
        }
        else
        {
            GetComponent<Location>().Display_2_Text = "";
        }
        switch (belongsTo)
        {
            case PointOwner.neutral:
                playerCaptureThePoint(other.gameObject.layer);
                break;
            case PointOwner.team1:
                ennemyCaptureThePoint(other.gameObject.layer);
                break;
            case PointOwner.team2:
                ennemyCaptureThePoint(other.gameObject.layer);

                break;
            default:
                break;
        }

    }

    public void ennemyCaptureThePoint(int layer)
    {
        if (layer == Layers.Ennemies)
        {
            if (playersIn.Count > 0)
            {
                playersIn.ForEach(CheckIfPlayersAreAlive);
            }
            if (playersIn.Count == 0)
            {
                captureThePoint(PointOwner.neutral);
            }

        }
    }

    public void playerCaptureThePoint(int layer)
    {
        if (enemiesIn.Count > 0)
        {
            enemiesIn.RemoveAll((GameObject obj) => obj == null);
        }
        if (layer == Layers.Player && enemiesIn.Count == 0)
        {
            captureThePoint(canBeOwnedBy);
        }
    }

    public virtual void captureThePoint(PointOwner target)
    {
        timeToCapture -= Time.fixedUnscaledDeltaTime;
        if (timeToCapture <= 0f)
        {
            timeToCapture = initialTimeToCapt;
            belongsTo = target;
        }
    }

    public void CheckIfPlayersAreAlive(GameObject player)
    {
        if (player.GetComponent<PlayerIGManager>().isDead)
        {
            playersIn.Remove(player);
        }

    }

    public void ChangeOwner(PointOwner newOwner)
    {
        GameManager.instanceGM.ActualizeLocSystem();
        belongsTo = newOwner;
        if (canBeOwnedBy == newOwner)
        {
            GetComponent<AudioSource>().PlayOneShot(Capture);
            GetComponent<Location>().IconColour = Color.green;

            if (isOutpost)
           {
                GetComponentInChildren<ShopScript>().isAccessible = true;
                transform.GetChild(0).GetComponent<Location>().enabled = true;
            }
//            if (isCatapulte)
//            {
//
//                GetComponent<CatapulteObjectScript>().userOfCata = playersIn[0];
//
//                GetComponent<CatapulteObjectScript>().ActivatePlayerBtn();
//            }
            if (GameManager.instanceGM.isTeam1)
            {
                if (belongsTo == PointOwner.team1)
                {
                    if (PlayerPrefs.GetString("LANGAGE") == "Fr")
                    {
                        GameManager.instanceGM.messageManager.SendAnAlertMess(alertMessSentPositiveFr, Color.green);
                        return;
                    }
                    GameManager.instanceGM.messageManager.SendAnAlertMess(alertMessSentPositiveEng, Color.green);
                }
                else
                {
                    if (PlayerPrefs.GetString("LANGAGE") == "Fr")
                    {
                        GameManager.instanceGM.messageManager.SendAnAlertMess(alertMessSentNegativeFr, Color.red);
                        return;
                    }
                    GameManager.instanceGM.messageManager.SendAnAlertMess(alertMessSentNegativeEng, Color.red);
                }
            }
            else
            {
                if (belongsTo == PointOwner.team1)
                {
                    if (PlayerPrefs.GetString("LANGAGE") == "Fr")
                    {
                        GameManager.instanceGM.messageManager.SendAnAlertMess(alertMessSentNegativeFr, Color.red);
                        return;
                    }
                    GameManager.instanceGM.messageManager.SendAnAlertMess(alertMessSentNegativeFr, Color.red);
                }
                else
                {
                    if (PlayerPrefs.GetString("LANGAGE") == "Fr")
                    {
                        GameManager.instanceGM.messageManager.SendAnAlertMess(alertMessSentPositiveFr, Color.green);
                        return;
                    }
                    GameManager.instanceGM.messageManager.SendAnAlertMess(alertMessSentPositiveEng, Color.green);
                }
            }
        }
        else
        {
            GetComponent<Location>().IconColour = Color.yellow;
//            if (isCatapulte)
//            {
//                GetComponent<CatapulteObjectScript>().DesactivatePlayerBtn();
//            }
            if (isOutpost)
            {

                transform.GetChild(0).GetComponent<Location>().enabled = false;
                GetComponentInChildren<ShopScript>().isAccessible = false;
                GetComponentInChildren<ShopScript>().CloseYourMenu();
            }

            if (GameManager.instanceGM.isTeam1)
            {
                if (belongsTo == PointOwner.team1)
                {
                    if (PlayerPrefs.GetString("LANGAGE") == "Fr")
                    {
                        GameManager.instanceGM.messageManager.SendAnAlertMess(alertMessSentPositiveLossFr, Color.green);
                        return;
                    }
                    GameManager.instanceGM.messageManager.SendAnAlertMess(alertMessSentPositiveLossEng, Color.green);
                }
                else
                {
                    if (PlayerPrefs.GetString("LANGAGE") == "Fr")
                    {
                        GameManager.instanceGM.messageManager.SendAnAlertMess(alertMessSentNegativeLossFr, Color.red);
                        return;
                    }
                    GameManager.instanceGM.messageManager.SendAnAlertMess(alertMessSentNegativeLossEng, Color.red);
                }
            }
            else
            {
                if (belongsTo == PointOwner.team1)
                {
                    if (PlayerPrefs.GetString("LANGAGE") == "Fr")
                    {
                        GameManager.instanceGM.messageManager.SendAnAlertMess(alertMessSentNegativeLossFr, Color.red);
                        return;
                    }
                    GameManager.instanceGM.messageManager.SendAnAlertMess(alertMessSentNegativeLossEng, Color.red);
                }
                else
                {
                    if (PlayerPrefs.GetString("LANGAGE") == "Fr")
                    {
                        GameManager.instanceGM.messageManager.SendAnAlertMess(alertMessSentPositiveLossFr, Color.green);
                        return;
                    }
                    GameManager.instanceGM.messageManager.SendAnAlertMess(alertMessSentPositiveLossEng, Color.green);
                }
            }
        }
    }

    public void SyncOutpostTimer(int t)
    {
		tmpTime = t;

        if (!haveGivenTip)
        {
            GameManager.instanceGM.ShowAGameTip(tipEn);
            if (PlayerPrefs.GetString("LANGAGE") == "Fr")
            {
                GameManager.instanceGM.ShowAGameTip(tipFr);

            }
            haveGivenTip = true;
        }
        if (!isServer)
        {
			if (t < initialTimeToCapt && t > 0f)
            {
                GetComponent<Location>().Display_2_Text = tmpTime.ToString();
            }
            else
            {
                GetComponent<Location>().Display_2_Text = "";
            }
        }
    }

}
