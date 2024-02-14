using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WiiU;

public class ArmController : MonoBehaviour {

	// 0 Is Left, 1 Is Right //
	public int channel;

	public Transform startShootPoint;
	public LineRenderer line;

	void Start(){
		line.SetColors (new Color(1f, 0f, 0f, 0.5f), new Color(1f, 0f, 0f, 0.5f));
	}

	void Update(){

		// Rotate Arm //
		RotateUpdate ();

		// Aim Line //
		AimUpdate ();

	}

	void RotateUpdate(){
		// Rotate Plane In Direction Of Controller //
		MotionPlusState data = Remote.Access(channel).state.motionPlus;
		Remote.Access(channel).motionPlus.Enable(MotionPlusMode.Standard);

		var look = -data.dir.Y;
		var up = data.dir.Z;

		look.x *= -1;
		up.x *= -1;

		transform.localRotation = Quaternion.LookRotation(up, look);
	}

	void AimUpdate(){
		RaycastHit hit;
		Vector3 endPosition;

		// Shoot raycast
		if (Physics.Raycast(startShootPoint.position, startShootPoint.TransformDirection(Vector3.forward), out hit)) {
			// If Hit, get the point where the raycast hit
			endPosition = hit.point;
		}
		else {
			// If not hit, set end position far away in the direction
			endPosition = startShootPoint.position + startShootPoint.TransformDirection(Vector3.forward) * 100; // Change 100 to whatever max distance you want
		}

		// Set LineRenderer positions
		line.positionCount = 2; // Set the number of points to 2
		line.SetPosition(0, startShootPoint.position); // Start position
		line.SetPosition(1, endPosition); // End position based on raycast hit or default
	}
}
