using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WiiU;
using UnityEngine.UI;

public class CameraController : MonoBehaviour {

	public Transform cameraPivot;
	GamePad gamePad;

	public float speed = 1f;

	void Start()
	{
		// Create a GamePad object
		gamePad = GamePad.access;
	}
	void Update()
	{
		// Get the new state of the GamePad
		GamePadState gamePadState = gamePad.state;

		if (gamePadState.gamePadErr == GamePadError.None) 
		{

			// Get Gyro Data, Convert To Degrees //
			Vector3 gyroData = gamePadState.gyro;
			//Vector3 gyroTwo = gamePadState.
			Vector3 gyroDegree = new Vector3 (Mathf.Rad2Deg * gyroData.x, -Mathf.Rad2Deg * gyroData.z, Mathf.Rad2Deg * gyroData.y) * speed / 10f;

			cameraPivot.Rotate (gyroDegree);

			// Check for the A button
			if (gamePadState.IsTriggered(GamePadButton.A)) 
			{ 
				//speed *= 10f;
				cameraPivot.rotation = Quaternion.identity;
			}
			if (gamePadState.IsTriggered(GamePadButton.B))
			{ 
				//speed /= 10f;
			}
			/*

			// Read data from the gyroscope
			Vector3 gyroData = gamePadState.gyro;
			Vector3 scaledData = new Vector3 (-gyroData.x, -gyroData.z, gyroData.y);

			Vector3 acelData = gamePadState.acc;
			Vector3 gravData = gamePadState.magnetometer;

			// Read data from the left stick
			// Vector2 leftStick = gamePadState.lStick; 

			cameraPivot.Rotate(scaledData * speed);


			// Read data from the gyroscope
			Vector3 gyroData = gamePadState.gyro;
			Vector3 scaledData = new Vector3(-gyroData.x, -gyroData.z, gyroData.y);

			// Clamp the x-axis rotation to prevent flipping over
			currentRotation.x += scaledData.x * speed * Time.deltaTime;
			currentRotation.x = Mathf.Clamp(currentRotation.x, -90f, 90f);

			currentRotation.y += scaledData.y * speed * Time.deltaTime;

			currentRotation.z += scaledData.z * speed * Time.deltaTime;

			// Apply the rotation to the camera pivot
			cameraPivot.rotation = Quaternion.Euler(currentRotation.x, currentRotation.y, currentRotation.z);

			/// MOVE ///
			Vector3 acelData = gamePadState.acc;
			if (acelData.z < 0) {
				transform.Translate (acelData.z * Vector3.forward * moveSpeed * Time.deltaTime);
			}
			if (acelData.z > 0) {
				transform.Translate (acelData.z * Vector3.back * moveSpeed * Time.deltaTime);
			}

			// Reading magnetometer data
			Vector3 magnData = gamePadState.magnetometer;
			// Assume magnData provides a directional vector towards magnetic north
			// Calculate the orientation angle relative to north
			float angleToNorth = Mathf.Atan2(magnData.y, magnData.x) * Mathf.Rad2Deg;

			// Adjusting y-rotation of the camera based on magnetic north
			// This simplistic approach may need refinement for accuracy
			currentRotation.y = angleToNorth;

			cameraPivot.rotation = Quaternion.Euler(currentRotation);
			*/
		}
	}

}
