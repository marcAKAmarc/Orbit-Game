using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Gravities{
	public static List<GravityObject> collection = new List<GravityObject>();
	
	public static Vector3 getGravityAtPosition(Transform t ,float mass){
		Vector3 result = Vector3.zero;
		foreach (GravityObject g in collection) {

			 float gravAmt = (9.8f+(mass*g.mass)) / Mathf.Pow(t.position.sqrMagnitude - g.position.sqrMagnitude,2);
			result = (g.position - t.transform.position)*gravAmt;
		}
		return result;
	}

	public static Vector3 getGravityAtPositionCheap(Transform t ,float mass){
		Vector3 result = Vector3.zero;
		foreach (GravityObject g in collection) {
			
			float gravAmt = (9.8f+(mass*g.mass)) / Mathf.Pow(t.position.magnitude - g.position.magnitude,2);
			result = (g.position - t.transform.position)*gravAmt;
		}
		return result;
	}
}
