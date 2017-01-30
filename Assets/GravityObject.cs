using UnityEngine;
using System.Collections;

public class GravityObject{ 
	//g = (G+(obj1mass*obj2mass))/(radius*radius)
	public float mass;
	public float radius;
	public Vector3 position;
	
	public GravityObject (float mass, float radius, Vector3 position){
		this.mass = mass;
		this.radius = radius;
		this.position = position;
		Gravities.collection.Add (this);
	}
}
