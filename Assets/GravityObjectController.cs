using UnityEngine;
using System.Collections;

public class GravityObjectController : MonoBehaviour {

	public float Mass;
	public float scaleSpoofAddition;

	void Start(){
		createGravityObject ();
	}

	private void createGravityObject(){
		 new GravityObject (Mass, transform.localScale.x + scaleSpoofAddition, transform.position);
	}
}
