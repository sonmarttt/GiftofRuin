using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private float groundCheckDistance = 1.1f;
    [SerializeField] private int maxJumps = 2;

    [Header("Sounds")]
    public AudioSource audioSource;
    public AudioClip jumpSound;
    public AudioClip walkSound;
    public AudioClip runSound;
    public AudioClip hurtSound;

    [Header("Speed Multiplier")]
    public float speedMultiplier = 1.0f;

    [HideInInspector] public bool isAttacking = false;

    private Rigidbody rb;
    private Transform cameraTransform;

    private float moveX;
    private float moveZ;
    private bool jumpRequest;
    private Vector3 moveDirection;
    private int jumpsRemaining;

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

        if (rb.velocity.y < 0)
            rb.velocity += Vector3.up * Physics.gravity.y * 2f * Time.fixedDeltaTime;
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
        if (IsGrounded && rb.velocity.y <= 0)
            jumpsRemaining = maxJumps;

        if (jumpRequest && jumpsRemaining > 0)
        {
            audioSource.PlayOneShot(jumpSound);
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpsRemaining--;
            jumpRequest = false;
        }
        else
        {
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

        float yVelocity = (IsGrounded && isAttacking) ? 0f : rb.velocity.y;

        Vector3 newVelocity = new Vector3(
            moveDirection.x * speed * speedMultiplier,
            yVelocity,
            moveDirection.z * speed * speedMultiplier
        );

        rb.velocity = newVelocity;
    }

    public void PlayWalkSound()
    {
        audioSource.PlayOneShot(walkSound);
    }

    public void RunSound()
    {
        audioSource.PlayOneShot(runSound);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Lava"))
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}