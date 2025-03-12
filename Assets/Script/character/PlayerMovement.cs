using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public CapsuleCollider colliderp;

    [Header("Animation Settings")]
    public Animator animator; // Assign the child's animator in Unity Inspector
    private float velocityX = 0f;
    private float velocityZ = 0f;

    [Header("Movement Settings")]
    public float walkSpeed = 3f;       // Normal walking speed
    public float sprintSpeed = 7f;    // Speed while sprinting
    private float currentSpeed;        // The speed that changes dynamically

    [Header("Jump & Gravity")]
    public float gravity = -9.81f * 2;
    public float jumpHeight = 3f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;


    private Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();

        if (animator == null)
        {
            Debug.LogError("Animator not found! Assign it manually.");
        }

        currentSpeed = walkSpeed;  // Start with normal walking speed
    }

    void Update()
    {
        if (MovementManager.instance.canMove)
        {
            Movement();
        }
        if (DialogSystem.instance.dialogUIActive == false)
        {
            Movement();
        }
    }

    public void Movement()
    {
        // Check if player is on the ground
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Reset fall speed

        }

        // Get movement input
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        bool isMoving = x != 0 || z != 0;

        bool isRunning = false;
        // Handle Sprinting 
        if (Input.GetKey(KeyCode.LeftShift) && PlayerState.Instance.currentStamina > 0)
        {
            currentSpeed = sprintSpeed; // Sprint speed
            isRunning = true;
        }
        else
        {
            currentSpeed = walkSpeed; // Normal walking speed
        }

        if (PlayerState.Instance != null)
        {
            PlayerState.Instance.isSprinting = isRunning;
        }

        // Move in the direction the camera is facing
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // **Update Animation Parameters**
        velocityX = x * currentSpeed;
        velocityZ = z * currentSpeed;

        animator.SetFloat("X_Velocity", velocityX);
        animator.SetFloat("Z_Velocity", velocityZ);

        // **Set Animation Conditions**
        animator.SetBool("IsWalking", isMoving && currentSpeed == walkSpeed);
        animator.SetBool("IsRunning", isMoving && currentSpeed == sprintSpeed);


        // Jumping
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetTrigger("IsJumping");

            // **Check if moving forward for Jump Forward**
            if (Mathf.Abs(velocityZ) > 1)
            {
                animator.SetFloat("Z_Velocity", 1); // Jump Forward
            }
            else
            {
                animator.SetFloat("Z_Velocity", 0); // Normal Jump
            }
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
