using UnityEngine;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Jump Settings")]
    public float jumpForce = 10f;
    public float gravityScale = 2.5f; // Gravity multiplier for faster fall
    public float upwardGravityMultiplier = 0.5f; // Multiplier for gravity during ascent
    public int maxJumps = 2;

    [Header("Dash Settings")]
    public float dashDistance = 10f; // Distance covered by the dash
    public float dashDuration = 0.2f; // How long the dash takes
    public float dashUpwardAngle = 15f; // Angle for ground dash
    public int maxComboDashes = 2;

    [Header("Floating Settings")]
    public float floatRange = 0.2f; // Range of the floating motion
    public float floatDuration = 1f; // Duration of one float cycle

    [Header("Ground Detection")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.3f;
    public LayerMask groundLayer;

    [Header("Sphere")] public Transform sphereTransform;

    private Rigidbody rb;
    private TrailRenderer trailRenderer;
    private Vector3 moveInput;
    private bool isGrounded;
    private int jumpsRemaining;
    private int dashesRemaining;
    private bool isDashing = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        trailRenderer = GetComponent<TrailRenderer>();
    }

    private void Start()
    {
       //StartFloating();
    }

    private void Update()
    {
        if (isDashing) return;

        // Handle movement
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        moveInput = new Vector3(moveHorizontal, 0, moveVertical).normalized;

        rb.linearVelocity = new Vector3(moveInput.x * moveSpeed, rb.linearVelocity.y, moveInput.z * moveSpeed);

        // Jump input
        if (Input.GetKeyDown(KeyCode.Space) && jumpsRemaining > 0)
        {
            Jump();
        }

        // Apply custom gravity
        ApplyGravity();

        // Dash input
        if (Input.GetKeyDown(KeyCode.LeftShift) && dashesRemaining > 0)
        {
            Dash();
        }

        // Ground check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
        if (isGrounded)
        {
            jumpsRemaining = maxJumps;
            dashesRemaining = maxComboDashes;
        }
    }

    private void StartFloating()
    {
        sphereTransform.DOMoveY(sphereTransform.position.y + floatRange, floatDuration)
                 .SetEase(Ease.InOutSine)
                 .SetLoops(-1, LoopType.Yoyo);
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
        jumpsRemaining--;
    }

    private void ApplyGravity()
    {
        if (rb.linearVelocity.y > 0)
        {
            // Apply stronger gravity during ascent for a snappier peak
            rb.AddForce(Vector3.down * gravityScale * upwardGravityMultiplier, ForceMode.Acceleration);
        }
        else if (rb.linearVelocity.y < 0)
        {
            // Apply full gravity during descent
            rb.AddForce(Vector3.down * gravityScale, ForceMode.Acceleration);
        }
    }

    private void Dash()
    {
        isDashing = true;
        trailRenderer.enabled = true;

        Vector3 dashDirection = moveInput.magnitude > 0 ? moveInput : transform.right;
        if (isGrounded)
        {
            // Adjust upward angle for ground dash
            float adjustedDashAngle = dashDirection.x < 0 ? -dashUpwardAngle : dashUpwardAngle;
            dashDirection = Quaternion.Euler(0, 0, adjustedDashAngle) * dashDirection;
        }

        Vector3 targetPosition = transform.position + dashDirection.normalized * dashDistance;

        transform.DOMove(targetPosition, dashDuration)
                 .SetEase(Ease.OutQuad)
                 .OnComplete(() =>
                 {
                     isDashing = false;
                     trailRenderer.enabled = false;
                 });

        dashesRemaining--;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
