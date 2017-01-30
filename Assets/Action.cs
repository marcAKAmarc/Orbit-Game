using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum action{thrust, brake}

public class Action : MonoBehaviour {
	public action myAction;
	public Transform Engine;
	public List<KeyCode> KeyList = new List<KeyCode>();


    public float maxActionForce = 100f;

    public float actionForce = 10f;

    void Awake()
    {
        maxActionForce = 50f;
        actionForce = maxActionForce;
    }
	[HideInInspector]
	public bool pilotEngaged;

	void LateUpdate(){
//		activateParticles (Engine, false);

	}
	void turnOffThrustEffects(){
		activateParticles (Engine, false);
		activateLight (Engine, false);
		activateSound (Engine, false);
	}

	void turnOnThrustEffects(Transform engine){
		activateParticles (engine, true);
		activateLight (engine, true);
		activateSound (engine, true);
	}

	void FixedUpdate(){
//		bool performed = false;
		foreach (KeyCode key in KeyList) {
			if (Input.GetKey (key) || pilotEngaged) {
				performAction(myAction, Engine);
//				performed = true;
			}
		}
	}

	private void performAction(action myAction, Transform myEngine){
		if (myAction == action.thrust) {
			thrust(myEngine);
		}
	
		if (myAction == action.brake) {
			brake(myEngine);
		}
	}

	private void thrust(Transform engine){

		Rigidbody rigid = transform.GetComponent<Rigidbody>();
		if (rigid == null)
			rigid = transform.GetComponentInParent<Rigidbody> ();
		Vector3 Pos = transform.position;
		Vector3 EnginePos = new Vector3(engine.position.x, engine.position.y, engine.position.z );

		rigid.AddForce((Pos-EnginePos)*actionForce);

        if(actionForce > 0f)
		    turnOnThrustEffects (engine);
	}

	private void activateParticles(Transform engine, bool on){
		ParticleSystem ps = engine.GetComponent<ParticleSystem> ();
		if (ps) 
			ps.enableEmission = on;
	}

	private void activateLight(Transform engine, bool on){
		Light l = engine.GetComponent<Light> ();
		if (l) {
			CancelInvoke ("turnOffThrustEffects");
			l.enabled = on;
			if (on)
				Invoke ("turnOffThrustEffects", Time.deltaTime);
		}

	}
	private void activateSound(Transform engine, bool on){
		AudioSource[] audios = engine.GetComponents<AudioSource> ();
		foreach(AudioSource audio in audios){
			if (on){
				if(!audio.isPlaying)
					audio.Play();
			}
			else
				audio.Stop ();
		}
	}

	private void brake(Transform engine){

		Debug.Log ("braking");
		
		Rigidbody rigid = transform.GetComponentInParent<Rigidbody> ();
		Vector3 Pos = transform.position;
		Vector3 EnginePos = new Vector3(engine.position.x, engine.position.y, engine.position.z );
		
		Vector3 brakeForce = Vector3.zero;
		if(rigid != transform.GetComponent<Rigidbody>())
			brakeForce = -(rigid.GetPointVelocity(transform.position) - rigid.velocity).normalized;

		rigid.AddForceAtPosition (brakeForce * 10.0f, transform.position);

//		Debug.Log ("braking");
//		Rigidbody rigid = transform.GetComponent<Rigidbody>();
//
//		Vector3 Pos = transform.position;
//		Vector3 EnginePos = new Vector3(engine.position.x, engine.position.y, engine.position.z );
//
//		Vector3 brakeForce;
//		Rigidbody jointRigid = null;
//		if (transform.GetComponent<FixedJoint>()) 
//			jointRigid = transform.GetComponent<FixedJoint>().connectedBody;
//		
//		if (jointRigid)
//			brakeForce = -(rigid.velocity - jointRigid.velocity).normalized;
//		else
//			brakeForce = -rigid.velocity * (Pos-EnginePos).magnitude;
//
//		rigid.AddForce(brakeForce*10.0f);
	}
	
	private void explode(){
		Rigidbody rigid = transform.GetComponent<Rigidbody>();
		Vector3 mypos = transform.position;
		Vector3 pos = new Vector3(mypos.x+.5f, mypos.y+ .5f, mypos.z + .5f)*100.0f;

		rigid.AddForceAtPosition (pos-mypos, pos);
	}
}
