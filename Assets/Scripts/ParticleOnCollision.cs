using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleOnCollision : MonoBehaviour {

	void OnParticleCollision (GameObject other)
	{
		if (other.layer == 9)
		{
			Debug.Log ("truite");
		}
//		Rigidbody body = other.GetComponent<Rigidbody> ();
//		if (body) {
//			Vector3 direction = other.transform.position - transform.position;
//			direction = direction.normalized;
//			body.AddForce (direction * 5);
//		}
	}
}
