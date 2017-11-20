using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EnergyManagerServer : NetworkBehaviour, ICanalisage {

    [SyncVar]
    public int energy = 0;
    [SyncVar]
    public int energyTampon = 0;
    public float distanceAccess = 15;
    public CrystalManager crystal;
    

	public void Request(GameObject crystal, int energy)
    {
        this.crystal = crystal.GetComponent<CrystalManager>();
        if (this.crystal.canTake() && canAccess(crystal))
        {
            launch(3, crystal.transform.position);
            CmdSendRequest(crystal, energy);
        }
    }

    [Command]
    public void CmdSendRequest(GameObject crystal, int energy)
    {
        energyTampon += crystal.GetComponent<CrystalManager>().getEnergie(energy);
        if (energyTampon == 0)
        {
            TargetFaillure(gameObject.GetComponent<NetworkIdentity>().connectionToClient);
        }
    }

    [TargetRpc]
    public void TargetFaillure(NetworkConnection target)
    {
        interruption();
    }

    public bool canAccess(GameObject crystal)
    {
        return Vector3.Distance(crystal.transform.position, gameObject.transform.position) < distanceAccess;
    }
    


    /// Interface ICanalisage ////

    public void launch(float time, Vector3 direction)
    {
        gameObject.GetComponent<PlayerCanalisage>().LaunchCanalisage(this, time, direction);
    }

    public void interruption()
    {
        
        crystal.replenish(energyTampon);
        energyTampon = 0;
    }

    public void success()
    {
        energy += energyTampon;
        energyTampon = 0;
    }
}
