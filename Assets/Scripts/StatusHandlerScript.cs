using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusHandlerScript : MonoBehaviour {

	//ce script gere les différents statuts d'un personnage : CC  / root / saignement / slow etc...

	public bool underCC;
	private GenericLifeScript lifeScript;
	private ennemiMover MoveScript;
	// ajouter le script d'autoAttack.

	// Use this for initialization
	void Start () {
		lifeScript = GetComponent<GenericLifeScript> ();
		MoveScript = GetComponent<ennemiMover> ();
		
	}
	public void MakeHimCC(float CCDuration)
	{
		StartCoroutine (CCprocedure ( CCDuration));
	}
	IEnumerator CCprocedure ( float CCTime)
	{
		MoveScript.enabled = false;
		//ajouter la désactivation de l'autoA;
		yield return new WaitForSeconds (CCTime);
		//réactiver l'autoA;
		MoveScript.enabled = true;
	}

	public void MakeHimRoot(float rootDuration)
	{
		StartCoroutine (RootProcedure(rootDuration));
	}
	IEnumerator RootProcedure (float rootTime)
	{
		MoveScript.enabled = false;
		yield return new WaitForSeconds (rootTime);
		MoveScript.enabled = true;	
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
}
