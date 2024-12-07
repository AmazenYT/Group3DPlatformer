using UnityEngine;

public class PlayerMovement3D : MonoBehaviour
{
    public Rigidbody rb;

    [Header("Player Settings")]
    public float speed = 5f;
    public float rotationSpeed = 2f;
    public float jumpHeight = 5f;

    [Header("Gravity Settings")]
    public float gravity = -9.81f;

    [Header("Grounding Settings")]
    public float groundCheckDistance = 1.1f;
    public LayerMask groundLayer;

    [Header("Knockback Settings")]
    public float knockbackForce = 10f;   // Force applied on collision
    public float resetHeight = -5f;      // Height at which the player resets

    [Header("Sound Settings")]
    public AudioSource audioSource;
    public AudioClip jumpSound;

    private Vector3 spawnPoint;
    private bool isGrounded;
    private Vector3 moveDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        Application.targetFrameRate = 60;

        // Save the starting position
        spawnPoint = transform.position;

        // Freeze rotation to keep the player upright unless hit
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    private void Update()
    {
        if (transform.position.y <= resetHeight)
        {
            ResetPlayer();  // Reset the player if they fall off the platform
        }

        HandleMovement();
        HandleJump();
    }

    private void FixedUpdate()
    {
        ApplyGravity();
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        moveDirection = new Vector3(horizontal, 0, vertical).normalized;

        if (moveDirection.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, targetAngle, 0f), rotationSpeed * Time.deltaTime);

            Vector3 movement = transform.forward * speed * Time.deltaTime;
            rb.MovePosition(rb.position + movement);
        }
    }

    private void HandleJump()
    {
        isGrounded = IsGrounded();

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);  // Reset Y velocity
            rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);

            if (audioSource != null && jumpSound != null)
            {
                audioSource.PlayOneShot(jumpSound);
            }
        }
    }

    private void ApplyGravity()
    {
        if (!isGrounded)
        {
            rb.AddForce(Vector3.down * gravity, ForceMode.Acceleration);
        }
    }

    private bool IsGrounded()
    {
        RaycastHit hit;
        Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;
        bool grounded = Physics.Raycast(rayOrigin, Vector3.down, out hit, groundCheckDistance, groundLayer);

        Debug.DrawRay(rayOrigin, Vector3.down * groundCheckDistance, grounded ? Color.green : Color.red);

        return grounded;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Knockback(collision);
        }
    }

    private void Knockback(Collision collision)
    {
        // Remove rotation constraints to allow falling
        rb.constraints = RigidbodyConstraints.FreezePositionY;

        // Calculate knockback direction
        Vector3 knockbackDirection = (transform.position - collision.contacts[0].point).normalized;
        knockbackDirection.y = 1f;  // Add upward force

        // Apply knockback force
        rb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
    }

    private void ResetPlayer()
    {
        // Reset position and rotation
        transform.position = spawnPoint;
        transform.rotation = Quaternion.identity;

        // Reset Rigidbody physics
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Reapply constraints
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position + Vector3.up * 0.1f, Vector3.down * groundCheckDistance);
    }
}
