using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Ground Movement Settings")]
    public float runSpeed = 5f;
    public float jumpForce = 10f;

    [Header("Glide & Wind Settings")]
    public float glideSpeed = 4f;      
    public float windLiftForce = 8f;
    public float maxVerticalSpeed = 5f;
    public float gravityScale = 3f;
    public float glideGravityScale = 0.8f;

    [Header("Ground Check Settings")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isHoldingUp;
    private bool hasStartedGliding = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = gravityScale;
    }

    void Update()
    {
        // Check if player is grounded
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        isHoldingUp = Input.GetKey(KeyCode.UpArrow);

        // Jump if grounded
        if (isGrounded && Input.GetKeyDown(KeyCode.UpArrow))
        {
            Jump();
        }

        // First-time gliding activation
        if (!hasStartedGliding && !isGrounded && Input.GetKeyDown(KeyCode.UpArrow))
        {
            hasStartedGliding = true;
        }
    }

    void FixedUpdate()
    {
        Vector2 velocity = rb.velocity;

        if (isGrounded)
        {
            // Move forward when grounded
            velocity.x = runSpeed;
        }
        else
        {
            if (hasStartedGliding && isHoldingUp)
            {
                // Apply wind lift and forward glide
                velocity.x = glideSpeed;
                velocity.y += windLiftForce * Time.fixedDeltaTime;
                velocity.y = Mathf.Clamp(velocity.y, -Mathf.Infinity, maxVerticalSpeed);
                rb.gravityScale = glideGravityScale;
            }
            else
            {
                // Fall normally, keep horizontal velocity
                velocity.x = rb.velocity.x;
                rb.gravityScale = gravityScale;
            }
        }

        rb.velocity = velocity;
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }
}
