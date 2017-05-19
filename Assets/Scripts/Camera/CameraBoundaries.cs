using UnityEngine;

[System.Serializable]
public class CameraBoundaries 
{
    // gameObject symbolisant les limites SUD et EST de la camera
    public GameObject WardSudEst;
    // gameObject symbolisant les limites NORD et OUEST de la camera
    public GameObject WardNordOuest;

    public float getSud()
    {
        return WardSudEst.transform.position.x;
    }

    public float getEst()
    {
        return WardSudEst.transform.position.z;
    }

    public float getOuest()
    {
        return WardNordOuest.transform.position.z;
    }

    public float getNord()
    {
        return WardNordOuest.transform.position.x;
    }
}