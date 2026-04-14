using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float baseWalkSpeed = 5f;
    [SerializeField] private float baseRunSpeed = 8f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float groundCheckDistance = 1.1f;

    public float speedMultiplier = 1.0f;

    private Rigidbody rb;
    private Transform cameraTransform;

    private float moveX;
    private float moveZ;
    private bool jumpRequest;
    private Vector3 moveDirection;

    [Header("Anim values")]
    public float groundSpeed;

    public bool IsGrounded => Physics.Raycast(transform.position + Vector3.up * 0.01f, Vector3.down, groundCheckDistance);

    private bool IsRunning => Input.GetKey(KeyCode.LeftShift);

    private void Awake()
    {
        InitializeComponents();
    }

    private void Update()
    {
        RegisterInput();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void InitializeComponents()
    {
        rb = GetComponent<Rigidbody>(); // Get the Rigidbody component
        rb.freezeRotation = true; // Prevent Rigidbody from rotating due to physics interactions
        rb.interpolation = RigidbodyInterpolation.Interpolate; // Smooth physics interpolation

        // Lock and hide the cursor for better gameplay control
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;

        // Assign the main camera if available
        if (Camera.main)
            cameraTransform = Camera.main.transform;
    }

    private void RegisterInput()
    {
        moveX = Input.GetAxis("Horizontal"); // Get horizontal movement input
        moveZ = Input.GetAxis("Vertical");   // Get vertical movement input

        // Register a jump request if the player presses the Jump button
        if (Input.GetButtonDown("Jump"))
        {
            jumpRequest = true;
        }
    }

    private void HandleMovement()
    {
        CalculateMoveDirection(); // Compute the movement direction based on input
        HandleJump(); // Process jump input
        RotateCharacter(); // Rotate the character towards the movement direction
        MoveCharacter(); // Move the character using velocity-based movement
    }

    private void CalculateMoveDirection()
    {
        // If the camera is not assigned, move based on world space
        if (!cameraTransform)
        {
            moveDirection = new Vector3(moveX, 0, moveZ).normalized;
        }
        else
        {
            // Get forward and right vectors from the camera perspective
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;

            // Ignore Y-axis movement to prevent unwanted tilting
            forward.y = 0f;
            right.y = 0f;

            // Normalize vectors to maintain consistent movement speed
            forward.Normalize();
            right.Normalize();

            // Calculate movement direction relative to the camera orientation
            moveDirection = (forward * moveZ + right * moveX).normalized;
        }
    }

    private void HandleJump()
    {
        // Apply jump force only if jump was requested and the character is grounded
        if (jumpRequest && IsGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // Apply force upwards
            jumpRequest = false; // Reset jump request after applying jump
        }
    }

    private void RotateCharacter()
    {
        // Rotate only if the character is moving
        if (moveDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    private void MoveCharacter()
    {
        // Determine movement speed (walking or running)
        float speed = IsRunning ? baseRunSpeed : baseWalkSpeed;

        // Set ground speed value for animation purposes
        groundSpeed = (moveDirection != Vector3.zero) ? speed : 0.0f;

        // Preserve the current Y velocity to maintain gravity effects
        Vector3 newVelocity = new Vector3(
            moveDirection.x * speed * speedMultiplier, 
            rb.velocity.y, // Keep the existing Y velocity for jumping & gravity
            moveDirection.z * speed * speedMultiplier
        );

        // Apply the new velocity directly
        rb.velocity = newVelocity;
    }
}
