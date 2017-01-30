using UnityEngine;
using System.Collections;

public class Aerodynamics : MonoBehaviour {
	public bool freezeX, freezeY, freezeZ;
	// Use this for initialization
	void Start () {
	
	}
	//what bullshit.
	// Update is called once per frame
	void FixedUpdate () {
		Rigidbody r = transform.GetComponent<Rigidbody> ();

		Quaternion goalRotation = Quaternion.LookRotation (r.velocity, transform.up);
		Vector3 goalVectorRelative = Quaternion.Inverse (transform.rotation) * goalRotation * Vector3.up;
		Vector3 flattenedVectorRelative;
		flattenedVectorRelative.x = freezeX ? 0.0f : goalVectorRelative.x;
		flattenedVectorRelative.y = freezeY ? 0.0f : goalVectorRelative.y;
		flattenedVectorRelative.z = freezeZ ? 0.0f : goalVectorRelative.z;
		Quaternion AbsFlattenedRotation = Quaternion.LookRotation(transform.rotation * goalVectorRelative, transform.up);
		float force = 1.0f - (1.0f / ((r.velocity.sqrMagnitude / 8.0f) + 1.0f));
		//Quaternion toRotation = Quaternion.Slerp (transform.rotation, AbsFlattenedRotation, 1.0f - (1.0f / ((r.velocity.sqrMagnitude / 800.0f) + 1.0f)));
		//r.MoveRotation (toRotation);
		r.AddForceAtPosition( AbsFlattenedRotation*Vector3.up* force, transform.position+Vector3.Cross (transform.forward,transform.up));
	}
}
