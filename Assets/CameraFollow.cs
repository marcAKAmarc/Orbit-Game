using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum CameraMode{follow, orbital, mouseOrient};

public class CameraFollow : MonoBehaviour
{
	public Transform target;
	public float distance;
	public Transform mouseLook;
	public float mouseDelay;
	public float mouseLookDist;
	public float timeSinceMouse;
//	private Vector3 initialOffset;
//	private Quaternion initialRotation;
	public CameraMode mode = CameraMode.orbital;
	public Transform worldRotation;
    private Vector3 goalDirFromTarget;
    private mouseSmooth mouseSmoother;
    private mouseWheelSmooth wheelSmoother;
	void Start(){
        mouseSmoother = new mouseSmooth(10);
        wheelSmoother = new mouseWheelSmooth(10);
		timeSinceMouse = mouseDelay;
//		initialOffset = transform.position - target.position;
//		initialRotation = Quaternion.FromToRotation (target.forward, initialOffset.normalized);
		Rigidbody rigid = transform.GetComponent<Rigidbody> ();
		rigid.MoveRotation(Quaternion.LookRotation (target.position - transform.position, -Gravities.getGravityAtPositionCheap(transform,1.0f).normalized));
	}

	void FixedUpdate(){
		//handleMouse ();
		updateMode ();
		updatePos ();
	}

	private void handleMouse(){
		if(Input.GetMouseButtonDown (0)){
			mouseLook.parent.rotation = transform.rotation;
			mouseLook.localRotation = Quaternion.identity;
			Debug.Log ("target : " + target.position.ToString() + " position: "+ transform.position.ToString());
			mouseLookDist = (target.position - transform.position).magnitude;
		}

		bool active = Input.GetMouseButton (0);
		if (active) {

			timeSinceMouse = 0.0f;
		}

		if (!active) {
			timeSinceMouse += Time.fixedDeltaTime;
		}
	}

	private void updateMode(){
		if (timeSinceMouse < mouseDelay) {
			mode = CameraMode.mouseOrient;
		} else {
			mode = CameraMode.orbital;
		}
	}

	public void updatePos()
	{
        mouseSmoother.updateStack();
        Vector2 mouseInput = Vector2.zero;
        if(Input.GetMouseButton(0))
            mouseInput = mouseSmoother.getAverage();

        wheelSmoother.updateStack();
        float wheelInput = wheelSmoother.getAverage();
        distance = distance + (wheelInput*distance*1f);
		if (target  ) {

			Rigidbody rigid = transform.GetComponent<Rigidbody>();
			Rigidbody targetRigid = target.transform.GetComponent<Rigidbody>();

			Vector3 direction = transform.position-target.position;
			Vector3 grav = Gravities.getGravityAtPositionCheap (target.transform, 1.0f).normalized;

			Vector3 goalPos = Vector3.zero;
			Vector3 goalUp = transform.up;

			//each mode must simply define the goal Pos and the goal up.
	
			if(mode == CameraMode.follow){

				goalPos = target.position +(direction.normalized*(distance)+Vector3.up+Vector3.right);
				Rigidbody r = target.GetComponent<Rigidbody>();
				if(r)
					goalPos-=r.velocity*Time.fixedDeltaTime*5.0F;
			}else if(mode == CameraMode.mouseOrient){

				Debug.Log ("Mouse look dist: " + mouseLookDist);
				goalPos = target.position + (-mouseLook.transform.forward * mouseLookDist );	
			}
			else if(mode == CameraMode.orbital){

				Quaternion relativeRot = Quaternion.LookRotation (transform.position - (target.position-(targetRigid.velocity*Time.fixedDeltaTime)), transform.up);
				worldRotation.rotation = Quaternion.FromToRotation (worldRotation.up, -grav) * worldRotation.rotation;
                float yRot = mouseInput.y * 2f;//Input.GetAxis("Mouse X");

                float xRot = mouseInput.x * 2f;//Input.GetAxis("Mouse Y");

				Quaternion rotOnLand = Quaternion.Inverse (worldRotation.rotation)  * relativeRot * Quaternion.Euler (-xRot, yRot, 0f);
			
				goalPos = target.position + (worldRotation.rotation * rotOnLand * Vector3.forward* distance);

				if(Input.GetAxis("Mouse X") == 0.0f && Input.GetAxis("Mouse Y") == 0.0f)
					goalUp = -grav;
			}
            //movement
            Ray ray = new Ray(target.position, (goalPos - target.position).normalized);
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(ray, out hit, (goalPos - target.position).magnitude+2f, 1 << LayerMask.NameToLayer("Planet"))) {
                rigid.MovePosition(transform.position + ((hit.point - transform.position)-((hit.point-transform.position).normalized * 2f)));
            }
            else
    			rigid.MovePosition(transform.position + (goalPos - transform.position));

			//rotation
			Quaternion goalRot = Quaternion.FromToRotation(transform.forward, (target.position - transform.position).normalized) * transform.rotation;
			rigid.MoveRotation(Quaternion.Slerp(goalRot, Quaternion.LookRotation(transform.forward, goalUp), .1f));		
		}
	}

    
}

public class mouseSmooth{
    List<Vector2> stack = new List<Vector2>();
    public mouseSmooth( int _buffer)
    {
        for(int i = 0; i < _buffer; i++)
        {
            stack.Add(new Vector2(0f, 0f));
        }
    }
    public void updateStack()
    {
        stack.Add(new Vector2(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X")));
        stack.RemoveAt(0);
    }

    public Vector2 getAverage()
    {
        Vector2 avg = new Vector2();
        foreach (var v in stack)
        {
            avg += v;
        }
        avg = avg / stack.Count;
        return avg;
    }
}

public class mouseWheelSmooth
{
    List<float> stack = new List<float>();
    public mouseWheelSmooth(int _buffer)
    {
        for (int i = 0; i < _buffer; i++)
        {
            stack.Add(0f);
        }
    }
    public void updateStack()
    {
        stack.Add(Input.GetAxis("Mouse ScrollWheel"));
        stack.RemoveAt(0);
    }

    public float getAverage()
    {
        var avg =0f;
        foreach (var v in stack)
        {
            avg += v;
        }
        avg = avg / stack.Count;
        return avg;
    }
}