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
    public float walkSpeed = 6f;       // Normal walking speed
    public float sprintSpeed = 12f;    // Speed while sprinting
    public float crouchSpeed = 3f;     // Speed while crouching
    private float currentSpeed;        // The speed that changes dynamically

    [Header("Jump & Gravity")]
    public float gravity = -9.81f * 2;
    public float jumpHeight = 3f;
    private bool isJumping = false; // Track jump state

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    [Header("Crouch Settings")]
    public float crouchHeight = 2.75f;  // Height when crouching
    public float standHeight = 4f;     // Height when standing
    private bool isCrouching = false;

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
        // Check if player is on the ground
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Reset fall speed
            isJumping = false; // Reset jump state when grounded
            animator.SetBool("IsJumping", false);
        }

        // Get movement input
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        bool isMoving = x != 0 || z != 0;

        // Handle Sprinting (Prevent sprinting while crouching)
        if (Input.GetKey(KeyCode.LeftShift) && !isCrouching)
        {
            currentSpeed = sprintSpeed; // Sprint speed
        }
        else if (isCrouching)
        {
            currentSpeed = crouchSpeed; // Use crouch speed
        }
        else
        {
            currentSpeed = walkSpeed; // Normal walking speed
        }

        // Handle Crouching
        if (Input.GetKeyDown(KeyCode.C))
        {
            isCrouching = !isCrouching; // Toggle crouch state

            if (isCrouching)
            {
                controller.height = crouchHeight; // Reduce height
                colliderp.height = crouchHeight;
            }
            else
            {
                controller.height = standHeight; // Restore height
                colliderp.height = standHeight;
            }
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
        animator.SetBool("IsCrouch", isCrouching);

        // Jumping
        if (Input.GetButtonDown("Jump") && isGrounded && !isCrouching) // Prevent jumping while crouching
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            isJumping = true;
            animator.SetBool("IsJumping", true); // Play jump animation

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
