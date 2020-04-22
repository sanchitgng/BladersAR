using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement_Controller : MonoBehaviour
{
    public Joystick joystick;
    [SerializeField] float speed = 2f;

    private Vector3 velocityVector = Vector3.zero; // initial velocity

    public float maxVelocityChange = 4f;
    public float tiltAmount = 10f;

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // taking input from joystick
        float xMovementInput = joystick.Horizontal;
        float zMovementInput = joystick.Vertical;

        // calculating velocity vectors
        Vector3 xMovement = transform.right * xMovementInput; //represents x movement vector
        Vector3 zMovement = transform.forward * zMovementInput; //represents z movement vector 

        // calculating final movement velocity vector
        Vector3 MovementVelocity = (xMovement + zMovement).normalized * speed;
        // what we did is, we took the horizontal and vertical comp, added them, normalised it so that it's length is 1 and then multiplied it with our speed.

        // Applying Movemment
        MovementApply(MovementVelocity);

        transform.rotation = Quaternion.Euler(joystick.Vertical * speed * tiltAmount, 0, -1 * joystick.Horizontal * speed * tiltAmount);


    }

    private void MovementApply(Vector3 movementVelocity)
    {
        velocityVector = movementVelocity;
    }

    private void FixedUpdate()
    {
        if(velocityVector != Vector3.zero)
        {
            // get rigidbody currrent velocity
            Vector3 currVelocity = rb.velocity;
            Vector3 velocityChange = (velocityVector - currVelocity);

            // Apply a force by the amount of velocity change to reach the target velocity
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, +maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, +maxVelocityChange);
            velocityChange.y = 0f;
            rb.AddForce(velocityChange, ForceMode.Acceleration);
        }

        
    }
}
