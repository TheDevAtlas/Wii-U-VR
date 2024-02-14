using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordController : MonoBehaviour {

	public AudioSource sound;

	void OnTriggerEnter(Collider col){
		if (col.gameObject.tag == "Note") {
			Destroy (col.gameObject);
			sound.Play ();
		}
	}
}
