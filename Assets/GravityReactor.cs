using UnityEngine;
using System.Collections;

public class GravityReactor : MonoBehaviour {

	void FixedUpdate(){
		Rigidbody r = transform.GetComponent<Rigidbody> ();
		if(r!=null)
		r.AddForce (
			Gravities.getGravityAtPosition (r.transform, r.mass)
		);
	}
}
