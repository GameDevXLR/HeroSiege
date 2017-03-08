using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
		GetComponent<NavMeshAgent> ().Stop ();
		GetComponent<Rigidbody> ().velocity = Vector3.zero;
		//ajouter la désactivation de l'autoA;
		yield return new WaitForSeconds (CCTime);
		//réactiver l'autoA;
		GetComponent<NavMeshAgent> ().Resume ();
	}

	public void MakeHimRoot(float rootDuration)
	{
		StartCoroutine (RootProcedure(rootDuration));
	}
	IEnumerator RootProcedure (float rootTime)
	{
		GetComponent<NavMeshAgent> ().Stop ();
		GetComponent<Rigidbody> ().velocity = Vector3.zero;
		yield return new WaitForSeconds (rootTime);
		GetComponent<NavMeshAgent> ().Resume ();
	}

}
