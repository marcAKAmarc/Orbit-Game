using UnityEngine;
using System.Collections;

public class ExplodeOnImpact : MonoBehaviour {

    Vector3 prevVelocity = Vector3.zero;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        var r = transform.GetComponent<Rigidbody>();
        if((r.velocity - prevVelocity).magnitude > 10f)
        {
            Application.LoadLevel("someScene");
        }
        prevVelocity = r.velocity;
	}
}
