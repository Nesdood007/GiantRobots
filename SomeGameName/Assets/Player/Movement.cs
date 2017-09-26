using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {

    Rigidbody rigidBody;
    public float horizontalSpeed = 1f;
    public float forwardSpeed = 1f;

    // Use this for initialization
    void Start () {
        rigidBody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(new Vector3(horizontalSpeed * Time.deltaTime * Input.GetAxis("Horizontal"), 0, forwardSpeed * Time.deltaTime * Input.GetAxis("Vertical")));

    }
}
