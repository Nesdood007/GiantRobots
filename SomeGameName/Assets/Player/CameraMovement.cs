using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {
    
    public Vector3 cameraOffset = new Vector3(0, 2, -10);
	// Use this for initialization
	void Start () {
        transform.position = GameObject.FindGameObjectWithTag("Player").transform.position + cameraOffset;
    }
	
	// Update is called once per frame
	void Update () {
        
	}
}
