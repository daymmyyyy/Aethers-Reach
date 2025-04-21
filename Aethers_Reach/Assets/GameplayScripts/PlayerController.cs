using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float forwardSpeed = 5f;
    public float jumpForce = 10f;

    [Header("Glide Settings")]
    public bool enableGlide = true;
    public float glideGravityScale = 0.5f;
    public float normalGravityScale = 3f;
    public float maxGlideTime = 2f; // ⏱ Max glide duration in seconds

    [Header("Ground Check Settings")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool jumpPressed;
    private float currentGlideTime;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = normalGravityScale;
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
        {
            jumpPressed = true;
        }

        // Reset glide timer on landing
        if (isGrounded)
        {
            currentGlideTime = 0f;
        }
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(forwardSpeed, rb.velocity.y);

        if (jumpPressed)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpPressed = false;
        }

        // Handle gliding
        if (enableGlide && Input.GetKey(KeyCode.UpArrow) && !isGrounded && rb.velocity.y <= 0 && currentGlideTime < maxGlideTime)
        {
            rb.gravityScale = glideGravityScale;
            currentGlideTime += Time.fixedDeltaTime;
        }
        else
        {
            rb.gravityScale = normalGravityScale;
        }
    }
}
