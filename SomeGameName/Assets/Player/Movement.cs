using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {

    CharacterController characterController;
    public float horizontalSpeed = 1f;
    public float forwardSpeed = 1f;
    public float rotationSpeed = 1f;
    public float gravity = 2f;
    Vector3 forward;
    Vector3 right;
    Vector3 gravityVec;
    
    //Enable or disable mouse movement
    public bool allowMouseX = true;
    
    
    // Use this for initialization
    void Start () {
        characterController = GetComponent<CharacterController>();
        gravityVec = new Vector3(0, -gravity, 0);
        
    }
	
	// Update is called once per frame
	void Update () {
        //var direction = transform.TransformDirection(Vector3.forward + Vector3.right);
        //characterController.Move(new Vector3(direction.x * horizontalSpeed * Time.deltaTime * Input.GetAxis("Horizontal"), , direction.z * forwardSpeed * Time.deltaTime * Input.GetAxis("Vertical")));


        var forw = Input.GetAxis("Vertical");
        var side = Input.GetAxis("Horizontal");
        var rot = Input.GetAxis("Rotate");
        var mouseX = Input.GetAxis("Mouse X");
        
        
        if (allowMouseX && mouseX != 0) {
          transform.Rotate(new Vector3(0, rotationSpeed * Time.deltaTime * mouseX, 0));
        }

        if (rot != 0) {    
            transform.Rotate(new Vector3(0, rotationSpeed * Time.deltaTime * rot, 0));
        }

        if (forw != 0)
        {
            var direction = transform.TransformDirection(Vector3.forward);            
            direction = direction * forwardSpeed;
            forward = new Vector3(direction.x * Time.deltaTime * forw, 0, direction.z * Time.deltaTime * forw);
        }
        
        if(side != 0)
        {
            var direction = transform.TransformDirection(Vector3.right);            
            direction = direction * forwardSpeed;
            right = new Vector3(direction.x * Time.deltaTime * side, 0, direction.z * Time.deltaTime * side);
        }

        if (side != 0 && forw != 0)
            characterController.Move(forward + right + gravityVec);
        else if(side != 0)
            characterController.Move(right + gravityVec);
        else if (forw != 0)
            characterController.Move(forward + gravityVec);
        else
            characterController.Move(gravityVec);
    }
}
