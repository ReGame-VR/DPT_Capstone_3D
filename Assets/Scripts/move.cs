using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move : MonoBehaviour {
	int mode = 1;

	// Use this for initialization
	void Start () {
		gameObject.GetComponent<Renderer>().material.color = Color.blue;
	}
	
	// Update is called once per frame
	void Update () {
		if (transform.position.x < -1.5) {
			mode = 2;
		}  else if (transform.position.x > 1.25) {
			mode = 1;
		}


		if (mode == 1) {
			transform.Translate (-.01f, 0, 0);
		} else if (mode == 2){
			transform.Translate (.01f, 0, 0);
		}
	}
}
