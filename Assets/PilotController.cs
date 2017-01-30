using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class ShipControls
{
    public static KeyCode VelocityPlus = KeyCode.Q;
    public static KeyCode VelocityMinus = KeyCode.W;
    public static KeyCode CameraPlus = KeyCode.A;
    public static KeyCode CameraMinus = KeyCode.S;
    public static KeyCode GravityPlus = KeyCode.Z;
    public static KeyCode GravityMinus = KeyCode.X;

    public static KeyCode Thrust = KeyCode.Space;



    public static List<KeyCode> Buttons = new List<KeyCode> { VelocityPlus, VelocityMinus, CameraPlus, CameraMinus, GravityPlus, GravityMinus, Thrust};

    public static int NumberActivated()
    {
        int activated = 0;
        foreach (var button in Buttons)
            if (Input.GetKey(button))
                activated += 1;
        return activated;
    }
}

public class PilotController : MonoBehaviour {

	public List<Action> ThrustEngines = new List<Action>();
	public List<Action> BrakeEngines = new List<Action> ();
    public List<Action> DamperEngines = new List<Action>();
    public Rigidbody shipMainRigid;
	public Camera cam;
	void Start(){
		updateThrustEngines ();
		updateBrakeEngines ();
	}

	void Update(){

		ControllerDelegate.setStick ();

		foreach (Action a in ThrustEngines) {
			a.pilotEngaged = false;
            a.actionForce = 0;
		}

		int selectionNumber = Mathf.FloorToInt (ThrustEngines.Count / 4.0f);
		if (selectionNumber == 0)
			selectionNumber = 1;
        selectionNumber = 1;
		List<Action> RelevantThrustEngines = new List<Action> ();
        List<Action> RelevantTurnEngines = new List<Action>();
        List<Action> RelevantDamperEngines = new List<Action>();
        List<Action> RelevantBrakeEngines = new List<Action> ();
        Vector3 goalDirection = Vector3.zero;


		if (Input.GetKey (ShipControls.Thrust)) {
			RelevantThrustEngines.AddRange (
				getActionsAll(ThrustEngines)
			);
		}


		if (Input.GetKey(ShipControls.CameraPlus)  || ControllerDelegate.isUp () ) {

			RelevantTurnEngines.AddRange ( 
				getActionsClosestToCamera(ThrustEngines, selectionNumber)
			);
            RelevantDamperEngines.AddRange(
                getActionsFurthestFromCamera(ThrustEngines, selectionNumber)
            );
            goalDirection += getDirectionAwayFromCamera().normalized;
					
		}

		if (Input.GetKey(ShipControls.CameraMinus) || ControllerDelegate.isDown()) {
			RelevantTurnEngines.AddRange ( 
				getActionsFurthestFromCamera (ThrustEngines, selectionNumber)
			);
            RelevantDamperEngines.AddRange(
               getActionsClosestToCamera(ThrustEngines, selectionNumber)
           );
            goalDirection += getDirectionTowardCamera().normalized;
		}

        if (Input.GetKey(ShipControls.GravityMinus) || ControllerDelegate.isDown())
        {
            RelevantTurnEngines.AddRange(
                getActionsFurthestFromGravity(ThrustEngines, selectionNumber)
            );
            RelevantDamperEngines.AddRange(
               getActionsClosestToGravity(ThrustEngines, selectionNumber)
           );
            goalDirection += getGravityDirection().normalized;
        }

        if (Input.GetKey(ShipControls.GravityPlus) || ControllerDelegate.isDown())
        {
            RelevantTurnEngines.AddRange(
                getActionsClosestToGravity(ThrustEngines, selectionNumber)
            );
            RelevantDamperEngines.AddRange(
               getActionsFurthestFromGravity(ThrustEngines, selectionNumber)
           );
            goalDirection += -getGravityDirection().normalized;
        }


        if (Input.GetKey(ShipControls.VelocityPlus)) {
			RelevantTurnEngines.AddRange (
				getActionsFurthestFromVelocity (ThrustEngines, selectionNumber)
			);
            RelevantDamperEngines.AddRange(
               getActionsClosestToVelocity(ThrustEngines, selectionNumber)
           );
            goalDirection += getDirectionTowardVelocity().normalized;
		}

		if (Input.GetKey(ShipControls.VelocityMinus)) {
			RelevantTurnEngines.AddRange (
				getActionsClosestToVelocity (ThrustEngines, selectionNumber)
			);
            RelevantDamperEngines.AddRange(
               getActionsFurthestFromVelocity(ThrustEngines, selectionNumber)
            );
            goalDirection += getDirectionAwayFromVelocity().normalized;
		}

		foreach (Action a in RelevantTurnEngines) {
            var aRigid = a.transform.GetComponent<Rigidbody>();
            var aLocalVelocity = aRigid.velocity - shipMainRigid.velocity;
            var dotProductLocalVelocity = Vector3.Dot(goalDirection.normalized, aLocalVelocity.normalized) ;
            var dotProductRotation = Vector3.Dot(goalDirection.normalized, a.transform.up.normalized);
            var thrustShare = (1f / (Mathf.Max((float)ShipControls.NumberActivated(), 1f)));

            if (!(dotProductLocalVelocity > 0f && dotProductRotation > 0f))
            {
                a.actionForce += a.maxActionForce * thrustShare;
                a.pilotEngaged = true;
            }
		}

        foreach(Action a in RelevantThrustEngines)
        {
            var thrustShare = (1f / (Mathf.Max((float)ShipControls.NumberActivated(), 1f)));
            a.actionForce += a.maxActionForce * thrustShare;
            a.pilotEngaged = true;
        }

        foreach (Action a in RelevantDamperEngines)
        {


            if (goalDirection != Vector3.zero)
            {

                var aRigid = a.transform.GetComponent<Rigidbody>();
                var aLocalVelocity = aRigid.velocity - shipMainRigid.velocity;
                var dotProductLocalVelocity = Vector3.Dot(goalDirection.normalized, aLocalVelocity.normalized);
                var dotProductRotation = Vector3.Dot(goalDirection.normalized, a.transform.up.normalized);
                var thrustShare = (1f / (Mathf.Max((float)ShipControls.NumberActivated(), 1f)));
                a.actionForce = a.maxActionForce*thrustShare;


                a.actionForce = Mathf.Clamp01(aLocalVelocity.magnitude) * a.maxActionForce;
            }
            else
            {
                a.actionForce = a.maxActionForce;
            }

        }
        foreach (Action a in RelevantBrakeEngines) {
			a.pilotEngaged = true;
		}
	}

    private Vector3 getDirectionAwayFromCamera() {
        return cam.transform.forward;
    }
    private Vector3 getDirectionTowardCamera() {
        return -(cam.transform.forward);
    }
    private Vector3 getDirectionAwayFromVelocity() {
        return -(shipMainRigid.velocity);
    }
    private Vector3 getDirectionTowardVelocity() {
        return shipMainRigid.velocity;
    }

    private List<Action> getActionsAll(List<Action> itemList){
		return (from Action e in itemList
		        select e).ToList ();
	}

	private List<Action> getActionsClosestToCamera(List<Action> itemList){
		return getActionsClosestToCamera (itemList, 1);
	}
	private List<Action> getActionsClosestToCamera(List<Action> itemList, int selectionNumber){
		
		return	(from Action e in itemList
		 		let weighting = (e.transform.position - cam.transform.position).sqrMagnitude 
		 		orderby weighting ascending 
		        select e).Take (selectionNumber).ToList ();
	}

	private List<Action> getActionsFurthestFromCamera(List<Action> itemList){
		return getActionsFurthestFromCamera (itemList, 1);
	}
	private List<Action> getActionsFurthestFromCamera(List<Action> itemList, int selectionNumber){
		
		return	(from Action e in itemList
		        let weighting = (e.transform.position - cam.transform.position).sqrMagnitude 
		        orderby weighting descending
		        select e).Take (selectionNumber).ToList ();
	}

	private List<Action> getActionsRightmostFromCamera(List<Action> itemList){
		return getActionsRightmostFromCamera (itemList, 1);
	}
	private List<Action> getActionsRightmostFromCamera(List<Action> itemList, int selectionNumber){
		
		return	(from Action e in itemList
		        let weighting = (Quaternion.Inverse (cam.transform.rotation) * (e.transform.position - cam.transform.position)).x 
		        orderby weighting ascending 
		        select e).Take (selectionNumber).ToList ();
	}

	private List<Action> getActionsLeftmostFromCamera(List<Action> itemList){
		return getActionsLeftMostFromCamera (itemList, 1);
	}
	private List<Action> getActionsLeftMostFromCamera(List<Action> itemList, int selectionNumber){
		
		return	(from Action e in itemList
		        let weighting = (Quaternion.Inverse (cam.transform.rotation) * (e.transform.position - cam.transform.position)).x 
		        orderby weighting descending 
		        select e).Take (selectionNumber).ToList ();
	}

	private List<Action> getActionsLeftmostFromVelocity(List<Action> itemList){
		return getActionsLeftMostFromVelocity (itemList, 1);
	}
	private List<Action> getActionsLeftMostFromVelocity(List<Action> itemList, int selectionNumber){
		Quaternion rotation = Quaternion.LookRotation (shipMainRigid.velocity, shipMainRigid.transform.up);		
		return	(from Action e in itemList
		        let weighting = (Quaternion.Inverse (rotation) * ((shipMainRigid.velocity+shipMainRigid.transform.position)-e.transform.position)).x 
		        orderby weighting descending 
		        select e).Take (selectionNumber).ToList ();
	}

	private List<Action> getActionsRightmostFromVelocity(List<Action> itemList){
		return getActionsRightMostFromVelocity (itemList, 1);
	}
	private List<Action> getActionsRightMostFromVelocity(List<Action> itemList, int selectionNumber){
		Quaternion rotation = Quaternion.LookRotation (shipMainRigid.velocity, shipMainRigid.transform.up);		
		return	(from Action e in itemList
		        let weighting = (Quaternion.Inverse (rotation) * ((shipMainRigid.velocity+shipMainRigid.transform.position)-e.transform.position)).x 
		        orderby weighting ascending 
		        select e).Take (selectionNumber).ToList ();
	}

	private List<Action> getActionsFurthestFromVelocity(List<Action> itemList){
		return getActionsFurthestFromVelocity (itemList, 1);
	}
	private List<Action> getActionsFurthestFromVelocity(List<Action> itemList, int selectionNumber){
		return	(from Action e in itemList
		        let weighting = ((shipMainRigid.transform.position + shipMainRigid.velocity)-e.transform.position).sqrMagnitude 
		        orderby weighting descending 
		        select e).Take (selectionNumber).ToList ();
	}
	
	private List<Action> getActionsClosestToVelocity(List<Action> itemList){
		return getActionsClosestToVelocity (itemList, 1);
	}
	private List<Action> getActionsClosestToVelocity(List<Action> itemList, int selectionNumber){
		return	(from Action e in itemList
		        let weighting = ((shipMainRigid.transform.position + shipMainRigid.velocity)-e.transform.position).sqrMagnitude
		        orderby weighting ascending 
		        select e).Take (selectionNumber).ToList ();
	}


	private List<Action> getActionsLeftmostFromGravity(List<Action> itemList){
		return getActionsLeftmostFromGravity (itemList, 1);
	}
	private List<Action> getActionsLeftmostFromGravity(List<Action> itemList, int selectionNumber){
		Quaternion rotation = Quaternion.LookRotation (shipMainRigid.velocity, shipMainRigid.transform.up);		
		return	(from Action e in itemList
		        let weighting = (Quaternion.Inverse (rotation) * ((Physics.gravity+shipMainRigid.transform.position)-e.transform.position)).x 
		        orderby weighting descending 
		        select e).Take (selectionNumber).ToList ();
	}
	
	private List<Action> getActionsRightmostFromGravity(List<Action> itemList){
		return getActionsRightMostFromGravity (itemList, 1);
	}
	private List<Action> getActionsRightMostFromGravity(List<Action> itemList, int selectionNumber){
		Quaternion rotation = Quaternion.LookRotation (shipMainRigid.velocity, shipMainRigid.transform.up);		
		return	(from Action e in itemList
		        let weighting = (Quaternion.Inverse (rotation) * ((Physics.gravity+shipMainRigid.transform.position)-e.transform.position)).x 
		        orderby weighting ascending 
		        select e).Take (selectionNumber).ToList ();
	}
	
	private List<Action> getActionsFurthestFromGravity(List<Action> itemList){
		return getActionsFurthestFromVelocity (itemList, 1);
	}
	private List<Action> getActionsFurthestFromGravity(List<Action> itemList, int selectionNumber){
        Vector3 grav = getGravityDirection() * 20.0f;
        return	(from Action e in itemList
		        let weighting = ((shipMainRigid.transform.position + grav)-e.transform.position).sqrMagnitude 
		        orderby weighting descending 
		        select e).Take (selectionNumber).ToList ();
	}
	
	private List<Action> getActionsClosestToGravity(List<Action> itemList){
		return getActionsClosestToGravity (itemList, 1);
	}
	private List<Action> getActionsClosestToGravity(List<Action> itemList, int selectionNumber){
		Vector3 grav =getGravityDirection()*20.0f;
		return	(from Action e in itemList
		        let weighting = ((shipMainRigid.transform.position + grav)-e.transform.position).sqrMagnitude
		        orderby weighting ascending 
		        select e).Take (selectionNumber).ToList ();
	}


	private void updateThrustEngines(){
		ThrustEngines = transform.parent.GetComponentsInChildren<Action> ().Where (x=>x.myAction == action.thrust && x.enabled == true).ToList();
	}

	private void updateBrakeEngines(){
		BrakeEngines = transform.parent.GetComponentsInChildren<Action> ().Where (x=>x.myAction == action.brake && x.enabled == true).ToList();
	}

    private Vector3 getGravityDirection()
    {
        return Gravities.getGravityAtPositionCheap(shipMainRigid.transform, 1.0f).normalized;
    }
	
}
