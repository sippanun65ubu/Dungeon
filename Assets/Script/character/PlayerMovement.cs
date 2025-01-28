using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{

    Animator animator;
    float velocityX = 0f;
    float velocityZ = 0f;
    public float acceleration = 2.0f;
    public float deceleration = 2.0f;
    public float maximumCrouchVelocity = 4.0f;
    public float maximumWalkVelocity = 6.0f;
    public float maximumRunVelocity = 10.0f;

    private bool isCrouching = false;

    int VelocityZHash;
    int VelocityXHash;
    public CharacterController controller;

    public float speed = 6f;
    public float gravity = -9.81f * 2;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;

    bool isGrounded;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        animator = GetComponent<Animator>();

        VelocityXHash = Animator.StringToHash("Velocity X");
        VelocityZHash = Animator.StringToHash("Velocity Z");
    }
    // Update is called once per frame
    void Update()
    {
        //checking if we hit the ground to reset our falling velocity, otherwise we will fall faster the next time
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        //right is the red Axis, foward is the blue axis
        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);

        //check if the player is on the ground so he can jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            //the equation for jumping
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        bool forwardPressed = Input.GetKey(KeyCode.W);
        bool leftPressed = Input.GetKey(KeyCode.A);
        bool backPressed = Input.GetKey(KeyCode.S);
        bool rightPressed = Input.GetKey(KeyCode.D);
        bool runPressed = Input.GetKey(KeyCode.LeftShift);
        bool crouchPressed = Input.GetKeyDown(KeyCode.C);


        float currentMaxVelocity = runPressed ? maximumRunVelocity : maximumWalkVelocity;

        //handle change in velocity
        changeVelocity(forwardPressed, leftPressed, backPressed, rightPressed,runPressed,currentMaxVelocity);
        lockOrResetVelocity(forwardPressed, leftPressed, backPressed, rightPressed, runPressed, currentMaxVelocity);

        animator.SetFloat(VelocityXHash, velocityX);
        animator.SetFloat(VelocityZHash, velocityZ);

    }
    //handle acceleration and deceleration
    void changeVelocity(bool forwardPressed,bool leftPressed, bool rightPressed, bool backPressed, bool runPressed, float currentMaxVelocity)
    {
        //forward
        if (forwardPressed && velocityZ < currentMaxVelocity)
        {
            velocityZ += Time.deltaTime * acceleration;
        }
        //left
        if (leftPressed && velocityX < -currentMaxVelocity)
        {
            velocityX -= Time.deltaTime * acceleration;
        }
        //right
        if (rightPressed && velocityX < currentMaxVelocity)
        {
            velocityX += Time.deltaTime * acceleration;
        }
        //back
        if (backPressed && velocityZ < -currentMaxVelocity)
        {
            velocityZ += Time.deltaTime * acceleration;
        }
        //decrease Z velocity
        if (!forwardPressed && velocityZ > 0.0f)
        {
            velocityZ -= Time.deltaTime * deceleration;
        }
        //increase Z velocity
        if (!backPressed && velocityZ > 0.0f)
        {
            velocityZ += Time.deltaTime * deceleration;
        }
        //increase X velocity
        if (!leftPressed && velocityX < 0.0f)
        {
            velocityX += Time.deltaTime * deceleration;
        }
        //decrease X velocity
        if (!rightPressed && velocityX > 0.0f)
        {
            velocityX -= Time.deltaTime * deceleration;
        }
    }
    void lockOrResetVelocity(bool forwardPressed, bool leftPressed, bool rightPressed, bool backPressed, bool runPressed, float currentMaxVelocity)
    {
        //reset Z velocity
        if (!forwardPressed && !backPressed && velocityX != 0.0f && (velocityZ > -0.05f && velocityZ < 0.05f))
        {
            velocityZ = 0.0f;
        }

        //reset X velocity
        if (!leftPressed && !rightPressed && velocityX != 0.0f && (velocityX > -0.05f && velocityX < 0.05f))
        {
            velocityX = 0.0f;
        }
        //run froward
        if (forwardPressed && runPressed && velocityZ > currentMaxVelocity)
        {
            velocityZ = currentMaxVelocity;
        }
        //decelerate
        else if (forwardPressed && velocityZ > currentMaxVelocity)
        {
            velocityZ -= Time.deltaTime * deceleration;
            if (velocityZ > currentMaxVelocity && velocityZ < (currentMaxVelocity + 0.05f))
            {
                velocityZ = currentMaxVelocity;
            }
        }
        //round velocity
        else if (forwardPressed && velocityZ < currentMaxVelocity && velocityZ > (currentMaxVelocity - 0.05f))
        {
            velocityZ = currentMaxVelocity;
        }
        //run back
        if (backPressed && runPressed && velocityZ > -currentMaxVelocity)
        {
            velocityZ = -currentMaxVelocity;
        }
        //decelerate
        else if (backPressed && velocityZ > -currentMaxVelocity)
        {
            velocityZ -= Time.deltaTime * deceleration;
            if (velocityZ > -currentMaxVelocity && velocityZ < (-currentMaxVelocity - 0.05f))
            {
                velocityZ = -currentMaxVelocity;
            }
        }
        //round velocity
        else if (backPressed && velocityZ < -currentMaxVelocity && velocityZ > (-currentMaxVelocity + 0.05f))
        {
            velocityZ = -currentMaxVelocity;
        }
        //run left
        if (leftPressed && runPressed && velocityX > -currentMaxVelocity)
        {
            velocityX = -currentMaxVelocity;
        }
        //decelerate
        else if (leftPressed && velocityX > -currentMaxVelocity)
        {
            velocityX -= Time.deltaTime * deceleration;
            if (velocityX > -currentMaxVelocity && velocityX < (-currentMaxVelocity - 0.05f))
            {
                velocityX = -currentMaxVelocity;
            }
        }
        //round velocity
        else if (leftPressed && velocityX < -currentMaxVelocity && velocityX > (-currentMaxVelocity + 0.05f))
        {
            velocityX = -currentMaxVelocity;
        }
        //run right
        if (rightPressed && runPressed && velocityX > currentMaxVelocity)
        {
            velocityX = currentMaxVelocity;
        }
        //decelerate
        else if (rightPressed && velocityX > currentMaxVelocity)
        {
            velocityX -= Time.deltaTime * deceleration;
            if (velocityX > currentMaxVelocity && velocityX < (currentMaxVelocity + 0.05f))
            {
                velocityX = currentMaxVelocity;
            }
        }
        //round velocity
        else if (rightPressed && velocityX < currentMaxVelocity && velocityX > (currentMaxVelocity - 0.05f))
        {
            velocityX = currentMaxVelocity;
        }
    }
}