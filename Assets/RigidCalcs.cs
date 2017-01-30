using UnityEngine;
using System.Collections;

public class RigidCalcs {

	public static void freezeLocalRotation(Rigidbody r){
		Vector3 localangularvelocity = r.transform.InverseTransformDirection(r.angularVelocity).normalized* r.angularVelocity.magnitude;
		localangularvelocity.x = 0;
		localangularvelocity.z = 0;
		localangularvelocity.y = 0;
		
		r.angularVelocity = r.transform.TransformDirection(localangularvelocity);
	}
}
