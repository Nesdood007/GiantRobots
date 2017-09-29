using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {

    CharacterController characterController;
    public float horizontalSpeed = 1f;
    public float forwardSpeed = 1f;
    public float rotationSpeed = 1f;
    public float gravity = 2f;
    public float jumpForce = 5f;
    public float airTime = 2f;
    bool isJumping = false;
    float elapsedJumpTime = 0f;
    Vector3 forward;
    Vector3 right;
    Vector3 gravityVec;
    Vector3 up;

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
        var jump = Input.GetKeyDown("space");
       
        
        if (Input.GetAxis("Rotate") != 0)        
            transform.Rotate(new Vector3(0, rotationSpeed * Time.deltaTime * Input.GetAxis("Rotate"), 0));

        if (isJumping)
        {
            elapsedJumpTime += Time.deltaTime;
            var jumpHeight = GetJumpHeight(elapsedJumpTime);
            up = new Vector3(0, jumpHeight, 0);
            if (elapsedJumpTime >= airTime)
            {
                isJumping = false;
                elapsedJumpTime = 0;
            }
            characterController.SimpleMove(up);
            return;
        }

        if (!characterController.isGrounded)
        {
            characterController.SimpleMove(gravityVec);
            Debug.Log(gravityVec);
            return;
        }

        if (forw != 0)
        {
            var direction = transform.TransformDirection(Vector3.forward);
            direction = direction * forwardSpeed;
            forward = new Vector3(direction.x * Time.deltaTime * forw, 0, direction.z * Time.deltaTime * forw);
        }
        else
            forward = Vector3.zero;
        
        if(side != 0)
        {
            var direction = transform.TransformDirection(Vector3.right);            
            direction = direction * forwardSpeed;
            right = new Vector3(direction.x * Time.deltaTime * side, 0, direction.z * Time.deltaTime * side);
        }
        else
            right = Vector3.zero;

       

        if (jump)
        {
            //var direction = transform.TransformDirection(Vector3.up);
            //direction = direction * jumpForce;
            //up = new Vector3(0, direction.y * Time.deltaTime, 0);
            isJumping = true;
            //Debug.Log(direction);
        }
        else
            up = Vector3.zero;

        //if (jump)
        //    characterController.Move(forward + right + up);
        //else
            characterController.Move(forward + right + gravityVec);
       
    }

    float GetJumpHeight(float elapsedTime)
    {
        //y = -(x - h)2
        return -Mathf.Pow((elapsedTime - airTime * 2), 2) * jumpForce;
    }
}
