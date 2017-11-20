using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class AutoCastCatapulteOnPlayers : NetworkBehaviour
{
    public bool teamOfCataIs1;
    public int spellDmg;
    public GameObject spellPrefab;
    private float rotSpeed = 2f;
    float previousFire;
    public float timeBetweenFire = 10f;
    public Animator animController;
    public AudioClip firingCataSnd;
    public AudioClip rotatingCataSnd;
    [SyncVar(hook = "SyncCataFiring")]
    public bool isFiring;
    [SyncVar]
    GameObject targetObj;
    bool firstShot = true;
    // Use this for initialization
    void Start()
    {
        if (GetComponent<CaptureThePoint>().canBeOwnedBy == CaptureThePoint.PointOwner.team2)
        {
            teamOfCataIs1 = false;
        }
    }
    [ServerCallback]
    void Update()
    {
        if (GameManager.instanceGM.Days == 1)
        {
            return;
        }
        if (Time.time > previousFire + timeBetweenFire)
        {
            previousFire = Time.time + Random.Range(0f, 20f);
            if (firstShot)
            {
                firstShot = false;
                return;
            }
            if (GetComponent<CaptureThePoint>().belongsTo == CaptureThePoint.PointOwner.neutral)
            {
                if (GetComponent<CaptureThePoint>().enemiesIn.Count > 0)
                {
                    FireOnAPlayer();
                }
            }
            if (GetComponent<CaptureThePoint>().belongsTo == GetComponent<CaptureThePoint>().canBeOwnedBy)
            {
                if (GetComponent<CaptureThePoint>().enemiesIn.Count <= 0)
                {
                    FireOnAnEnemyPlayer();
                }
            }
        }
    }
    public void LateUpdate()
    {
        if (isFiring)
        {
            Quaternion targetRot = Quaternion.LookRotation(targetObj.transform.position - transform.position);
            float str = Mathf.Min(rotSpeed * Time.deltaTime, 1);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, str);
            //			Debug.Log (str.ToString());
        }
    }
    public void FireOnAPlayer()
    {
        int iValue = Mathf.CeilToInt(spellDmg * 1.05f);
        spellDmg = iValue+1;
        //Dans tous les cas : la puissance augmente a chaque tir, que yé une cible ou pas, que le tir soit effectué ou pas.
        if (teamOfCataIs1)
        {
            int x = Random.Range(0, GameManager.instanceGM.team1ID.Count);
            targetObj = ClientScene.FindLocalObject(GameManager.instanceGM.team1ID[x]);
            if (targetObj.GetComponent<PlayerIGManager>().isDead)
            {
                return;
            }
            Vector3 targPos = targetObj.transform.position;
            FireOnTarget(targPos);
        }
        else
        {
            if (GameManager.instanceGM.soloGame)
            {
                return;
            }
            int x = Random.Range(0, GameManager.instanceGM.team2ID.Count);
            targetObj = ClientScene.FindLocalObject(GameManager.instanceGM.team2ID[x]);
            if (targetObj.GetComponent<PlayerIGManager>().isDead)
            {
                return;
            }
            Vector3 targPos = targetObj.transform.position;
            FireOnTarget(targPos);
        }
    }
    public void FireOnAnEnemyPlayer()
    {
        int iValue = Mathf.CeilToInt(spellDmg * 1.05f);
        spellDmg = iValue+1;
        //Dans tous les cas : la puissance augmente a chaque tir, que yé une cible ou pas, que le tir soit effectué ou pas.
        if (teamOfCataIs1)
        {
            if (GameManager.instanceGM.soloGame)
            {
                return;
            }
            int x = Random.Range(0, GameManager.instanceGM.team2ID.Count);
            targetObj = ClientScene.FindLocalObject(GameManager.instanceGM.team2ID[x]);
            if (targetObj.GetComponent<PlayerIGManager>().isDead)
            {
                return;
            }
            Vector3 targPos = targetObj.transform.position;
            FireOnTarget(targPos);
        }
        else
        {
            int x = Random.Range(0, GameManager.instanceGM.team1ID.Count);
            targetObj = ClientScene.FindLocalObject(GameManager.instanceGM.team1ID[x]);
            if (targetObj.GetComponent<PlayerIGManager>().isDead)
            {
                return;
            }
            Vector3 targPos = targetObj.transform.position;
            FireOnTarget(targPos);
        }
    }
    public void FireOnTarget(Vector3 pos)
    {
        StartCoroutine(FiringCycle());
        GameObject go = Instantiate(spellPrefab, pos, Quaternion.identity);
        //		go.GetComponent<SpellCatapulteAuto>().caster = gameObject;
        go.GetComponent<SpellCatapulteAuto>().spellDamage = spellDmg;
        NetworkServer.Spawn(go);
    }
    public void SyncCataFiring(bool fire)
    {
        isFiring = fire;
        StartCoroutine(FireProcedure());
        //jouer ici les sons et animations lorsque la catapulte tire.
    }
    IEnumerator FiringCycle()
    {
        isFiring = true;
        yield return new WaitForSeconds(3f);
        isFiring = false;
    }
    IEnumerator FireProcedure()
    {
        GetComponent<AudioSource>().PlayOneShot(rotatingCataSnd);
        animController.enabled = true;
        yield return new WaitForSeconds(2f);
        GetComponent<AudioSource>().PlayOneShot(firingCataSnd);
        yield return new WaitForSeconds(3f);
        animController.enabled = false;
    }
}