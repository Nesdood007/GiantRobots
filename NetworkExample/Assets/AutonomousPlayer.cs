using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class AutonomousPlayer : NetworkBehaviour {

  public float deltaX = 1.0f, deltaZ = 1.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (!isServer) {
      return;
    }
    
    var x = deltaX * Time.deltaTime * 150.0f;
    var z = deltaZ * Time.deltaTime * 3.0f;

    transform.Rotate(0, x, 0);
    transform.Translate(0, 0, z);
	}
}
