using UnityEngine;
using System.Collections;


public enum movementStick{up, upleft, left, downleft, down, downright, right, upright, none}

public class ControllerDelegate{

	public static movementStick stick = movementStick.none;

	public static void setStick(){
		stick = getMovementStick ();
	}

	public static movementStick getMovementStick(){
		float threshold = Mathf.Asin (Mathf.Deg2Rad * 30.0f);
		float horizontal = Input.GetAxis ("Horizontal");
		float vertical = Input.GetAxis ("Vertical");

		if (vertical > threshold) {
			if (horizontal > threshold)
				return movementStick.upright;
			if (horizontal < -threshold)
				return movementStick.upleft;

			return movementStick.up;
		}

		if (vertical < -threshold) {
			if(horizontal > threshold)
				return movementStick.downright;
			if(horizontal < -threshold)
				return movementStick.downleft;

			return movementStick.down;
		}

		if(horizontal > threshold)
			return movementStick.right;
		if(horizontal < -threshold)
			return movementStick.left;

		return movementStick.none;
	}


	public static bool isUp(){
		return isUp (stick);
	}

	public static bool isUp(movementStick stick){
		if (stick == movementStick.up || stick == movementStick.upleft || stick == movementStick.upright)
			return true;
		else
			return false;
	}


	public static bool isDown(){
		return isDown (stick);
	}
	public static bool isDown(movementStick stick){
		if (stick == movementStick.down || stick == movementStick.downleft || stick == movementStick.downright)
			return true;
		else
			return false;
	}

	public static bool isRight(){
		return isRight (stick);
	}
	public static bool isRight(movementStick stick){
		if (stick == movementStick.right || stick == movementStick.upright || stick == movementStick.downright)
			return true;
		else
			return false;
	}

	public static bool isLeft(){
		return isLeft (stick);
	}
	public static bool isLeft(movementStick stick){
		if (stick == movementStick.left || stick == movementStick.downleft || stick == movementStick.upleft)
			return true;
		else
			return false;
	}
}
