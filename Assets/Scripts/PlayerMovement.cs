using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float healthAmount = 100f;
    public Slider healthBar;

    [Header("Movement Settings")]
    [SerializeField] private float baseWalkSpeed = 5f;
    [SerializeField] private float baseRunSpeed = 8f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float groundCheckDistance = 1.1f;

    [Header("Sounds")]
    public AudioSource audioSource;
    public AudioClip jumpSound;
    public AudioClip walkSound;
    public AudioClip runSound;
    public AudioClip hurtSound;

    [Header("Speed Multiplier")]
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
        UpdateHealthBar();
    }

    private void Update()
    {
        RegisterInput();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    public void TakeDamage(int damage)
    {
        audioSource.PlayOneShot(hurtSound);
        healthAmount -= damage;
        healthAmount = Mathf.Clamp(healthAmount, 0, maxHealth);
        UpdateHealthBar();

        if (healthAmount <= 0)
            Die();
    }

    public void Heal(int healAmount)
    {
        healthAmount += healAmount;
        healthAmount = Mathf.Clamp(healthAmount, 0, maxHealth);
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null)
            healthBar.value = healthAmount / maxHealth;
    }

    private void Die()
    {
        Debug.Log("Player died");
        this.enabled = false;
        rb.velocity = Vector3.zero;
    }

    private void InitializeComponents()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;

        if (Camera.main)
            cameraTransform = Camera.main.transform;

        if (healthBar != null)
        {
            healthBar.minValue = 0;
            healthBar.maxValue = 1;
            healthBar.value = 1;
        }
    }

    private void RegisterInput()
    {
        moveX = Input.GetAxis("Horizontal");
        moveZ = Input.GetAxis("Vertical");

        if (Input.GetButtonDown("Jump"))
            jumpRequest = true;
    }

    private void HandleMovement()
    {
        CalculateMoveDirection();
        HandleJump();
        RotateCharacter();
        MoveCharacter();
    }

    private void CalculateMoveDirection()
    {
        if (!cameraTransform)
        {
            moveDirection = new Vector3(moveX, 0, moveZ).normalized;
        }
        else
        {
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();
            moveDirection = (forward * moveZ + right * moveX).normalized;
        }
    }

    private void HandleJump()
    {
        if (jumpRequest && IsGrounded)
        {
            audioSource.PlayOneShot(jumpSound);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpRequest = false;
        }
    }

    private void RotateCharacter()
    {
        if (moveDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    private void MoveCharacter()
    {
        float speed = IsRunning ? baseRunSpeed : baseWalkSpeed;
        groundSpeed = (moveDirection != Vector3.zero) ? speed : 0.0f;

        Vector3 newVelocity = new Vector3(
            moveDirection.x * speed * speedMultiplier,
            rb.velocity.y,
            moveDirection.z * speed * speedMultiplier
        );

        rb.velocity = newVelocity;
    }

    public void PlayWalkSound() {
        audioSource.PlayOneShot(walkSound);
    }

    public void RunSound() {
        audioSource.PlayOneShot(runSound);
    }
}