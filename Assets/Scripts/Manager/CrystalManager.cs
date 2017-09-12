using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CrystalManager : NetworkBehaviour
{

    [SerializeField]
    [SyncVar] private int storageCapacitie = 100;

    [SerializeField]
    [SyncVar(hook = "actualizeEnergie")] private int currentEnergie = 100;


    public int secondToWait = 10;

    [SyncVar(hook = "actualizeDecreaseBool")] public bool decrease = false;

    public List<EnnemyIGManager> listEnnemy;
    public JungleCampSpawnManager jungleCamp;


    public void initialize()
    {
        foreach(GameObject obj in jungleCamp.jungCampMinion)
        {
            listEnnemy.Add(obj.GetComponent<EnnemyIGManager>());
        }
        foreach (GameObject obj in jungleCamp.jungCampBoss)
        {
            listEnnemy.Add(obj.GetComponent<EnnemyIGManager>());
        }
    }

    public void actualizeEnergie(int energie)
    {
        currentEnergie = energie;
        if (currentEnergie <= 0)
        {
            decrease = false;
            currentEnergie = 0;
        }
            
        else if (currentEnergie > storageCapacitie)
            currentEnergie = storageCapacitie;
    }

    public void actualizeDecreaseBool(bool decrease)
    {
        if (decrease)
            launchDecrease();
    }

    public void launchDecrease()
    {
        decrease = true;
        StartCoroutine(decreaseStock());

    }

    IEnumerator decreaseStock()
    {
        if (UtilsArray.allEnnemyDie(listEnnemy))
        {
            decrease = false;
        }
        while (decrease && currentEnergie != 0)
        {
            currentEnergie--;
            yield return new WaitForSeconds(secondToWait);
        }
    }

    /// <summary>
    /// Allow to get some energie. If too much we take the energie which rest
    /// </summary>
    /// <param name="sum"></param>
    /// <returns></returns>
    public int getEnergie(int sum)
    {
        if (currentEnergie != 0)
        {
            if (currentEnergie - sum >= 0)
                return sum;
            else
            {
                return currentEnergie;
            }
        }
        return 0;
    }

    /// <summary>
    /// function too replenish in energie. if no parameters or parameters == -1 the storage if replenish in full
    /// </summary>
    /// <param name="energie"> integer </param>
    public void replenish(int energie = -1)
    {
        if (energie == -1)
            currentEnergie = storageCapacitie;
        else
        {
            currentEnergie += energie;
        }
    }

    public void increaseStorage(int increase)
    {
        storageCapacitie += increase;
    }
}
