using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EnergyManagerServer : NetworkBehaviour {

    [SyncVar]
    public int energy = 0;

	public void Request(GameObject crystal, int energy)
    {
        if(!crystal.GetComponent<CrystalManager>().isEmpty())
        {
            CmdSendRequest(crystal, energy);
        }
    }

    [Command]
    public void CmdSendRequest(GameObject crystal, int energy)
    {
        this.energy += crystal.GetComponent<CrystalManager>().getEnergie(energy);
    }
}
