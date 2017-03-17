using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PNJManager : NetworkBehaviour 
{
	//pour le moment : contient juste une liste de point de "patrouille" pour les pnj alliés.
	//il garde aussi en compte le nombre de pnj spawn et est sync sur le réseau.
	public Transform[] campGuardPositions;
	public int GuardNbr;
}
