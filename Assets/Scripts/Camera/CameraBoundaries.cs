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
        Debug.Log( "Sud : " + WardSudEst.transform.position.y);
        return WardSudEst.transform.position.y;
    }

    public float getEst()
    {
        Debug.Log("Est : " + WardSudEst.transform.position.x);
        return WardSudEst.transform.position.x;
    }

    public float getOuest()
    {
        Debug.Log("Ouest : " + WardNordOuest.transform.position.x);
        return WardNordOuest.transform.position.x;
    }

    public float getNord()
    {
        Debug.Log("Nord : " + WardNordOuest.transform.position.y);
        return WardNordOuest.transform.position.y;
    }
}